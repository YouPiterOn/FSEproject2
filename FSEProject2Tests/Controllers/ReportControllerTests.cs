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
        private readonly List<User> sampleData = new List<User>
        {
            new User { userId = "4", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = new DateTime(2023, 10, 25, 12, 0, 0), end = new DateTime(2023, 10, 25, 14, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 16, 12, 0, 0), end = new DateTime(2023, 10, 16, 15, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 24, 12, 0, 0), end = new DateTime(2023, 10, 24, 16, 0, 0) }}},
            new User { userId = "5", periodsOnline = new List<PeriodOnline>()}
        };
        private readonly List<ReportRequest> sampleRequests = new List<ReportRequest>
        {
            new ReportRequest()
            {
                name = "name",
                metrics = new List<string>() { "dailyAverage", "weeklyAverage", "total", "min", "max" },
                users = new List<string>() { "4" }
            }
        };
        [TestMethod()]
        public void CreateReport_Created()
        {
            var test = new ReportController();
            var requestPayload = new ReportRequest
            {
                metrics = new List<string> { "dailyAverage", "total", "weeklyAverage" },
                users = new List<string> { "1", "2", "3", "4", "5" }
            };
            var name = "name";
            var response = test.CreateReport(name, requestPayload);

            var expected = new ReportRequest() { metrics = new List<string>() { "dailyAverage", "total", "weeklyAverage" }, users = new List<string>() { "1", "2", "3", "4", "5" } };
            var report = Data.ReportRequests.FirstOrDefault(x => x.name == name);

            Assert.IsNotNull(report);
            Assert.IsNotNull(report.metrics);
            Assert.IsNotNull(report.users);
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
            var requestPayload = new ReportRequest { };
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

            var response = test.GetReport("name", "2023-12-10-00:00", "2023-26-10-00:00");
            Assert.IsNotNull(response.Value);
            Assert.IsNotNull(response.Value.userReports);
            var report = response.Value.userReports[0];
            Assert.IsNotNull(report);
            Assert.IsNotNull(report.metrics);
            Assert.AreEqual("4", report.userId);

            var x = report.metrics[0];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.GetType());
            var property = x.GetType().GetProperty("dailyAverage");
            Assert.IsNotNull(property);
            Assert.AreEqual(10800, property.GetValue(x, null));

            x = report.metrics[1];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.GetType());
            property = x.GetType().GetProperty("weeklyAverage");
            Assert.IsNotNull(property);
            Assert.AreEqual(16200, property.GetValue(x, null));

            x = report.metrics[2];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.GetType());
            property = x.GetType().GetProperty("total");
            Assert.IsNotNull(property);
            Assert.AreEqual(32400, property.GetValue(x, null));

            x = report.metrics[3];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.GetType());
            property = x.GetType().GetProperty("min");
            Assert.IsNotNull(property);
            Assert.AreEqual(7200, property.GetValue(x, null));

            x = report.metrics[4];
            Assert.IsNotNull(x);
            Assert.IsNotNull(x.GetType());
            property = x.GetType().GetProperty("max");
            Assert.IsNotNull(property);
            Assert.AreEqual(14400, property.GetValue(x, null));
        }
        [TestMethod]
        public void GetReport_NotFound()
        {
            var test = new ReportController();
            Data.Users = sampleData;
            Data.ReportRequests = sampleRequests;

            var response = test.GetReport("notName", "2023-12-10-00:00", "2023-26-10-00:00");
            Assert.IsInstanceOfType(response.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public void GetReportsList_CorrectResponse()
        {
            var test = new ReportController();
            Data.Users = sampleData;
            Data.ReportRequests = sampleRequests;

            var response = test.GetReportsList();
            Assert.AreEqual(sampleRequests, response.Value);
        }
        [TestCleanup]
        public void Cleanup()
        {
            Data.ReportRequests = new List<ReportRequest>();
            Data.Users = new List<User>();
        }
    }
}