using System;
using System.Collections.Generic;
using sts_scheduling.Models.Responses;

namespace sts_scheduling.Utils
{
    public class Testing
    {
        public Testing()
        {
        }
        public static List<ShiftAssignment> GenerateShiftAssignments()
        {
            List<ShiftAssignment> list = new();

            list.Add(new ShiftAssignment
            {
                Username = "mystaff02",
                SkillId = 18,
                TimeStart = new DateTime(2021, 07, 12, 07, 00, 00),
                TimeEnd = new DateTime(2021, 07, 12, 15, 0, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff01",
                SkillId = 21,
                TimeStart = new DateTime(2021, 07, 12, 9, 30, 0),
                TimeEnd = new DateTime(2021, 07, 12, 17, 30, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff01",
                SkillId = 21,
                TimeStart = new DateTime(2021, 07, 13, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 13, 13, 0, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff03",
                SkillId = 20,
                TimeStart = new DateTime(2021, 07, 14, 12, 0, 0),
                TimeEnd = new DateTime(2021, 07, 14, 18, 0, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff04",
                SkillId = 20,
                TimeStart = new DateTime(2021, 07, 14, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 14, 12, 30, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff04",
                SkillId = 18,
                TimeStart = new DateTime(2021, 07, 15, 12, 30, 0),
                TimeEnd = new DateTime(2021, 07, 15, 18, 00, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff03",
                SkillId = 20,
                TimeStart = new DateTime(2021, 07, 15, 12, 30, 0),
                TimeEnd = new DateTime(2021, 07, 15, 18, 0, 0)
            });
            list.Add(new ShiftAssignment
            {
                Username = "mystaff03",
                SkillId = 20,
                TimeStart = new DateTime(2021, 07, 16, 12, 30, 0),
                TimeEnd = new DateTime(2021, 07, 16, 18, 0, 0)
            });

            return list;
        }
    }
}
