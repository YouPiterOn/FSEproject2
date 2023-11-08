using FSEProject2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices.JavaScript;

namespace FSEProject2.Tests
{
    [TestClass]
    public class E2ETests
    {
        public static async Task<string> FetchResponse(string apiUrl)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(apiUrl);
            return response;
        }

        [TestMethod]
        public async Task PredictUsersOnline_Null()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/predictions/users?date=2023-24-10-12:00";

            string response = await FetchResponse(apiUrl);

            var predictionData = JsonConvert.DeserializeObject<PredictionData>(response);

            Assert.IsNotNull(predictionData);
            Assert.AreEqual(null, predictionData.onlineUsers);
        }

        [TestMethod]
        public async Task GetUsersOnline_Null()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/stats/users?date=2023-10-10-12:00";

            string response = await FetchResponse(apiUrl);

            var historicalData = JsonConvert.DeserializeObject<HistoricalData>(response);

            Assert.IsNotNull(historicalData);
            Assert.AreEqual(null, historicalData.usersOnline);
        }

        [TestMethod]
        public async Task GetUserStats_NotFound()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/stats/user?date=2023-10-10-12:00&userId=3";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetUserTimeData_NotFound()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/stats/user/total?userId=3";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetUserAverageTimeOnline_NotFound()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/stats/user/average?userId=3";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task Forget_NotFound()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/user/forget?userId=3";
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task GetReportsList_CorrectResponse()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/reports";

            string response = await FetchResponse(apiUrl);
            var reports = JsonConvert.DeserializeObject<object>(response);
            Assert.IsNotNull(reports);
            Assert.AreEqual("[]", reports.ToString());
        }

        [TestMethod]
        public async Task GetUsersList_CorrectResponse()
        {
            var apiUrl = "https://fseproject.azurewebsites.net/api/users/list";

            string response = await FetchResponse(apiUrl);
            var usersData = JsonConvert.DeserializeObject<object>(response);
            Assert.IsNotNull(usersData);
            Assert.AreEqual("[]", usersData.ToString());
        }
    }
}