
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Polly;

internal class ResilientDbConnection : DbConnection
{
    private readonly DbConnection _inner;
    private readonly Policy _resiliencePolicy;


    public ResilientDbConnection(DbConnection inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));

        var circuitBreakerPolicy = Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
            .CircuitBreaker(
                exceptionsAllowedBeforeBreaking: 20,
                durationOfBreak: TimeSpan.FromMinutes(2)
                );

        var reTryPolicy = Policy
             .Handle<SqlException>()
             .Or<TimeoutException>()
             .WaitAndRetry(3,
             retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                     (exception, timeSpan) =>
                     {
                         // Log the exception or take any other action
                         Console.WriteLine($"An error occurred: {exception.Message}. Waiting {timeSpan} before next retry.");
                     });

        _resiliencePolicy = Policy.Wrap(circuitBreakerPolicy, reTryPolicy);
    }

    private T ExecuteWithResilience<T>(Func<T> operation)
    {
        return _resiliencePolicy.Execute(() =>
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }



    public override string ConnectionString { get => _inner.ConnectionString; set => _inner.ConnectionString = value; }

    public override string Database => _inner.Database;

    public override string DataSource => _inner.DataSource;

    public override string ServerVersion => _inner.ServerVersion;

    public override ConnectionState State => _inner.State;
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
    {
        return _inner.BeginTransaction(isolationLevel);
    }

    public override void ChangeDatabase(string databaseName)
    {
        _inner.ChangeDatabase(databaseName);
    }

    public override void Close()
    {
        _inner.Close();
    }

    protected override DbCommand CreateDbCommand()
    {
        return ExecuteWithResilience(() => _inner.CreateCommand());
    }

    public override void Open()
    {
        ExecuteWithResilience(() => {
            _inner.Open();
            return 1;
        });
    }
 
}