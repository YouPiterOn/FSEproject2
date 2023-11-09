using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSEProject2.Controllers
{
    [Route("api")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpPost("report/{name}")]
        public ActionResult<object> CreateReport(string name, [FromBody] ReportRequest request)
        {
            if (request == null) { return BadRequest(); }

            var response = Reports.CreateReport(name, request);

            if (response == null) { return BadRequest(); }
            return response;
        }
        [HttpGet("report/{name}")]
        public ActionResult<Report> GetReport(string name, string from, string to)
        {
            var actualFrom = DateTime.ParseExact(from, "yyyy-dd-MM-HH:mm", CultureInfo.InvariantCulture);
            var actualTo = DateTime.ParseExact(to, "yyyy-dd-MM-HH:mm", CultureInfo.InvariantCulture);

            var response = Reports.GetReport(name, actualFrom, actualTo);

            if (response == null) { return NotFound(); }
            return response;
        }
        [HttpGet("reports")]
        public ActionResult<List<ReportRequest>> GetReportsList()
        {
            return Data.ReportRequests;
        }
    }
}
