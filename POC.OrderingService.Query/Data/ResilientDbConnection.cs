
using System.Data;
using Microsoft.Data.SqlClient;
using Polly;

internal class ResilientDbConnection : IDbConnection
{
    private readonly IDbConnection _inner;
    private readonly Policy _resiliencePolicy;

    public ResilientDbConnection(IDbConnection inner)
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

    public string ConnectionString
    {
        get => _inner.ConnectionString;
        set => _inner.ConnectionString = value;
    }

    public int ConnectionTimeout => _inner.ConnectionTimeout;
    public string Database => _inner.Database;
    public ConnectionState State => _inner.State;

    public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
    public void Close() => _inner.Close();
    public IDbTransaction BeginTransaction() => _inner.BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel il) => _inner.BeginTransaction(il);

    public void Dispose() => _inner.Dispose(); 
    public IDbCommand CreateCommand()
    {
        return ExecuteWithResilience(() => _inner.CreateCommand());
    }
    public void Open()
    {
        ExecuteWithResilience(() => {
            _inner.Open();
            return 1;
        });
    }
}

