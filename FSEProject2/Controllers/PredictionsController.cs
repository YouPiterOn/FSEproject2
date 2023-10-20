using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/predictions")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        public List<UserOnline> usersData = new List<UserOnline>();

        [HttpGet("users")]
        public ActionResult<PredictionData> PredictUsersOnline(string date)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);
            var dayOfWeek = actualDate.DayOfWeek;
            List<UserOnline> relevantData = usersData.FindAll(x => x.date.DayOfWeek == dayOfWeek && x.date.Hour == actualDate.Hour);
            var usersCounts = relevantData.GroupBy(x => x.date).Select(g => g.Count()).ToList();
            var averageUsers = (int)usersCounts.Average();

            return new PredictionData { onlineUsers = averageUsers };
        }

        [HttpGet("user")]
        public ActionResult<UserPredictionData> PredictUserOnline(string date, double tolerance, string userId)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);
            var dayOfWeek = actualDate.DayOfWeek;

            var userOnlineData = usersData.FindAll(u => u.user.userId == userId);

            if (userOnlineData.Count == 0) return NotFound();

            var totalWeeks = (int)(actualDate.Subtract(userOnlineData.Min(x => x.date)).TotalDays / 7);
            var timesUserWasOnline = userOnlineData.Count(x => x.date.DayOfWeek == dayOfWeek && x.date.Hour == actualDate.Hour);
            var onlineChance = timesUserWasOnline / totalWeeks;

            return new UserPredictionData { willBeOnline = onlineChance >= tolerance, onlineChance = onlineChance };
        }
    }
}
