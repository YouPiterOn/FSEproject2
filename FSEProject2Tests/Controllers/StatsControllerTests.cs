using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSEProject2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers.Tests
{
    [TestClass()]
    public class StatsControllerTests
    {
        private List<UserOnline> sampleData = new List<UserOnline>
        {
            new UserOnline { user = new UsersData { userId = "1" }, date = DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null) },
            new UserOnline { user = new UsersData { userId = "2" }, date = DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null) },
        };

        [TestMethod()]
        public void GetUsersOnline_CorrectCount()
        {
            var test = new StatsController() { usersData = sampleData };
            var expected = 2;

            var result = test.GetUsersOnline("2023-01-01-12:00");

            Assert.AreEqual(expected, result.Value.usersOnline);
        }
        [TestMethod()]
        public void GetUsersOnline_Null()
        {
            var test = new StatsController() { usersData = sampleData };

            var result = test.GetUsersOnline("2023-01-01-13:00");

            Assert.IsNull(result.Value.usersOnline);
        }

        [TestMethod()]
        public void GetUserStats_NotFound()
        {
            var test = new StatsController() { usersData = sampleData };

            var result = test.GetUserStats("2023-01-01-12:00", "3");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod()]
        public void GetUserStats_WasOnline()
        {
            var test = new StatsController() { usersData = sampleData };
            var result = test.GetUserStats("2023-01-01-12:00", "1");

            Assert.IsTrue(result.Value.wasUserOnline.Value);
            Assert.IsNull(result.Value.nearestOnlineTime);
        }

        [TestMethod()]
        public void GetUserStats_WasOffline()
        {
            var test = new StatsController() { usersData = sampleData };
            var result = test.GetUserStats("2023-01-01-13:00", "1");
            var expected = DateTime.ParseExact("2023-01-01-12:00", "yyyy-dd-MM-HH:mm", null);

            Assert.IsFalse(result.Value.wasUserOnline.Value);
            Assert.AreEqual(expected, result.Value.nearestOnlineTime);
        }
    }
}