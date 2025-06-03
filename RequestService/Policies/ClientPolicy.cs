using Polly;
using Polly.Retry;

namespace RequestService.Policies
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }

        public ClientPolicy()
        {
            // if the response is not a http successcode,
            
            // retry for another 5 attempts
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).RetryAsync(5);

            // retry for another 5 attempts, while waiting 3 seconds in between
            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));
        }
    }
}
