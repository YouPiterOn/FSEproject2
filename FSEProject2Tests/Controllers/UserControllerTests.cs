using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSEProject2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        private List<User> sampleData = new List<User>
        {
            new User { userId = "1" },
        };

        [TestMethod()]
        public void Forget_CorrectResponse()
        {
            var test = new UserController();
            Data.Users = sampleData;
            var expected = "1";

            var result = test.Forget("1");

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(expected, result.Value.userId);
            Assert.IsFalse(Data.Users.Any(x => x.userId == expected));
        }

        [TestMethod()]
        public void Forget_NotFound() 
        { 
            var test = new UserController();
            Data.Users = sampleData;

            var result = test.Forget("2");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}