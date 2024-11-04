using System.Data;
using System.Runtime.InteropServices;
using Npgsql;

namespace Postgres;

public class IsolationLevels
{
    public static async Task ReadCommitted()
    {
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account SET amount = amount - 200 where id = 1";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            var selectSql = "SELECT * from account where client = 'alice'";
            var accounts = await LoadAccounts(selectSql, conn);
            //800
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT * from account where client = 'alice'";
            var accounts = await LoadAccounts(sql, conn);
            //1000
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1);
        var do2 = DbConnector.InTransaction(Do2);
        await Task.WhenAll(do1, do2);
    }
    
    public static async Task RepeatableRead()
    {
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account SET amount = amount - 200 where id = 1";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            var selectSql = "SELECT * from account where client = 'alice'";
            var accounts = await LoadAccounts(selectSql, conn);
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT * from account where client = 'alice'";
            var accounts1 = await LoadAccounts(sql, conn);
            //1000
            
            //ждем пока первая транзакция завершится
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            var accounts2 = await LoadAccounts(sql, conn);
            //800 - !!!
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1);
        var do2 = DbConnector.InTransaction(Do2);
        await Task.WhenAll(do1, do2);
    }
    public static async Task InsertRepeatableRead()
    {
        
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var sql = "INSERT INTO account values (7, 'test', 100)";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT * from account ORDER BY id";
            var accounts1 = await LoadAccounts(sql, conn);
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var accounts2 = await LoadAccounts(sql, conn);
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1);
        var do2 = DbConnector.InTransaction(Do2, IsolationLevel.RepeatableRead);
        await Task.WhenAll(do1, do2);
    }
    
    public static async Task ReadSkew1()
    {
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account SET amount = amount - 100 where id = 2";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            //ждем пока первая транзакция завершится
            await Task.Delay(TimeSpan.FromSeconds(1));
            
            var sql2 = "UPDATE account SET amount = amount + 100 where id = 3";
            var cmd2 = new NpgsqlCommand(sql2, conn);
            await cmd2.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT * from account where id = 2";
            var accounts1 = await LoadAccounts(sql, conn);
            //100
            
            //ждем пока первая транзакция завершится
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            var sql2 = "SELECT * from account where id = 3";
            var accounts2 = await LoadAccounts(sql2, conn);
            //1000 - !!!
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1);
        var do2 = DbConnector.InTransaction(Do2);
        await Task.WhenAll(do1, do2);
    }
    
    public static async Task ReadSkew2()
    {
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account SET amount = amount + 100 where id = 2";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            var sql2 = "UPDATE account SET amount = amount - 100 where id = 3";
            var cmd2 = new NpgsqlCommand(sql2, conn);
            await cmd2.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT amount, pg_sleep(2) from account where client = 'bob'";
            var accounts1 = await LoadAccounts(sql, conn);
            //100
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1);
        var do2 = DbConnector.InTransaction(Do2);
        await Task.WhenAll(do1, do2);
    }
    public static async Task ReadSkew3()
    {
        //200, 800 at start
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account SET amount = amount - 100 where id = 3";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account set amount = amount * 1.01 where client in (select client from account group by client having sum(amount) >= 1000)";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            var sql2 = "SELECT * from account where client = 'bob'";
            var accounts1 = await LoadAccounts(sql2, conn);
            //202, 707
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do2, IsolationLevel.RepeatableRead);
        var do2 = DbConnector.InTransaction(Do1);
        await Task.WhenAll(do1, do2);
    }
    public static async Task LostUpdate()
    {
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql2 = "SELECT * from account where id = 1";
            var accounts1 = await LoadAccounts(sql2, conn);
            //800 loaded in app!
            
            await Task.Delay(2000);
            
            var sql = "UPDATE account SET amount = 800 + 100 where id = 1";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql2 = "SELECT * from account where id = 1";
            var accounts1 = await LoadAccounts(sql2, conn);
            //800 loaded in app!
            await Task.Delay(1000);
            
            var sql = "UPDATE account SET amount = 800 + 100 where id = 1";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do2);
        var do2 = DbConnector.InTransaction(Do1);
        await Task.WhenAll(do1, do2);
    }
    
    
    public static async Task RepeatableReadWriteSkew()
    {
        /*
         * Допускаются отрицательные суммы на отдельных счетах, если общая сумма на всех счетах остается неотрицательной
         * в сумме изначально должно быть 900, на выходе получаем -400 и 100
         */
        
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT sum(amount) from account where client = 'bob'";
            var cmd = new NpgsqlCommand(sql, conn);
            var res = await cmd.ExecuteScalarAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var sql2 = "UPDATE account set amount = amount - 600 where id = 2;";
            var cmd2 = new NpgsqlCommand(sql2, conn);
            await cmd2.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
            
            var accsSql = "SELECT * from account where client = 'bob'";
            var accounts = await LoadAccounts(accsSql, conn);
            var y = accounts;
        }
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "SELECT sum(amount) from account where client = 'bob'";
            var cmd = new NpgsqlCommand(sql, conn);
            var res = await cmd.ExecuteScalarAsync();
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var sql2 = "UPDATE account set amount = amount - 600 where id = 3;";
            var cmd2 = new NpgsqlCommand(sql2, conn);
            await cmd2.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do2, IsolationLevel.RepeatableRead);
        var do2 = DbConnector.InTransaction(Do1, IsolationLevel.RepeatableRead);
        await Task.WhenAll(do1, do2);
    }
    
    public static async Task RepeatableReadOnlyReadTransactionAnomaly()
    {
        /*
         * Первая транзакция начисляет бобу проценты на сумму средств на всех счетах
         * Проценты начисляются на один из его счетов
         */
        
        async Task Do1(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            var sql = "UPDATE account set amount = amount + (select sum(amount) from account where client = 'bob') * 0.01 where id = 2;";
            var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();

            await Task.Delay(TimeSpan.FromSeconds(2));
            
            await tran.CommitAsync();
            
            var accsSql = "SELECT * from account where client = 'bob'";
            var accs = await LoadAccounts(accsSql, conn);
            var a = accs;
            //итого 910 и 0
        }
        
        /*
         * Затем другая транзакция снимает деньги с другого счета боба и фиксирует свои изменения
         * Если в этот момент первая транзакция будет зафиксирована никакой аномалии не будет
         */
        
        async Task Do2(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            var sql2 = "UPDATE account set amount = amount - 100 where id = 3;";
            var cmd2 = new NpgsqlCommand(sql2, conn);
            await cmd2.ExecuteNonQueryAsync();
            
            await tran.CommitAsync();
        }
        
        /*
         * В момент незафиксированности первой транзакции третья транзакция запрашивает суммы на счетах боба
         */
        async Task Do3(NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            await Task.Delay(TimeSpan.FromSeconds(4));
            
            var sql = "SELECT * from account where client = 'bob'";
            var accs = await LoadAccounts(sql, conn);
            
            await tran.CommitAsync();
        }
        
        var do1 = DbConnector.InTransaction(Do1, IsolationLevel.RepeatableRead);
        var do2 = DbConnector.InTransaction(Do2, IsolationLevel.RepeatableRead);
        var do3 = DbConnector.InTransaction(Do3, IsolationLevel.RepeatableRead);
        await Task.WhenAll(do1, do2, do3);
    }
    
    public static async Task SerializableReadOnlyReadTransactionAnomaly()
    {
        //читающую транзакцию обьявляем
        //begin isolation level serializable read only defferable
    }
    
    static async Task<List<Account>> LoadAccounts(string sql, NpgsqlConnection conn)
    {
        var res = new List<Account>();
        var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
        {
            var client = reader.GetString(reader.GetOrdinal("client"));
            var amount = reader.GetDecimal(reader.GetOrdinal("amount"));
            res.Add(new Account
            {
                Client = client,
                Amount = amount
            });
        }

        return res;
    }

    class Account
    {
        public string Client { get; set; }
        public decimal Amount { get; set; }
    }
}