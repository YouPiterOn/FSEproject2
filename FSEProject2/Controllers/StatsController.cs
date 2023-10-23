using FSEProject2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {

        [HttpGet("users")]
        public ActionResult<HistoricalData> GetUsersOnline(string date)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);

            var response = Stats.GetUsersOnline(actualDate);

            if (response == null) { return NotFound(); }
            return response;
        }

        [HttpGet("user")]
        public ActionResult<UserHistoricalData> GetUserStats(string date, string userId)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);

            var response = Stats.GetUserStats(actualDate, userId);

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
