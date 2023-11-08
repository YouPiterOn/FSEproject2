using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FSEProject2
{
    public static class Stats
    {
        public static HistoricalData GetUsersOnline(DateTime date)
        {
            List<DateTime> onlineData = new List<DateTime>();
			foreach(var wasOnline in Data.Users.Select(user => user.wasOnline))
			{
				if (wasOnline == null) continue;
				foreach (var dateOnline in wasOnline)
					onlineData.Add(dateOnline);
			}

			var usersCount = onlineData.FindAll(x => x == date).Count;
            if (usersCount == 0) return new HistoricalData { usersOnline = null };
            return new HistoricalData { usersOnline = usersCount };
        }

        public static UserHistoricalData? GetUserStats(DateTime date, string userId)
        {
            var user = Data.Users.Find(u => u.userId == userId);

            if (user == null || user.wasOnline == null)
            {
                return null;
            }

            bool? wasUserOnline = null;
            DateTime? nearestOnlineTime = null;

            if (user.wasOnline.Contains(date))
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

        public static UserTimeData? GetUserTimeData(string userId) 
        {
            var user = Data.Users.Find(u => u.userId == userId);
            if (user == null) 
            {
                return null;
            }
            if (user.periodsOnline == null)
            {
                return new UserTimeData { totalTime = 0 };
            }
            var result = (int)Enumerable.Sum(user.periodsOnline.Select(x => (x.end - x.start).TotalSeconds));
            return new UserTimeData { totalTime = result };
        }

        public static UserAverageTime? GetUserAverageTimeOnline(string userId)
        {
            var user = Data.Users.Find(x => x.userId == userId);
            if (user == null)
            {
                return null;
            }
            if (user.periodsOnline == null)
            {
                return new UserAverageTime { weeklyAverage = 0, dailyAverage = 0 };
            }
            var weeks = user.periodsOnline.Select(x => new
            {
                Period = x,
                Year = x.start.Year,
                Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear
                (x.start, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
            .GroupBy(x => new { x.Year, x.Week });
            var secondsWeek = weeks.Select(y => Enumerable.Sum(y.Select(x => (x.Period.end - x.Period.start).TotalSeconds)));

            var days = user.periodsOnline.GroupBy(x => x.start.Date);
            var secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));

            var weeklyAverage = (int)secondsWeek.Average();
            var dailyAverage = (int)secondsDay.Average();
            return new UserAverageTime { weeklyAverage = weeklyAverage, dailyAverage = dailyAverage };
        }
    }
}
