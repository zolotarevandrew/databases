﻿using System;
using System.Threading;
using StackExchange.Redis;

static class Redis
{
    static long lastReconnectTicks = DateTimeOffset.MinValue.UtcTicks;
    static DateTimeOffset firstError = DateTimeOffset.MinValue;
    static DateTimeOffset previousError = DateTimeOffset.MinValue;

    static object reconnectLock = new object();

    // In general, let StackExchange.Redis handle most reconnects, 
    // so limit the frequency of how often this will actually reconnect.
    public static TimeSpan ReconnectMinFrequency = TimeSpan.FromSeconds(60);

    // if errors continue for longer than the below threshold, then the 
    // multiplexer seems to not be reconnecting, so re-create the multiplexer
    public static TimeSpan ReconnectErrorThreshold = TimeSpan.FromSeconds(30);

    static string connectionString = "TODO: CALL InitializeConnectionString() method with connection string";
    static Lazy<ConnectionMultiplexer> multiplexer = CreateMultiplexer();

    public static ConnectionMultiplexer Connection { get { return multiplexer.Value; } }

    public static void InitializeConnectionString(string cnxString)
    {
        if (string.IsNullOrWhiteSpace(cnxString))
            throw new ArgumentNullException(nameof(cnxString));

        connectionString = cnxString;
    }

    /// <summary>
    /// Force a new ConnectionMultiplexer to be created.  
    /// NOTES: 
    ///     1. Users of the ConnectionMultiplexer MUST handle ObjectDisposedExceptions, which can now happen as a result of calling ForceReconnect()
    ///     2. Don't call ForceReconnect for Timeouts, just for RedisConnectionExceptions or SocketExceptions
    ///     3. Call this method every time you see a connection exception, the code will wait to reconnect:
    ///         a. for at least the "ReconnectErrorThreshold" time of repeated errors before actually reconnecting
    ///         b. not reconnect more frequently than configured in "ReconnectMinFrequency"

    /// </summary>    
    public static void ForceReconnect()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var previousTicks = Interlocked.Read(ref lastReconnectTicks);
        var previousReconnect = new DateTimeOffset(previousTicks, TimeSpan.Zero);
        var elapsedSinceLastReconnect = utcNow - previousReconnect;

        // If mulitple threads call ForceReconnect at the same time, we only want to honor one of them.
        if (elapsedSinceLastReconnect > ReconnectMinFrequency)
        {
            lock (reconnectLock)
            {
                utcNow = DateTimeOffset.UtcNow;
                elapsedSinceLastReconnect = utcNow - previousReconnect;

                if (firstError == DateTimeOffset.MinValue)
                {
                    // We haven't seen an error since last reconnect, so set initial values.
                    firstError = utcNow;
                    previousError = utcNow;
                    return;
                }

                if (elapsedSinceLastReconnect < ReconnectMinFrequency)
                    return; // Some other thread made it through the check and the lock, so nothing to do.

                var elapsedSinceFirstError = utcNow - firstError;
                var elapsedSinceMostRecentError = utcNow - previousError;

                var shouldReconnect =
                    elapsedSinceFirstError >= ReconnectErrorThreshold   // make sure we gave the multiplexer enough time to reconnect on its own if it can
                    && elapsedSinceMostRecentError <= ReconnectErrorThreshold; //make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).

                // Update the previousError timestamp to be now (e.g. this reconnect request)
                previousError = utcNow;

                if (shouldReconnect)
                {
                    firstError = DateTimeOffset.MinValue;
                    previousError = DateTimeOffset.MinValue;

                    var oldMultiplexer = multiplexer;
                    CloseMultiplexer(oldMultiplexer);
                    multiplexer = CreateMultiplexer();
                    Interlocked.Exchange(ref lastReconnectTicks, utcNow.UtcTicks);
                }
            }
        }
    }

    private static Lazy<ConnectionMultiplexer> CreateMultiplexer()
    {
        return new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
    }

    private static void CloseMultiplexer(Lazy<ConnectionMultiplexer> oldMultiplexer)
    {
        if (oldMultiplexer != null)
        {
            try
            {
                oldMultiplexer.Value.Close();
            }
            catch (Exception)
            {
                // Example error condition: if accessing old.Value causes a connection attempt and that fails.
            }
        }
    }
}