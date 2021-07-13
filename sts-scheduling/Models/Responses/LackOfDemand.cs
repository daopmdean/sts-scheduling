using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Models.Responses
{
    public class LackOfDemandDay
    {
        public int Day { set; get; }
        public List<LackOfDemandSkill> LackOfDemandSkills { get; set; }
    }

    public class LackOfDemandSkill  
    {
        public int SkillId { set; get; }
        public List<LackOfDemandDetail> LackOfDemandDetails { get; set; }
    }

    public class LackOfDemandDetail
    {
        public int Quantity { set; get; }
        public int Level { set; get; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
