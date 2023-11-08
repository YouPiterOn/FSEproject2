using FSEProject2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FSEProject2.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {

        [HttpGet("users")]
        public ActionResult<HistoricalData> GetUsersOnline(string date)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", CultureInfo.InvariantCulture);

            var response = Stats.GetUsersOnline(actualDate);

            if (response == null) { return NotFound(); }
            return response;
        }

        [HttpGet("user")]
        public ActionResult<UserHistoricalData> GetUserStats(string date, string userId)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", CultureInfo.InvariantCulture);

            var response = Stats.GetUserStats(actualDate, userId);

            if (response == null) { return NotFound(); }
            return response;
        }

        [HttpGet("user/total")]
        public ActionResult<UserTimeData> GetUserTimeData(string userId)
        {
            var response = Stats.GetUserTimeData(userId);

            if (response == null) { return NotFound(); }
            return response;
        }

        [HttpGet("user/average")]
        public ActionResult<UserAverageTime> GetUserAverageTimeOnline(string userId)
        {
            var response = Stats.GetUserAverageTimeOnline(userId);

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
