using sts_scheduling.Data;
using sts_scheduling.Utils;
using System.Collections.Generic;

namespace sts_scheduling.Models.Requests
{
    public class ScheduleRequest
    {
        public List<StaffRequestData> Staffs { get; set; }

        public List<Skill> Skills { get; set; }

        public List<DemandDayRequest> Demands { get; set; }

        public ConstraintDataRequest Constraints { get; set; }
    }
}
