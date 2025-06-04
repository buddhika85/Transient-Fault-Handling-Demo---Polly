using Polly;
using Polly.Retry;
using System.Diagnostics.Metrics;
using System;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace RequestService.Policies
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; }

        public AsyncRetryPolicy<HttpResponseMessage> ExponetialHttpRetry { get; }


        public AsyncPolicy<HttpResponseMessage> CircuiteBreakerHttpPolicy { get; }

        public AsyncTimeoutPolicy<HttpResponseMessage> TimeOutPolicy { get; }

        public ClientPolicy()
        {
            // if the response is not a http successcode,

            #region RETRY_POLICIES
            // retry for another 5 attempts
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).RetryAsync(5);

            // retry for another 5 attempts, while waiting 3 seconds in between
            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

            // can use exponential backoff to gradually increase wait time between retries.
            // retry for another 5 attempts, while waiting 1 seconds first, 4 seconeds second, 9 seconds third
            ExponetialHttpRetry = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode).WaitAndRetryAsync(5, retryAttempt => 
                                                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            #endregion

            #region CIRCUITE_BREAKER
            // Prevents repeated failures by temporarily blocking requests after a threshold is reached.
            // do immediate 10 requests
            // if all fails then take a break for  5 seconds
            // do immediate 10 requests 
            // ..

            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                            .RetryAsync(10); // Retries 10 times

            // Breaks after 10 failures, and rests for 5 seconds
            var circuiteBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 10, durationOfBreak: TimeSpan.FromSeconds(5));

            CircuiteBreakerHttpPolicy = Policy.WrapAsync(retryPolicy, circuiteBreakerPolicy);


            #endregion region


            // Define a timeout policy that cancels requests if they exceed 5 seconds
            TimeOutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));

        }
    }
}
