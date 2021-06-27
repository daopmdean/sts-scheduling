using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;

namespace sts_scheduling.Service.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerator<ShiftAssignment>> ComputeSchedule(
            SchedulingRequests requests);
    }
}
