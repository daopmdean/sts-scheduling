using System;
namespace sts_scheduling.Data
{
    public class Staff
    {
        private int _id;
        private bool _isFulltime;

        public string Name { get; set; }
        public AvailableTime[] Availables { get; set; }//[day][timeIndex]
        public Skill[] Skills { get; set; }
        public int Id { get => _id; set => _id = value; }
        public bool IsFulltime { get => _isFulltime; set => _isFulltime = value; }
    }
}
