using sts_scheduling.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Models.Requests
{
    public class StaffRequestData
    {
        public string Username { get; set; }
        public List<SkillStaff> Skills { get; set; }
        public int TypeStaff { get; set; }
        public List<AvalailableDayRequest> AvalailableDays { get; set; }
    }


    public class AvalailableDayRequest
    {
        public int Day { get; set; }
        public List<AvailableTimeRequest> AvailableTimes { get; set; }

    }

    public class AvailableTimeRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
