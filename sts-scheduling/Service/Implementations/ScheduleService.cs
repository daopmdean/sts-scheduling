using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sts_scheduling.Data;
using sts_scheduling.Enums;
using sts_scheduling.Models.Requests;
using sts_scheduling.Models.Responses;
using sts_scheduling.Service.Interfaces;
using sts_scheduling.Utils;

namespace sts_scheduling.Service.Implementations
{
    public class ScheduleService : IScheduleService
    {
        public ScheduleService()
        {
        }

        public async Task<ScheduleResponse> ComputeSchedule(
            ScheduleRequest requests)
        {
            int NumTimeFrames = 48;
            //Get data

            //Skill 
            List<Skill> skills = requests.Skills;

            //GET STAFF
            List<Staff> PtStaffs = new List<Staff>();
            List<Staff> FtStaffs = new List<Staff>();

            List<StaffRequestData> staffRequests = requests.Staffs;
            
            for (int i = 0; i < staffRequests.Count; i++)
            {

                //Id To Map
                int Id = staffRequests[i].TypeStaff == 1 ? PtStaffs.Count : FtStaffs.Count;

                Staff staff = new()
                {
                    Id = Id,
                    Username = staffRequests[i].Username,
                    Skills = staffRequests[i].Skills.ToArray(),
                    //convert AvailableTime
                    Availables = (AvailableDay[])ConvertData.ConvertFromRequest(staffRequests[i].AvalailableDays, NumTimeFrames),
                };

                //Phân loại nhân viên
                //PARTTIME
                if (staffRequests[i].TypeStaff == 1)
                {
                    PtStaffs.Add(staff);
                }
                //FULLLTIME
                else
                {
                    FtStaffs.Add(staff);
                }
            }

            //convert Demand
            DemandDay[] demands = (DemandDay[])ConvertData.ConvertFromRequest(requests.Demands, NumTimeFrames);


            SchedulingHandle handle = new()
            {
                DataInput = new DataInput()
                {
                    Demand = demands,
                    NumDay = 7,
                    NumTimeFrame = NumTimeFrames,
                    Skills = skills,
                    StaffDic = new Dictionary<TypeStaff, List<Staff>>()
                    {
                        [TypeStaff.FULL_TIME] = FtStaffs,
                        [TypeStaff.PART_TIME] = PtStaffs
                    }
                },
                ConstraintData = ConvertData.ConvertFromRequest(requests.Constraints)

            };

            ScheduleResponse response = new();

            handle.Solve(ref response, 120);

            return response;
        }
    }
}
