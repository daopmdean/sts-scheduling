using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;
using sts_scheduling.Service.Interfaces;

namespace sts_scheduling.Controllers
{
    [ApiController]
    [Route("api/scheduling")]
    public class SchedulingController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulingController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok("Hello");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost]
        public ActionResult<ScheduleResponse> ComputeSchedule(
            ScheduleRequest request)
        {
            try
            {
                return Ok(_scheduleService.ComputeSchedule(request));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("testing")]
        public ActionResult<ScheduleResponse> Testing()
        {
            try
            {
                var res = new ScheduleResponse
                {
                    Conflicts = 2,
                    Branches = 1,
                    WallTime = 10,
                    ShiftAssignments = Utils.Testing.GenerateShiftAssignments()
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 500,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("testing2")]
        public ActionResult<ScheduleResponse> Testing2(
            ScheduleRequest request)
        {
            return Ok("testing2");
        }
    }
}
