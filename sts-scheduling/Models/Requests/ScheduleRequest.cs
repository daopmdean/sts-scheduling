using sts_scheduling.Utils;

namespace sts_scheduling.Models.Requests
{
    public class ScheduleRequest
    {
        public DataInput DataInput { get; set; }

        public ConstraintData ConstraintData { get; set; }
    }
}
