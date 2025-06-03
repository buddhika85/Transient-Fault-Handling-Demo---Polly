using Microsoft.AspNetCore.Mvc;
using RequestService.Policies;

namespace RequestService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly ClientPolicy clientPolicy;

        public RequestController(ClientPolicy clientPolicy)
        {
            this.clientPolicy = clientPolicy;
        }

        // GET api/response/100
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> MakeRequest(int id)
        {
            var client = new HttpClient();
            //var response = await client.GetAsync($"https://localhost:7297/api/response/{id}");

            var response = await clientPolicy.ImmediateHttpRetry.ExecuteAsync(() => client.GetAsync($"https://localhost:7297/api/response/{id}"));

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
