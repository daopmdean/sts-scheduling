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

            var a = new ShiftAssignment
            {
                Username = "effstaff1",
                SkillId = 3,
                TimeStart = new DateTime(2021, 07, 05, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 05, 12, 0, 0)
            };
            var b = new ShiftAssignment
            {
                Username = "effstaff2",
                SkillId = 4,
                TimeStart = new DateTime(2021, 07, 05, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 05, 12, 0, 0)
            };
            var c = new ShiftAssignment
            {
                Username = "effstaff3",
                SkillId = 2,
                TimeStart = new DateTime(2021, 07, 05, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 05, 12, 0, 0)
            };
            var d = new ShiftAssignment
            {
                Username = "effstaff4",
                SkillId = 1,
                TimeStart = new DateTime(2021, 07, 05, 7, 0, 0),
                TimeEnd = new DateTime(2021, 07, 05, 12, 0, 0)
            };

            list.AddRange(new ShiftAssignment[] { a, b, c, d });

            return list;
        }
    }
}
