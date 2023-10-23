﻿using FSEProject2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSEProject2.Controllers
{
    [Route("api/predictions")]
    [ApiController]
    public class PredictionsController : ControllerBase
    {
        public List<UserOnline> usersData = new List<UserOnline>();

        [HttpGet("users")]
        public ActionResult<PredictionData> PredictUsersOnline(string date)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);
            var response = Predictions.PredictUsersOnline(actualDate);

            if (response == null) { return NotFound(); }
            return response;
        }

        [HttpGet("user")]
        public ActionResult<UserPredictionData> PredictUserOnline(string date, double tolerance, string userId)
        {
            var actualDate = DateTime.ParseExact(date, "yyyy-dd-MM-HH:mm", null);
            var response = Predictions.PredictUserOnline(actualDate, tolerance, userId);

            if (response == null) { return NotFound(); }
            return response;
        }
    }
}
