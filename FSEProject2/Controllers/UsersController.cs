using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("list")]
        public ActionResult<List<FirstSeen>> GetUsersList()
        {
            var response = UsersActions.GetUsersList();

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
