using System;
using System.Collections.Generic;
using sts_scheduling.Data;

namespace sts_scheduling.Models.Requests
{
    public class ScheduleRequest
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public List<StaffRequestData> Staffs { get; set; }

        public List<Skill> Skills { get; set; }

        public List<DemandDayRequest> Demands { get; set; }

        public ConstraintDataRequest Constraints { get; set; }

        public DateTime DateStart { get; set; }
    }
}
