using FSEProject2.Models;
using System.Globalization;

namespace FSEProject2
{
    public static class Reports
    {
        public static Object? CreateReport(string name, ReportRequest request)
        {
            if (request.users == null || request.metrics == null) return null;
            request.name = name;
            Data.ReportRequests.Add(request);
            return new Object() { };
        }
        public static Report? GetReport(string name, DateTime from, DateTime to)
        {
            var result = new Report()
            {
                userReports = new List<UserReport?>(),
                dailyAverage = 0,
                weeklyAverage = 0,
                total = 0,
                min = 0,
                max = 0
            };
            var request = Data.ReportRequests.Find(x => x.name == name);
            if (request == null) return null;
            if (request.users == null) return null;

            foreach(var userId in request.users)
            {
                var user = Data.Users.Find(x => x.userId == userId);
                if (user == null)
                {
                    result.userReports.Add(null);
                    continue;
                }
                var report = new UserReport() { userId = user.userId, metrics = new List<Object?>() };
                if(request.metrics == null) 
                { 
                    report.metrics = null; 
                    continue; 
                }
                foreach(var metric in request.metrics)
                {
                    switch(metric)
                    {
                        case "dailyAverage":
                            if (user.periodsOnline == null) 
                            {
                                report.metrics.Add(null);
                                break;
                            }
                            var days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
                            var secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
                            var dailyAverage = (int)secondsDay.Average();
                            report.metrics.Add(new { dailyAverage = dailyAverage });
                            result.dailyAverage += dailyAverage;
                            break;

                        case "weeklyAverage":
                            if (user.periodsOnline == null)
                            {
                                report.metrics.Add(null);
                                break;
                            }
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
                            report.metrics.Add(new { weeklyAverage = weeklyAverage });
                            result.weeklyAverage += weeklyAverage;
                            break;

                        case "total":
                            if (user.periodsOnline == null)
                            {
                                report.metrics.Add(null);
                                break;
                            }
                            var total = user.periodsOnline.Where(x => from < x.start && x.end < to).Select(x => (x.end - x.start).TotalSeconds).Sum();
                            report.metrics.Add(new { total = (int)total });
                            result.total += (int)total;
                            break;
                        case "min":
                            if (user.periodsOnline == null)
                            {
                                report.metrics.Add(null);
                                break;
                            }
                            days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
                            secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
                            var min = (int)secondsDay.Min();
                            report.metrics.Add(new { min = min });
                            result.min += min;
                            break;
                        case "max":
                            if (user.periodsOnline == null)
                            {
                                report.metrics.Add(null);
                                break;
                            }
                            days = user.periodsOnline.FindAll(x => from < x.start && x.end < to).GroupBy(x => x.start.Date);
                            secondsDay = days.Select(y => Enumerable.Sum(y.Select(x => (x.end - x.start).TotalSeconds)));
                            var max = (int)secondsDay.Max();
                            report.metrics.Add(new { max = max });
                            result.max += max;
                            break;
                    }
                }
                result.userReports.Add(report);
            }
            return result;
        }
    }
}
