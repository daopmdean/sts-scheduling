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
        public ActionResult Hello()
        {
            return Ok("Hello");
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleResponse>> ComputeSchedule(
            ScheduleRequest request)
        {
            try
            {
                return Ok(await _scheduleService.ComputeSchedule(request));


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
                    //ShiftAssignments = Utils.Testing.GenerateShiftAssignments()
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
    }
}
