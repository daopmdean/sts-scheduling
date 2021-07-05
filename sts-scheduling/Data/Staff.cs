using System;
namespace sts_scheduling.Data
{
    public class Staff
    {
        private int _id;

        public string Username { get; set; }
        public AvailableDay[] Availables { get; set; }//[day][timeIndex]
        public SkillStaff[] Skills { get; set; }
        public int Id { get => _id; set => _id = value; 
        }

    }
}
