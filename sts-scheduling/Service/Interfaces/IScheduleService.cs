using System.Threading.Tasks;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;

namespace sts_scheduling.Service.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleResponse> ComputeSchedule(
            ScheduleRequest requests);
    }
}
