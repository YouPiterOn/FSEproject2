using FSEProject2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("forget")]
        public ActionResult<UserId> Forget(string userId)
        {
            var response = UserActions.Forget(userId);

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
