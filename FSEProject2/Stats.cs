using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2
{
    public class Stats
    {
        public static HistoricalData GetUsersOnline(DateTime date)
        {
            List<DateTime> onlineData = new List<DateTime>();
            foreach (var user in Data.Users)
                foreach(var dateOnline in user.wasOnline)
                    onlineData.Add(dateOnline);

            var usersCount = onlineData.FindAll(x => x == date).Count;
            if (usersCount == 0) return new HistoricalData { usersOnline = null };
            return new HistoricalData { usersOnline = usersCount };
        }
        public static UserHistoricalData GetUserStats(DateTime date, string userId)
        {
            var user = Data.Users.FirstOrDefault(u => u.userId == userId);

            if (user == null)
            {
                return null;
            }

            bool? wasUserOnline = null;
            DateTime? nearestOnlineTime = null;

            if (user.wasOnline.Any(x => x == date))
            {
                wasUserOnline = true;
            }
            else 
            {
                wasUserOnline = false;

                nearestOnlineTime = user.wasOnline.OrderBy(x => Math.Abs((x - date).Ticks)).FirstOrDefault();
            }

            return new UserHistoricalData { wasUserOnline = wasUserOnline, nearestOnlineTime = nearestOnlineTime };
        }
    }
}
