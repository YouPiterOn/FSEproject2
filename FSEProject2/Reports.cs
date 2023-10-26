using FSEProject2.Models;
using System.Globalization;

namespace FSEProject2
{
    public class Reports
    {
        private static Object DailyAverage(User user, DateTime from, DateTime to)
        {
            if (user.periodsOnline == null) return null;
            var days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
            var secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
            var dailyAverage = (int)secondsDay.Average();
            return new { dailyAverage = dailyAverage };
        }
        private static Object WeeklyAverage(User user, DateTime from, DateTime to)
        {
            if (user.periodsOnline == null) return null;
            var weeks = user.periodsOnline.Where(x => from < x.start && x.end < to).Select(x => new
            {
                Period = x,
                Year = x.start.Year,
                Week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear
                (x.start, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
            })
            .GroupBy(x => new { x.Year, x.Week });
            var secondsWeek = weeks.Select(y => Enumerable.Sum(y.Select(x => (x.Period.end - x.Period.start).TotalSeconds)));
            var weeklyAverage = (int)secondsWeek.Average();
            return new { weeklyAverage = weeklyAverage };
        }
        private static Object Total(User user, DateTime from, DateTime to) 
        {
            if (user.periodsOnline == null) return null;
            var total = user.periodsOnline.Where(x => from < x.start && x.end < to).Select(x => (x.end - x.start).TotalSeconds).Sum();
            return new { total = (int)total };
        }
        private static Object Min(User user, DateTime from, DateTime to)
        {
            if (user.periodsOnline == null) return null;
            var days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
            var secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
            var min = (int)secondsDay.Min();
            return new { min = min };
        }
        private static Object Max(User user, DateTime from, DateTime to)
        {
            if (user.periodsOnline == null) return null;
            var days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
            var secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
            var max = (int)secondsDay.Max();
            return new { max = max };
        }


        public static Object CreateReport(string name, ReportRequest request)
        {
            if (request.users == null || request.metrics == null) return null;
            Data.ReportRequests.Add(name, request);
            return new Object() { };
        }
        public static List<Report?> GetReport(string name, DateTime from, DateTime to)
        {
            var result = new List<Report?>();
            var request = Data.ReportRequests.GetValueOrDefault(name);
            if (request == null) return null;
            foreach(var userId in request.users)
            {
                var user = Data.Users.FirstOrDefault(x => x.userId == userId);
                if (user == null)
                {
                    result.Add(null);
                }
                var report = new Report() { userId = user.userId, metrics = new List<Object>() };
                foreach(var metric in request.metrics)
                {
                    switch(metric)
                    {
                        case "dailyAverage":
                            report.metrics.Add(DailyAverage(user, from, to));
                            break;
                        case "weeklyAverage":
                            report.metrics.Add(WeeklyAverage(user, from, to));
                            break;
                        case "total":
                            report.metrics.Add(Total(user, from, to));
                            break;
                        case "min":
                            report.metrics.Add(Min(user, from, to));
                            break;
                        case "max":
                            report.metrics.Add(Max(user, from, to));
                            break;
                    }
                }
                result.Add(report);
            }
            return result;
        }
    }
}
