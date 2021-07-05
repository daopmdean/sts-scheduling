using sts_scheduling.Utils;
using System.Collections.Generic;

namespace sts_scheduling.Models.Requests
{
    public class ScheduleRequest
    {
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<Staff> StaffList { get; set; }
        public IEnumerable<Demand> Demands { get; set; }
        public Contraint Contraint { get; set; }
    }

    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
    }

    public class Staff
    {
        public string Username { get; set; }
        public List<Skill> Skills { get; set; }
        public int TypeStaff { get; set; }
        public List<AvailableByDay> AvailableByDays { get; set; }
    }

    public class AvailableByDay
    {
        public int Day { get; set; }
        public List<AvailableTime> AvailableTimes { get; set; }
    }

    public class AvailableTime
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class Demand
    {
        public int Day { get; set; }
        public OperatingTime OperatingTime { get; set; }
        public IEnumerable<DemandBySkill> DemandBySkills { get; set; }
    }

    public class OperatingTime
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

    public class DemandBySkill
    {
        public int SkillId { get; set; }

        public IEnumerable<DemandData> DemandDatas { get; set; }
    }

    public class DemandData
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Level { get; set; }
        public int Quantity { get; set; }
    }

    public class Contraint
    {
        public int MinDistanceBetweenSession { get; set; }
        public ContraintDetail FullTime { get; set; }
        public ContraintDetail PartTime { get; set; }
    }

    public class ContraintDetail
    {
        public int MinDayOff { get; set; }

        public int MaxDayOff { get; set; }

        public float MinHoursPerWeek { get; set; }

        public float MaxHoursPerWeek { get; set; }

        public float MinHoursPerDay { get; set; }

        public float MaxHoursPerDay { get; set; }

        public float MinShiftDuration { get; set; }

        public float MaxShiftDuration { get; set; }

        public int MaxShiftPerDay { get; set; }
    }
}
