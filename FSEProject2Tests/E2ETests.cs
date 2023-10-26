/*using FSEProject2.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Runtime.InteropServices.JavaScript;

namespace FSEProject2.Tests
{
    [TestClass]
    public class E2ETests
    {
        private Task _hostTask;
        private IWebDriver _driver;

        [TestInitialize]
        public void Setup()
        {
            var builder = WebApplication.CreateBuilder(new string[] { });
            
            var app = builder.Build();
            app.MapControllers();
            _driver = new ChromeDriver();
            _hostTask = app.RunAsync("http://localhost:5000");
        }

        [TestMethod]
        public void Forget_CorrectResponse()
        {
            var myUrl = "http://localhost:5000";
            
            _driver.Navigate().GoToUrl(myUrl);
            string response = _driver.PageSource;
            Assert.IsNotNull(myUrl);
        }
    }
}
*/