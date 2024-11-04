using System.Data;
using Npgsql;

namespace Postgres;

public class DbConnector
{
    static string ConnectionString = "Host=localhost;Username=admin;Password=root;Database=postgres";
    
    public static async Task InTransaction(Func<NpgsqlConnection, NpgsqlTransaction, Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var transaction = await conn.BeginTransactionAsync(isolationLevel);

        await action(conn, transaction);
    }
}