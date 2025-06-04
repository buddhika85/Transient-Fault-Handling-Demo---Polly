using Microsoft.AspNetCore.Mvc;
using Polly.Timeout;
using Polly;
using RequestService.Policies;
using Polly.Bulkhead;
using System.Net;

namespace RequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly ClientPolicy clientPolicy;
        private readonly IHttpClientFactory clientFactory;

        public RequestController(ClientPolicy clientPolicy, IHttpClientFactory clientFactory)
        {
            this.clientPolicy = clientPolicy;
            this.clientFactory = clientFactory;
        }

        //// GET api/response/30
        //[Route("{id}")]
        //[HttpGet]
        //public async Task<IActionResult> MakeRequest(int id)
        //{
        //    var client = new HttpClient();
        //    //var response = await client.GetAsync($"https://localhost:7297/api/response/{id}");        // run once - no polly

        //    // 1 using polly, if failed, retry 5 times immediately 
        //    //var response = await clientPolicy.ImmediateHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

        //    // 2 using polly, if failed, retry 5 times, 3 seconds waits in between 
        //    //var response = await clientPolicy.LinearHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

        //    // 3 using polly, if failed, retry 5 times, 3 seconds waits in between 
        //    var response = await clientPolicy.ExponetialHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

        //    if (response.IsSuccessStatusCode) 
        //    {
        //        Console.WriteLine("----> ResponseService returned SUCCESS");
        //        return StatusCode(StatusCodes.Status200OK, id);
        //    }
        //    Console.WriteLine("----> ResponseService returned FAILURE");
        //    return StatusCode(StatusCodes.Status500InternalServerError, id);
        //}


        // using HTTP client factory to create HTTP Client
        // GET api/response/MakeRequestRetry/30
        [Route("MakeRequestRetry/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequestRetry(int id)
        {
            var client = clientFactory.CreateClient("PollyPolicyCleint");
            

            // using polly retries configured in Program.cs
            var response = await client.GetAsync($"https://localhost:7297/api/response/{id}");

          

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("----> ResponseService returned SUCCESS");
                return StatusCode(StatusCodes.Status200OK, id);
            }
            Console.WriteLine("----> ResponseService returned FAILURE");
            return StatusCode(StatusCodes.Status500InternalServerError, id);
        }



        // GET api/response/MakeRequestCircuiteBreaker/30
        [Route("MakeRequestCircuiteBreaker/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequestCircuiteBreaker(int id)
        {
            var client = new HttpClient();
           

            // using polly, if failed
            // do immediate 10 requests
            // if all fails then take a break for  5 seconds
            // do immediate 10 requests 
            // ..
            var response = await clientPolicy.CircuiteBreakerHttpPolicy.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("----> ResponseService returned SUCCESS");
                return StatusCode(StatusCodes.Status200OK, id);
            }
            Console.WriteLine("----> ResponseService returned FAILURE");
            return StatusCode(StatusCodes.Status500InternalServerError, id);
        }


        // Apply the timeout policy when making an HTTP request
        [Route("MakeRequestWithTimeout/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequestWithTimeout(int id)
        {
            var client = new HttpClient();

            try
            {
                var response = await clientPolicy.TimeOutPolicy.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("----> ResponseService returned SUCCESS");
                    return StatusCode(StatusCodes.Status200OK, id);
                }
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine("----> Request timed out!");
                return StatusCode(StatusCodes.Status408RequestTimeout, id);
            }

            Console.WriteLine("----> ResponseService returned FAILURE");
            return StatusCode(StatusCodes.Status500InternalServerError, id);
        }


        

        [Route("MakeRequestBulkhead/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequestBulkhead(int id)
        {
            var client = new HttpClient();

            var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 5, maxQueuingActions: 10);

            try
            {
                var response = await bulkheadPolicy.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("----> ResponseService returned SUCCESS");
                    return StatusCode(StatusCodes.Status200OK, id);
                }
            }
            catch (BulkheadRejectedException)
            {
                Console.WriteLine("----> Request rejected due to bulkhead limit!");
                return StatusCode(StatusCodes.Status429TooManyRequests, id);
            }

            Console.WriteLine("----> ResponseService returned FAILURE");
            return StatusCode(StatusCodes.Status500InternalServerError, id);
        }


     

        [Route("MakeRequestFallback/{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequestFallback(int id)
        {
            var client = new HttpClient();

            var fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
 .FallbackAsync(new HttpResponseMessage(HttpStatusCode.OK)
 {
     Content = new StringContent("Fallback response: Service unavailable, please try again later.")
 });

            var response = await fallbackPolicy.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

            Console.WriteLine("----> ResponseService returned: " + await response.Content.ReadAsStringAsync());
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
