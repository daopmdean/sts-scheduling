using System;
namespace sts_scheduling.Data
{
    public class AvailableDay
    {
        public int Day { get; set; }
        public Session[] AvailableTimes { get; set; }
    }

}
