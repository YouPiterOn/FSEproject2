using FSEProject2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        public List<UserOnline> usersData = new List<UserOnline>();

        [HttpGet("users")]
        public ActionResult<HistoricalData> GetUsersOnline(string date)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);
            List<UserOnline> users = usersData.FindAll(x => x.date == actualDate);
            if(users.Count == 0) return new HistoricalData { usersOnline = null };
            return new HistoricalData { usersOnline = users.Count };
        }

        [HttpGet("user")]
        public ActionResult<UserHistoricalData> GetUserStats(string date, string userId)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);

            var userOnlineData = usersData.FirstOrDefault(u => u.user.userId == userId);

            if (userOnlineData == null)
            {
                return NotFound();
            }

            bool? wasUserOnline = null;
            DateTime? nearestOnlineTime = null;

            if (userOnlineData.date == actualDate)
            {
                wasUserOnline = true;
            }
            else if (usersData.Any(u => u.user.userId == userId))
            {
                wasUserOnline = false;

                nearestOnlineTime = usersData.Where(u => u.user.userId == userId).OrderBy(u => Math.Abs((u.date - actualDate).Ticks)).FirstOrDefault()?.date;
            }

            return new UserHistoricalData { wasUserOnline = wasUserOnline, nearestOnlineTime = nearestOnlineTime};
        }
    }
}
