using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;
using sts_scheduling.Service.Interfaces;

namespace sts_scheduling.Service.Implementations
{
    public class ScheduleService : IScheduleService
    {
        public ScheduleService()
        {
        }

        public Task<IEnumerator<ShiftAssignment>> ComputeSchedule(
            SchedulingRequests requests)
        {
            throw new NotImplementedException();
        }
    }
}
