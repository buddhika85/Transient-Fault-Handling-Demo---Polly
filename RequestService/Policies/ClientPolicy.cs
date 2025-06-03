using Polly;
using Polly.Retry;

namespace RequestService.Policies
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }

        public AsyncRetryPolicy<HttpResponseMessage> ExponetialHttpRetry { get; }

        public ClientPolicy()
        {
            // if the response is not a http successcode,
            
            // retry for another 5 attempts
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).RetryAsync(5);

            // retry for another 5 attempts, while waiting 3 seconds in between
            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

            // retry for another 5 attempts, while waiting 1 seconds first, 4 seconeds second, 9 seconds third
            ExponetialHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => 
                                                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
