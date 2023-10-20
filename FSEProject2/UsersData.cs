using System;

namespace FSEProject2
{
    public class UsersData
    {
        public string nickname { get; set; }
        public string userId { get; set; }
        public DateTime? lastSeenDate { get; set; }
    }

    public class AllData
    {
        public List<UsersData> data { get; set; }
    }
}
