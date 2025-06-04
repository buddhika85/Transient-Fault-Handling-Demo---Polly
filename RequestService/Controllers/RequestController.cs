using Microsoft.AspNetCore.Mvc;
using RequestService.Policies;

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
    }
}
