using System;

namespace FSEProject2.Models
{
    public class User
    {
        public string? nickname { get; set; }
        public string? userId { get; set; }
        public DateTime? lastSeenDate { get; set; }
        public List<DateTime>? wasOnline { get; set; }
    }

    public class AllData
    {
        public List<User> data { get; set; }
    }
}
