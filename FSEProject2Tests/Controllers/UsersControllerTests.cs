using FSEProject2;
using FSEProject2.Controllers;
using FSEProject2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSEProject2.Controllers.Tests
{
    [TestClass()]
    public class UsersControllerTests
    {
        private List<User> sampleData = new List<User>()
        {
            new User { userId = "1", nickname = "admin", periodsOnline = new List<PeriodOnline>{
                    new PeriodOnline { start = new DateTime(2023, 10, 25, 12, 0, 0), end = new DateTime(2023, 10, 25, 14, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 16, 12, 0, 0), end = new DateTime(2023, 10, 16, 15, 0, 0) },
                    new PeriodOnline { start = new DateTime(2023, 10, 24, 12, 0, 0), end = new DateTime(2023, 10, 24, 16, 0, 0) }}}
        };

        [TestMethod()]
        public void GetUsersList_CorrectResponse ()
        {
            var test = new UsersController();
            Data.Users = sampleData;
            var result = test.GetUsersList();
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("admin", result.Value[0].username);
            Assert.AreEqual("1", result.Value[0].userId);
            Assert.AreEqual(new DateTime(2023, 10, 16, 12, 0, 0), result.Value[0].firstSeen);
        }
    }
}
