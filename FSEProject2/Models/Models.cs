namespace FSEProject2.Models
{
    public class UserOnline
    {
        public User user { get; set; }
        public DateTime date { get; set; }
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
}
