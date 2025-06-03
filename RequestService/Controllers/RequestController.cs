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

        //// GET api/response/100
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
        // GET api/response/100
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequest(int id)
        {
            var client = clientFactory.CreateClient("PollyPolicyCleint");
            //var response = await client.GetAsync($"https://localhost:7297/api/response/{id}");        // run once - no polly

            // 1 using polly, if failed, retry 5 times immediately 
            var response = await clientPolicy.ImmediateHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

            // 2 using polly, if failed, retry 5 times, 3 seconds waits in between 
            //var response = await clientPolicy.LinearHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

            // 3 using polly, if failed, retry 5 times, 3 seconds waits in between 
            //var response = await clientPolicy.ExponetialHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

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
