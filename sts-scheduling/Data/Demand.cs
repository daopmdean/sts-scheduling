using System;
namespace sts_scheduling.Data
{
    public class DemandDay
    {
        public int Day { set; get; }
        public DemandSkill[] DemandBySkills { get; set; }
    }

    public class DemandSkill
    {
        public int SkillId { set; get; }
        public Demand[] Demands { get; set; }
    }

    public class Demand
    {
        public int Quantity { set; get; }
        public int Level { set; get; }
        public Session Session { set; get; }
    }
}
