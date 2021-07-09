using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Models.Requests
{
    public class ConstraintDataRequest
    {
        public ConstraintSpecificRequest FulltimeConstraints { get; set; }
        public ConstraintSpecificRequest ParttimeConstraints { get; set; }

        public int MinDistanceBetweenSession { get; set; }
    }

    public class ConstraintSpecificRequest
    {
        public int MinDayOff { get; set; }
        public int MaxDayOff { get; set; }
        public float MinHoursPerWeek { get; set; }
        public float MaxHoursPerWeek { get; set; }
        public float MinShiftDuration { get; set; }
        public float MaxShiftDuration { get; set; }
        public float MinHoursPerDay { get; set; }
        public float MaxHoursPerDay { get; set; }
        public int MaxNormalHour { get; set; }
        public int MaxShiftPerDay { get; set; }
    }
}
