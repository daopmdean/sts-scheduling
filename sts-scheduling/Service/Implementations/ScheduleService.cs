using System;
using System.Threading.Tasks;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;
using sts_scheduling.Service.Interfaces;
using sts_scheduling.Utils;

namespace sts_scheduling.Service.Implementations
{
    public class ScheduleService : IScheduleService
    {
        public ScheduleService()
        {
        }

        public async Task<ScheduleResponse> ComputeSchedule(
            ScheduleRequest requests)
        {
            SchedulingHandle s = new();

            var scheduleResponse = new ScheduleResponse();
            scheduleResponse.Conflicts = 1;
            return scheduleResponse;
        }
    }
}
