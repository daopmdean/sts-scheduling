namespace sts_scheduling.Utils
{
    public class ConstraintData
    {
        public ConstraintSpecific FulltimeConstraints { get; set; }
        public ConstraintSpecific ParttimeConstraints { get; set; }

        public int MinDistanceBetweenSession { get; set; }
    }

    public class ConstraintSpecific
    {
        public int MinDayOff { get; set; }
        public int MaxDayOff { get; set; }

        public int MinWorkingTimeOnWeek { get; set; }
        public int MaxWorkingTimeOnWeek { get; set; }

        public int MinSessionDuration { get; set; }
        public int MaxSessionDuration { get; set; }

        public int MinWorkingTimeInDay { get; set; }
        public int MaxWorkingTimeInDay { get; set; }
        //unuse
        public int MaxNormalHour { get; set; }

        public int MaxShiftInDay { get; set; }
    }


}
