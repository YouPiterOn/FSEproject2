using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2
{
    public class Predictions
    {
        public static PredictionData PredictUsersOnline(DateTime date)
        {
            List<DateTime> onlineData = new List<DateTime>();
            foreach (var user in Data.Users)
                foreach (var dateOnline in user.wasOnline)
                    onlineData.Add(dateOnline);

            var dayOfWeek = date.DayOfWeek;
            var relevantData = onlineData.FindAll(x => x.DayOfWeek == dayOfWeek && x.Hour == date.Hour);
            var usersCounts = relevantData.GroupBy(x => x).Select(g => g.Count()).ToList();
            if (usersCounts.Count == 0) return new PredictionData { onlineUsers = null };

            var averageUsers = (int)usersCounts.Average();
            return new PredictionData { onlineUsers = averageUsers };
        }

        public static UserPredictionData PredictUserOnline(DateTime date, double tolerance, string userId)
        {

            var user = Data.Users.FirstOrDefault(u => u.userId == userId);

            if (user == null) return null;

            var totalWeeks = (int)(date.Subtract(user.wasOnline.Min()).TotalDays / 7);
            var timesUserWasOnline = user.wasOnline.Count(x => x.DayOfWeek == date.DayOfWeek && x.Hour == date.Hour);
            var onlineChance = timesUserWasOnline / totalWeeks;

            return new UserPredictionData { willBeOnline = onlineChance >= tolerance, onlineChance = onlineChance };
        }
    }
}
