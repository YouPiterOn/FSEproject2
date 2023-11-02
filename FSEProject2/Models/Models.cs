namespace FSEProject2.Models
{
    public class PeriodOnline
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
    public class HistoricalData
    {
        public int? usersOnline { get; set; }
    }

    public class UserHistoricalData
    {
        public bool? wasUserOnline { get; set; }
        public DateTime? nearestOnlineTime { get; set; }
    }

    public class PredictionData
    {
        public int? onlineUsers { get; set; }
    }

    public class UserPredictionData
    {
        public bool willBeOnline { get; set; }
        public double onlineChance { get; set; }
    }

    public class UserTimeData
    {
        public double totalTime { get; set; }
    }

    public class UserAverageTime
    {
        public double weeklyAverage { get; set; }
        public double dailyAverage { get; set; }
    }
    public class UserId
    {
        public string userId { get; set; }
    }
    public class ReportRequest
    {
        public List<string>? metrics { get; set; }
        public List<string>? users { get; set; }
    }
    public class Report
    {
        public string? userId { get; set; }
        public List<Object>? metrics { get; set; }
    }
    public class FirstSeen
    {
        public string? username { get; set;}
        public string? userId { get; set; }
        public DateTime? firstSeen { get; set;}
    }   
}

