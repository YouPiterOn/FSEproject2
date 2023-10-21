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

        }

        [TestMethod()]
        public async Task PredictUsersOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/predictions/users?date=2023-24-10-12:00");
            Console.WriteLine(response.Content);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PredictionData>(content);

            Assert.IsNotNull(result);
            Assert.IsNull(result.onlineUsers);
        }
        
        [TestMethod]
        public async Task PredictUserOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/predictions/user?date=2023-24-10-12:00&tolerance=0.85&userId=1");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task GetUsersOnline_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/stats/users?date=2023-24-10-12:00");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HistoricalData>(content);

            Assert.IsNotNull(result);
            Assert.IsNull(result.usersOnline);
        }

        [TestMethod]
        public async Task GetUserStats_CorrectResponse()
        {
            var response = await _client.GetAsync("/api/stats/user?date=2023-24-10-12:00&userId=1");

            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }
    }
}