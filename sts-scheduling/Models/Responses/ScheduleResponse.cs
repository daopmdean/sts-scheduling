using System.Collections.Generic;
using Google.OrTools.Sat;

namespace sts_scheduling.Models.Responses
{
    public class ScheduleResponse
    {
        public CpSolverStatus Status { get; set; }

        public long Conflicts { get; set; }

        public long Branches { get; set; }

        public double WallTime { get; set; }

        public IEnumerable<ShiftAssignment> ShiftAssignments { get; set; }
    }
}
