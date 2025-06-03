using Microsoft.AspNetCore.Mvc;

namespace ResponseAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        // GET api/response/100
        // if you pass 100 or more you have more chance of getting Ok response
        // if you pass something like 1 you have high chance of getting 500 internal server error/ failure
        [Route("{id}")]
        [HttpGet]
        public ActionResult GetResponse(int id)
        {
            var random = new Random();
            var randomNum = random.Next(1, 101);
            if (randomNum >= id)
            {
                Console.WriteLine("----> Failure - Generated 500");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Console.WriteLine("----> Success - Generated 200");
            return StatusCode(StatusCodes.Status200OK, $"{id} >= random {randomNum} == true");
        }
    }
}
