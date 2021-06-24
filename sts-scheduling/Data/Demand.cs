using System;
namespace sts_scheduling.Data
{
    public class Demand
    {
        public int Quantity { set; get; }
        public Session Session { set; get; }
    }

    public class DemandByDay
    {
        public int Day { set; get; }
        public DemandBySkill[] DemandBySkills { get; set; }
    }

    public class DemandBySkill
    {
        public Skill Skill { set; get; }
        public Demand[] Demands { get; set; }
    }
}
