namespace sts_scheduling.Utils
{
    public class ConstraintData
    {
        public int MinDayOff { get; set; }
        public int MaxDayOff { get; set; }
        public int MinFTWorkingTimeOnWeek { get; set; }
        public int MaxFTWorkingTimeOnWeek { get; set; }
        public int MinPTWorkOnWeek { get; set; }
        public int MaxPTWorkOnWeek { get; set; }
        public int MinFTSessionDuration { get; set; }
        public int MinPTSessionDuration { get; set; }
        public int MaxFTSessionDuration { get; set; }
        public int MaxPTSessionDuration { get; set; }
        public int MaxFTWorkingTimeInDay { get; set; }
        public int MaxPTWorkingTimeInDay { get; set; }
        public int MaxNormalHour { get; set; }
        public int TimeStart { get; set; }
        public int TimeEnd { get; set; }
        public int MaxShiftInDay { get; set; }
        public int MinDistanceBetweenSession { get; set; }
    }
}
