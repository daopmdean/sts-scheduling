using System;
namespace sts_scheduling.Data
{
    public class AvailableTime
    {
        public int Day { get; set; }
        public Session[] Sessions { get; set; }
    }

    class AvailablePool
    {
        public AvailableTime[] Availables { get; set; }//[day][timeIndex]

    }
}
