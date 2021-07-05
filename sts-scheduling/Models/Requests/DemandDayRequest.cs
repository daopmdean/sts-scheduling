using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Models.Requests
{
    public class DemandDayRequest
    {
        public int Day { set; get; }
        public DemandSkillRequest[] DemandBySkills { get; set; }
    }   

    public class DemandSkillRequest
    {
        public int SkillId { set; get; }
        public DemandRequest[] Demands { get; set; }
    }

    public class DemandRequest
    {
        public int Quantity { set; get; }
        public int Level { set; get; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
