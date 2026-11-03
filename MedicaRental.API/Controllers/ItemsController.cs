using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicaRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAll(string filters, string orderby, string includes)
        {
            Console.WriteLine();

            return Ok();
        }
    }
}
