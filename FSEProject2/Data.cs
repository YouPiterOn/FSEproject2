﻿using FSEProject2.Models;

namespace FSEProject2
{
    public class Data
    {
        public static List<User> Users = new List<User>();

        public static List<string> ForgottenUsers = new List<string>();

        public static Dictionary<string, ReportRequest> ReportRequests = new Dictionary<string, ReportRequest>();
    }
}
