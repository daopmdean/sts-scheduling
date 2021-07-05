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
            SchedulingHandle handle = new()
            {
                DataInput = requests.DataInput,
                ConstraintData = requests.ConstraintData
            };
            ScheduleResponse response = new();

            handle.Solve(response, 120);

            return response;
        }
    }
}
