﻿using FSEProject2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSEProject2.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

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
    }
}