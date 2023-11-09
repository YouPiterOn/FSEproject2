using FSEProject2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FSEProject2.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            Data.Users = new List<User>
            {
            new User {userId = "1", wasOnline = new List<DateTime>(){
                DateTime.ParseExact("2023-10-10-12:00", "yyyy-dd-MM-HH:mm", null),
                DateTime.ParseExact("2023-17-10-12:00", "yyyy-dd-MM-HH:mm", null) } },
            new User {userId = "2", wasOnline = new List<DateTime>(){
                DateTime.ParseExact("2023-10-10-12:00", "yyyy-dd-MM-HH:mm", null),
                DateTime.ParseExact("2023-11-10-12:00", "yyyy-dd-MM-HH:mm", null) }},
            new User { userId = "4", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = DateTime.Now.AddHours(-1), end = DateTime.Now },
                    new PeriodOnline { start = DateTime.Now.AddHours(-3), end = DateTime.Now.AddHours(-2) }}},
            new User { userId = "5", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = DateTime.Now.AddHours(-1), end = DateTime.Now },
                    new PeriodOnline { start = DateTime.Now.AddHours(-25), end = DateTime.Now.AddHours(-24) },
                    new PeriodOnline { start = DateTime.Now.AddDays(-8), end = DateTime.Now.AddDays(-7) }}}
            };
        }

        [TestMethod()]
        public async Task PredictUsersOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/predictions/users?date=2023-24-10-12:00");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();
            var expected = 1;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PredictionData>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.onlineUsers);
        }
        [TestMethod]
        public async Task PredictUsersOnline_Null()
        {
            var response = await _client.GetAsync("/api/predictions/users?date=2023-23-10-12:00");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PredictionData>(content);

            Assert.IsNotNull(result);
            Assert.IsNull(result.onlineUsers);
        }
        [TestMethod()]
        public async Task PredictUserOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/predictions/user?date=2023-24-10-12:00&tolerance=0.85&userId=1");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();
            var expected = 1;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserPredictionData>(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.willBeOnline);
            Assert.AreEqual(expected, result.onlineChance);
        }
        [TestMethod]
        public async Task PredictUserOnline_NotFound()
        {
            var response = await _client.GetAsync("/api/predictions/user?date=2023-24-10-12:00&tolerance=0.85&userId=3");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task GetUsersOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/stats/users?date=2023-10-10-12:00");
            response.EnsureSuccessStatusCode();
            var expected = 2;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HistoricalData>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.usersOnline);
        }
        [TestMethod]
        public async Task GetUsersOnline_Null()
        {
            var response = await _client.GetAsync("/api/stats/users?date=2023-09-10-12:00");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HistoricalData>(content);

            Assert.IsNotNull(result);
            Assert.IsNull(result.usersOnline);
        }
        [TestMethod()]
        public async Task GetUserStats_CorrectResponseWasOnline()
        {
            var response = await _client.GetAsync("/api/stats/user?date=2023-10-10-12:00&userId=1");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserHistoricalData>(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.wasUserOnline);
            Assert.IsNull(result.nearestOnlineTime);
        }
        [TestMethod()]
        public async Task GetUserStats_CorrectResponseWasOffline()
        {
            var response = await _client.GetAsync("/api/stats/user?date=2023-09-10-12:00&userId=1");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();
            var expected = DateTime.ParseExact("2023-10-10-12:00", "yyyy-dd-MM-HH:mm", null);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserHistoricalData>(content);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.wasUserOnline);
            Assert.AreEqual(expected, result.nearestOnlineTime);
        }
        [TestMethod]
        public async Task GetUserStats_NotFound()
        {
            var response = await _client.GetAsync("/api/stats/user?date=2023-10-10-12:00&userId=3");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
        [TestMethod()]
        public async Task GetUserTimeData_CorrectCount()
        {
            var expected = 7200;
            var response = await _client.GetAsync("/api/stats/user/total?userId=4");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserTimeData>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.totalTime);
        }

        [TestMethod()]
        public async Task GetUserTimeData_NotFound()
        {
            var response = await _client.GetAsync("/api/stats/user/total?userId=3");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod()]
        public async Task GetUserAverageTimeOnline_CorrectCount()
        {
            var expectedWeekly = 46800;
            var expectedDaily = 31200;
            var response = await _client.GetAsync("/api/stats/user/average?userId=5");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserAverageTime>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedWeekly, result.weeklyAverage);
            Assert.AreEqual(expectedDaily, result.dailyAverage);
        }

        [TestMethod()]
        public async Task GetUserAverageTimeOnline_NotFound()
        {
            var response = await _client.GetAsync("/api/stats/user/average?userId=3");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
        [TestMethod()]
        public async Task Forget_CorrectResponse()
        {
            var expected = "1";
            var response = await _client.GetAsync("/api/user/forget?userId=1");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserId>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(expected, result.userId);
            Assert.IsFalse(Data.Users.Any(x => x.userId == expected));
        }

        [TestMethod()]
        public async Task Forget_NotFound()
        {
            var response = await _client.GetAsync("/api/user/forget?userId=3");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
        [TestMethod()]
        public async Task CreateReport_Created()
        {
            var requestPayload = new ReportRequest
            {
                metrics = new List<string> { "dailyAverage", "total", "weeklyAverage" },
                users = new List<string> { "1", "2", "3", "4", "5" }
            };

            var x = JsonContent.Create(requestPayload);
            var response = await _client.PostAsync("/api/report/first", JsonContent.Create(requestPayload));

            response.EnsureSuccessStatusCode();

            var result = Data.ReportRequests.FirstOrDefault(x => x.name == "first");

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(requestPayload.metrics, result.metrics);
            CollectionAssert.AreEqual(requestPayload.users, result.users);
        }

        [TestMethod()]
        public async Task CreateReport_Failed()
        {
            var response = await _client.PostAsync("/api/report/name",
                new StringContent("", Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod()]
        public async Task GetReport_CorrectResponse()
        {
            Data.ReportRequests = new List<ReportRequest>()
            {
                new ReportRequest
                {
                    name = "first",
                    metrics = new List<string> { "dailyAverage", "weeklyAverage", "total", "min", "max" },
                    users = new List<string> { "4" }
                }
            };


            Data.Users = new List<User> {
            new User { userId = "4", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = new DateTime(2023, 10, 25, 12, 0, 0), end = new DateTime(2023, 10, 25, 14, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 16, 12, 0, 0), end = new DateTime(2023, 10, 16, 15, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 24, 12, 0, 0), end = new DateTime(2023, 10, 24, 16, 0, 0) }}},
            };
            var response = await _client.GetAsync("/api/report/first?from=2023-12-10-00:00&to=2023-26-10-00:00");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var fullReport = JsonConvert.DeserializeObject<Report>(content);
            Assert.IsNotNull(fullReport);
            Assert.IsNotNull(fullReport.userReports);
            var report = fullReport.userReports[0];
            Assert.IsNotNull(report);
            Assert.IsNotNull(report.metrics);

            var x = (JObject?)report.metrics[0];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.First);
            Assert.IsNotNull(x.First.First);
            Assert.AreEqual(10800, (int)x.First.First);
            Assert.AreEqual(10800, fullReport.dailyAverage);

            x = (JObject?)report.metrics[1];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.First);
            Assert.IsNotNull(x.First.First);
            Assert.AreEqual(16200, (int)x.First.First);
            Assert.AreEqual(16200, fullReport.weeklyAverage);

            x = (JObject?)report.metrics[2];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.First);
            Assert.IsNotNull(x.First.First);
            Assert.AreEqual(32400, (int)x.First.First);
            Assert.AreEqual(32400, fullReport.total);

            x = (JObject?)report.metrics[3];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.First);
            Assert.IsNotNull(x.First.First);
            Assert.AreEqual(7200, (int)x.First.First);
            Assert.AreEqual(7200, fullReport.min);

            x = (JObject?)report.metrics[4];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.First);
            Assert.IsNotNull(x.First.First);
            Assert.AreEqual(14400, (int)x.First.First);
            Assert.AreEqual(14400, fullReport.max);
        }

        [TestMethod]
        public async Task GetReport_NotFound()
        {
            var response = await _client.GetAsync("/api/report/notName?from=2023-12-10-00:00&to=2023-26-10-00:00");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetReportsList_CorrectResponse()
        {
            var sampleReport = new ReportRequest
            {
                name = "first",
                metrics = new List<string> { "dailyAverage", "weeklyAverage", "total", "min", "max" },
                users = new List<string> { "4" }
            };
            Data.ReportRequests = new List<ReportRequest>()
            {
                sampleReport
            };
            var response = await _client.GetAsync("/api/reports");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<ReportRequest>>(content);
            Assert.IsNotNull(result);
            var report = result[0];
            Assert.IsNotNull(report);
            Assert.AreEqual(sampleReport.name, report.name);

            Assert.IsNotNull(report.metrics);
            Assert.AreEqual(0, report.metrics.Except(sampleReport.metrics).Count());
            Assert.AreEqual(0, sampleReport.metrics.Except(report.metrics).Count());

            Assert.IsNotNull(report.users);
            Assert.AreEqual(0, report.users.Except(sampleReport.users).Count());
            Assert.AreEqual(0, sampleReport.users.Except(report.users).Count());
        }

        [TestMethod]
        public async Task GetUsersList_CorrectResponse()
        {
            Data.Users = new List<User> {
            new User { userId = "1", nickname = "admin", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = new DateTime(2023, 10, 25, 12, 0, 0), end = new DateTime(2023, 10, 25, 14, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 16, 12, 0, 0), end = new DateTime(2023, 10, 16, 15, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 24, 12, 0, 0), end = new DateTime(2023, 10, 24, 16, 0, 0) }}},
            };
            var response = await _client.GetAsync("/api/users/list");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<FirstSeen>>(content);
            Assert.IsNotNull(result);
            Assert.AreEqual("admin", result[0].username);
            Assert.AreEqual("1", result[0].userId);
            Assert.AreEqual(new DateTime(2023, 10, 16, 12, 0, 0), result[0].firstSeen);
        }



        [TestCleanup]
        public void Cleanup()
        {
            Data.ReportRequests = new List<ReportRequest>();
            Data.Users = new List<User>();
        }
    }
}