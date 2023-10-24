using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSEProject2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FSEProject2.Models;

namespace FSEProject2.Controllers.Tests
{
    [TestClass()]
    public class StatsControllerTests
    {
        private List<User> sampleData = new List<User>
        {
            new User { userId = "1", wasOnline = new List<DateTime>(){ DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null) } },
            new User { userId = "2", wasOnline = new List<DateTime>(){ DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null) } },
            new User { userId = "4", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = DateTime.Now.AddHours(-1), end = DateTime.Now },
                    new PeriodOnline { start = DateTime.Now.AddHours(-3), end = DateTime.Now.AddHours(-2) }}},
            new User { userId = "5", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = DateTime.Now.AddHours(-1), end = DateTime.Now },
                    new PeriodOnline { start = DateTime.Now.AddHours(-25), end = DateTime.Now.AddHours(-24) },
                    new PeriodOnline { start = DateTime.Now.AddDays(-8), end = DateTime.Now.AddDays(-7) }}}
        };

        [TestMethod()]
        public void GetUsersOnline_CorrectCount()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var expected = 2;

            var result = test.GetUsersOnline("2023-01-01-12:00");

            Assert.AreEqual(expected, result.Value.usersOnline);
        }
        [TestMethod()]
        public void GetUsersOnline_Null()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUsersOnline("2023-01-01-13:00");

            Assert.IsNull(result.Value.usersOnline);
        }

        [TestMethod()]
        public void GetUserStats_NotFound()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserStats("2023-01-01-12:00", "3");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void GetUserStats_WasOnline()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserStats("2023-01-01-12:00", "1");

            Assert.IsTrue(result.Value.wasUserOnline.Value);
            Assert.IsNull(result.Value.nearestOnlineTime);
        }

        [TestMethod()]
        public void GetUserStats_WasOffline()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserStats("2023-01-01-13:00", "1");
            var expected = DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null);

            Assert.IsFalse(result.Value.wasUserOnline.Value);
            Assert.AreEqual(expected, result.Value.nearestOnlineTime);
        }

        [TestMethod()]
        public void GetUserTimeData_CorrectCount()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserTimeData("4");
            var expected = 7200;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(expected, result.Value.totalTime);
        }

        [TestMethod()]
        public void GetUserTimeData_Null()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserTimeData("3");

           Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void GetUserAverageTimeOnline_CorrectCount()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserAverageTimeOnline("5");
            var expectedWeekly = 46800;
            var expectedDaily = 31200;

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(expectedWeekly, result.Value.weeklyAverage);
            Assert.AreEqual(expectedDaily, result.Value.dailyAverage);
        }

        [TestMethod()]
        public void GetUserAverageTimeOnline_Null()
        {
            var test = new StatsController();
            Data.Users = sampleData;

            var result = test.GetUserAverageTimeOnline("3");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}