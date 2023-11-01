using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSEProject2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using FSEProject2.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FSEProject2.Controllers.Tests
{
    [TestClass()]
    public class ReportControllerTests
    {
        private List<User> sampleData = new List<User>
        {
            new User { userId = "4", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = new DateTime(2023, 10, 25, 12, 0, 0), end = new DateTime(2023, 10, 25, 14, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 16, 12, 0, 0), end = new DateTime(2023, 10, 16, 15, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 24, 12, 0, 0), end = new DateTime(2023, 10, 24, 16, 0, 0) }}},
            new User { userId = "5", periodsOnline = new List<PeriodOnline>()}
        };
        private Dictionary<string, ReportRequest> sampleRequests = new Dictionary<string, ReportRequest>
        {
            ["name"] = new ReportRequest()
            {
                metrics = new List<string>() { "dailyAverage", "weeklyAverage", "total", "min", "max" },
                users = new List<string>() { "4" }
            }
        };
        [TestMethod()]
        public void CreateReport_Created()
        {
            var test = new ReportController();
            var json = "{\"metrics\": [\"dailyAverage\", \"total\", \"weeklyAverage\"], \"users\": [\"1\", \"2\", \"3\", \"4\", \"5\"]}";
            var requestPayload = new ReportRequest
            {
                metrics = new List<string> { "dailyAverage", "total", "weeklyAverage" },
                users = new List<string> { "1", "2", "3", "4", "5" }
            };
            var name = "name";
            var response = test.CreateReport(name, requestPayload);

            var expected = new ReportRequest() { metrics = new List<string>() { "dailyAverage", "total", "weeklyAverage" }, users = new List<string>() { "1", "2", "3", "4", "5" } };
            var report = Data.ReportRequests[name];

            Assert.IsNotNull(report);
            Assert.IsInstanceOfType(response, typeof(Object));
            Assert.AreEqual(0, report.metrics.Except(expected.metrics).Count());
            Assert.AreEqual(0, report.users.Except(expected.users).Count());
            Assert.AreEqual(0, expected.metrics.Except(report.metrics).Count());
            Assert.AreEqual(0, expected.users.Except(report.users).Count());
        }
        [TestMethod()]
        public void CreateReport_Failed()
        {
            var test = new ReportController();
            var requestPayload = new ReportRequest{};
            var name = "name";
            var response = test.CreateReport(name, requestPayload);

            Assert.IsInstanceOfType(response.Result, typeof(BadRequestResult));
        }

        [TestMethod()]
        public void GetReport_CorrectResponse()
        {
            var test = new ReportController();
            Data.Users = sampleData;
            Data.ReportRequests = sampleRequests;

            var response = test.GetReports("name", "2023-12-10-00:00", "2023-26-10-00:00");
            Assert.IsNotNull(response.Value);
            var report = response.Value[0];
            Assert.IsNotNull(report);
            Assert.AreEqual("4", report.userId);
            var x = report.metrics[0];
            Assert.AreEqual(10800, x.GetType().GetProperty("dailyAverage").GetValue(x, null));

            x = report.metrics[1];
            Assert.AreEqual(16200, x.GetType().GetProperty("weeklyAverage").GetValue(x, null));

            x = report.metrics[2];
            Assert.AreEqual(32400, x.GetType().GetProperty("total").GetValue(x, null));

            x = report.metrics[3];
            Assert.AreEqual(7200, x.GetType().GetProperty("min").GetValue(x, null));

            x = report.metrics[4];
            Assert.AreEqual(14400, x.GetType().GetProperty("max").GetValue(x, null));
        }
        [TestMethod]
        public void GetReport_NotFound()
        {
            var test = new ReportController();
            Data.Users = sampleData;
            Data.ReportRequests = sampleRequests;

            var response = test.GetReports("notName", "2023-12-10-00:00", "2023-26-10-00:00");
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }
        [TestCleanup]
        public void Cleanup() 
        {
            Data.ReportRequests = new Dictionary<string, ReportRequest>();
            Data.Users = new List<User>();
        }
    }
}