using System.Collections.Generic;

namespace sts_scheduling.Models.Responses
{
    public class ScheduleResponse
    {
        public long Conflicts { get; set; }

        public long Branches { get; set; }

        public double WallTime { get; set; }

        public IEnumerable<ShiftAssignment> ShiftAssignments { get; set; }
    }
}
