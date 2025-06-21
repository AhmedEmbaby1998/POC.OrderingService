internal partial class ResilientDbConnection
{
    public class SqlResilienceOptions
    {
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 20;
        public int BreakDurationInMinutes { get; set; } = 2;
        public int RetryCount { get; set; } = 3;
        public double RetryBaseDelayInSeconds { get; set; } = 2;
    }
 
}