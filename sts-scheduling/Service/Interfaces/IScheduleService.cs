using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;

namespace sts_scheduling.Service.Interfaces
{
    public interface IScheduleService
    {
        ScheduleResponse ComputeSchedule(
            ScheduleRequest requests);
    }
}
