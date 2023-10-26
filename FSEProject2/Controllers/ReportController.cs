using FSEProject2.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSEProject2.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        [HttpPost("{name}")]
        public ActionResult<object> CreateReport(string name, [FromBody] ReportRequest request)
        {
            if (request == null) { return BadRequest(); }

            var response = Reports.CreateReport(name, request);

            if (response == null) { return BadRequest(); }
            return response;
        }
        [HttpGet("{name}")]
        public ActionResult<List<Report>> GetReports(string name, string from, string to) 
        {
            var actualFrom = DateTime.ParseExact(from, "yyyy-dd-MM-HH:mm", null);
            var actualTo = DateTime.ParseExact(to, "yyyy-dd-MM-HH:mm", null);

            var response = Reports.GetReport(name, actualFrom, actualTo);

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
