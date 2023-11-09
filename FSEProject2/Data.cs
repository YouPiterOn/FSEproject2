using FSEProject2.Models;

namespace FSEProject2
{
    public static class Data
    {
#pragma warning disable S2223 // Non-constant static fields should not be visible
#pragma warning disable S1104 // Fields should not have public accessibility
#pragma warning disable S2386 // Mutable fields should not be "public static"
        public static List<User> Users = new List<User>();

        public static List<string> ForgottenUsers = new List<string>();


        public static List<ReportRequest> ReportRequests = new List<ReportRequest>();
#pragma warning restore S2223 // Non-constant static fields should not be visible
#pragma warning restore S1104 // Fields should not have public accessibility
#pragma warning restore S2386 // Mutable fields should not be "public static"
    }
}
