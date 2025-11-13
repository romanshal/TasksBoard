namespace OcelotApiGateway.Options
{
    public class RetryPolicyOptions
    {
        public int RetryCount { get; set; }
        public int RetryAttemptTimeout { get; set; }
        public int HandleBeforeBreaking { get; set; }
        public int DurationOfBreak { get; set; }
    }
}
