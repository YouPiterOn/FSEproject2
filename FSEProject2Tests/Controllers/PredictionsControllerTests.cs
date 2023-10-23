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
    public class PredictionsControllerTests
    {
        private List<User> sampleData = new List<User>
        {
            new User {userId = "1", wasOnline = new List<DateTime>(){ DateTime.ParseExact("2023-10-10-12:00", "yyyy-dd-MM-HH:mm", null),
                                                                      DateTime.ParseExact("2023-17-10-12:00", "yyyy-dd-MM-HH:mm", null) } },
            new User {userId = "2", wasOnline = new List<DateTime>(){ DateTime.ParseExact("2023-10-10-12:00", "yyyy-dd-MM-HH:mm", null),
                                                                      DateTime.ParseExact("2023-11-10-12:00", "yyyy-dd-MM-HH:mm", null) }}
        };

        [TestMethod]
        public void PredictUsersOnline_CorrectPrediction()
        {
            var controller = new PredictionsController();
            Data.Users = sampleData;

            var result = controller.PredictUsersOnline("2023-24-10-12:00");
            var expected = 1;

            Assert.AreEqual(expected, result.Value.onlineUsers);
        }

        [TestMethod]
        public void PredictUserOnline_NotFound()
        {
            var controller = new PredictionsController();
            Data.Users = sampleData;

            var result = controller.PredictUserOnline("2023-24-10-12:00", 0.5, "unknownUserId");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PredictUserOnline_CorrectPredictionAboveTolerance()
        {
            var controller = new PredictionsController();
            Data.Users = sampleData;

            var result = controller.PredictUserOnline("2023-24-10-12:00", 0.5, "1");
            double expected = 1;

            Assert.IsTrue(result.Value.willBeOnline);
            Assert.AreEqual(expected, result.Value.onlineChance);
        }

        [TestMethod]
        public void PredictUserOnline_CorrectPredictionBelowTolerance()
        {
            var controller = new PredictionsController();
            Data.Users = sampleData;

            var result = controller.PredictUserOnline("2023-24-10-12:00", 1.1, "1");
            double expected = 1;

            Assert.IsFalse(result.Value.willBeOnline);
            Assert.AreEqual(expected, result.Value.onlineChance);
        }
    }
}