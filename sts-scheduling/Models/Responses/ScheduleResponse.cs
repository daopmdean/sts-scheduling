using System.Collections.Generic;
using Google.OrTools.Sat;

namespace sts_scheduling.Models.Responses
{
    public class ScheduleResponse
    {
        public int ShiftScheduleResultId { get; set; }
        public int StoreId { get; set; }
        public CpSolverStatus Status { get; set; }

        public long Conflicts { get; set; }

        public long Branches { get; set; }

        public double WallTime { get; set; }

        public IEnumerable<ShiftAssignment> ShiftAssignments { get; set; }
        public IEnumerable<LackOfDemandDay> LackOfDemandDays { get; set; }
    }
}
