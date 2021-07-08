using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.OrTools.Sat;
using sts_scheduling.Data;
using sts_scheduling.Enums;
using sts_scheduling.Models.Responses;

namespace sts_scheduling.Utils
{
    public class SchedulingHandle
    {
        public DataInput DataInput { get; set; }
        public ConstraintData ConstraintData { get; set; }
        public DateTime DateStart = new(2021, 7, 5);
        private const int UNASSIGNED = -1;

        public void Solve(ref ScheduleResponse response, int timeLimit)
        {
            //INIT

            //date - skill - time
            var demands = DataInput.GetDemandMatrix();
            int[,] skillFTStaffs = DataInput.GetSkillMatrixOf(TypeStaff.FULL_TIME);
            int[,] skillPTStaffs = DataInput.GetSkillMatrixOf(TypeStaff.PART_TIME);
            int[,,] availableFT = DataInput.GetAvailableMatrixOf(TypeStaff.FULL_TIME);
            int[,,] availablePT = DataInput.GetAvailableMatrixOf(TypeStaff.PART_TIME);
            int numFTStaffs = DataInput.GetNumStaff(TypeStaff.FULL_TIME);
            int numPTStaffs = DataInput.GetNumStaff(TypeStaff.PART_TIME);
            int totalStaff = DataInput.GetNumStaff(TypeStaff.All);
            int numPosition = DataInput.GetNumSkill();
            int numDays = DataInput.NumDay;
            int numTimeFrames = DataInput.NumTimeFrame;

            //Fix Assignment

            var objIntVars = new List<IntVar>();
            var objIntCoeffs = new List<int>();

            var model = new CpModel();
            IntVar[,,,] work_ft = NewBoolVars(
                model, "workFT", numFTStaffs, numDays, numPosition, numTimeFrames);
            int[,,,] sch_ft = new int[numFTStaffs, numPosition, numDays, numTimeFrames];

            IntVar[,,,] work_pt = NewBoolVars(
                model, "workPT", numPTStaffs, numDays, numPosition, numTimeFrames);
            int[,,,] sch_pt = new int[numPTStaffs, numPosition, numDays, numTimeFrames];

            //--TODO Assign Fix Assignment

            //Nhân viên làm việc trong khoảng thời gian rãnh
            AddWokingInAvailableTimeConstrain(
                model, work_ft, availableFT, numFTStaffs,
                numPosition, numDays, numTimeFrames);

            //Nhân viên làm việc trong khoảng thời gian rãnh
            AddWokingInAvailableTimeConstrain(
                model, work_pt, availablePT, numPTStaffs,
                numPosition, numDays, numTimeFrames);

            //Tổng thời gian làm việc 1 tuần của nhân viên Fulltime, PartTime làm việc luôn nằm trong khoảng (min, max)
            foreach (int s in Range(numFTStaffs))
            {
                AddLimitWokingTimeByWeekConstraint(
                    model, work_ft, s, numPosition, numDays,
                    numTimeFrames, ConstraintData.FulltimeConstraints.MinWorkingTimeOnWeek,
                    ConstraintData.FulltimeConstraints.MaxWorkingTimeOnWeek);
            }

            foreach (int s in Range(numPTStaffs))
            {
                AddLimitWokingTimeByWeekConstraint(
                    model, work_pt, s, numPosition, numDays,
                    numTimeFrames, ConstraintData.ParttimeConstraints.MinWorkingTimeOnWeek,
                    ConstraintData.ParttimeConstraints.MaxWorkingTimeOnWeek);
            }

            /*     //Constrains Ca làm việc có thể bắt đầu từ timeStart và kết thúc trước timeEnd
                 AddDomainWokingTimeConstraints(model, work_ft, numFTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.TimeStart, ConstraintData.TimeEnd);
                 AddDomainWokingTimeConstraints(model, work_pt, numPTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.TimeStart, ConstraintData.TimeEnd);*/

            //Tổng thời gian làm việc 1 ngày < maxHoursInDay
            AddMaxWorkingTimeInDayConstraints(model, work_ft, numFTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.FulltimeConstraints.MaxWorkingTimeInDay);
            AddMaxWorkingTimeInDayConstraints(model, work_pt, numPTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.ParttimeConstraints.MaxWorkingTimeInDay);

            //Mỗi nhân viên chỉ làm việc tại 1 ngày 1 vị trí 1 thời gian:
            AddUniqueWorkConstraint(model, work_ft, numFTStaffs, numPosition, numDays, numTimeFrames);
            AddUniqueWorkConstraint(model, work_pt, numPTStaffs, numPosition, numDays, numTimeFrames);

            //Nhân viên Fulltime được nghỉ ít nhất n ngày trong tuần
            AddMinDayOffConstrains(model, work_ft, numFTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.FulltimeConstraints.MinDayOff, ConstraintData.FulltimeConstraints.MaxDayOff);
            AddMinDayOffConstrains(model, work_pt, numPTStaffs, numPosition, numDays, numTimeFrames, ConstraintData.ParttimeConstraints.MinDayOff, ConstraintData.ParttimeConstraints.MaxDayOff);

            //Nhân viên e chỉ có thể làm việc tại các vị trí mà người đó đã đăng kí sẵn trong hợp đồng
            AddWorkBySkillConstraint(model, work_ft, numFTStaffs, numPosition, numDays, numTimeFrames, skillFTStaffs);
            AddWorkBySkillConstraint(model, work_pt, numPTStaffs, numPosition, numDays, numTimeFrames, skillPTStaffs);

            //Ca làm việc là những khoảng thời gian liên tục lớn hơn minShift

            //Có tối đa maxShiftsInDay
            foreach (int s in Range(numFTStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    var countShifts_Day = model.NewIntVar(0, ConstraintData.FulltimeConstraints.MaxShiftInDay * 2, $"count_shift_day(day={d},staff={s}");
                    var countShift_Pos_s = new IntVar[numPosition];
                    foreach (int p in Range(numPosition))
                    {
                        var works = new IntVar[numTimeFrames];
                        foreach (int t in Range(numTimeFrames))
                        {
                            works[t] = work_ft[s, p, d, t];
                        }

                        //đếm số ca làm việc = countShift_Pos/2
                        //countShift_Pos = số ca làm việc * 2
                        var countShift_Pos = model.NewIntVar(0, ConstraintData.FulltimeConstraints.MaxShiftInDay * 2, $"count_shift_pos");
                        countShift_Pos_s[p] = countShift_Pos;

                        //xác định có làm việc không tại day d, staff s, pos p
                        var isDontWortAt = model.NewBoolVar($"prod");
                        AddSequenceConstraint(model, works, ConstraintData.FulltimeConstraints.MaxShiftInDay, ConstraintData.FulltimeConstraints.MinSessionDuration, ConstraintData.FulltimeConstraints.MaxSessionDuration, numTimeFrames, countShift_Pos, isDontWortAt);

                    }
                    model.Add(countShifts_Day == LinearExpr.Sum(countShift_Pos_s));
                }
            }

            foreach (int s in Range(numPTStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    var countShifts_Day = model.NewIntVar(0, ConstraintData.ParttimeConstraints.MaxShiftInDay * 2, $"count_shift_day(day={d},staff={s}");
                    var countShift_Pos_s = new IntVar[numPosition];
                    foreach (int p in Range(numPosition))
                    {
                        var works = new IntVar[numTimeFrames];
                        foreach (int t in Range(numTimeFrames))
                        {
                            works[t] = work_pt[s, p, d, t];
                        }

                        //đếm số ca làm việc = countShift_Pos/2
                        //countShift_Pos = số ca làm việc * 2
                        var countShift_Pos = model.NewIntVar(0, ConstraintData.ParttimeConstraints.MaxShiftInDay * 2, $"count_shift_pos");
                        countShift_Pos_s[p] = countShift_Pos;

                        //xác định có làm việc không tại day d, staff s, pos p
                        var isDontWortAt = model.NewBoolVar($"prod");
                        AddSequenceConstraint(model, works, ConstraintData.ParttimeConstraints.MaxShiftInDay, ConstraintData.ParttimeConstraints.MinSessionDuration, ConstraintData.ParttimeConstraints.MaxSessionDuration, numTimeFrames, countShift_Pos, isDontWortAt);

                    }
                    model.Add(countShifts_Day == LinearExpr.Sum(countShift_Pos_s));
                }
            }


            //cover constraints for fulltime
            foreach (int d in Range(numDays))
            {
                foreach (int t in Range(numTimeFrames))
                {

                    foreach (int p in Range(numPosition))
                    {


                        var works = new List<IntVar>();

                        foreach (int s in Range(numFTStaffs))
                        {
                            works.Add(work_ft[s, p, d, t]);
                        }

                        foreach (int s in Range(numPTStaffs))
                        {
                            works.Add(work_pt[s, p, d, t]);
                        }

                        int demand = demands[d, p, t];

                        int overCoverPenalty = d == 5 || d == 6 ? 2 : 1;
                        int underCoverPenalty = d == 5 || d == 6 ? 6 : 3;

                        //đếm số nhân viên làm việc tại khoảng thời gian t ngày d

                        model.Add(LinearExpr.Sum(works) == demand);
                        /*if (demands[d, p, t] == 0)
                        {
                            model.Add(LinearExpr.Sum(works) == 0);                 
                        }
                        else
                        {
                            var worked = model.NewIntVar(1, totalStaff, "");
                            model.Add(LinearExpr.Sum(works) == worked);
                            var name = $"excessPanalty_demand(shift={t}, position={p}, day={d}";
                            var excessPanalty = model.NewIntVar(0, 100, name);
                            var excess = model.NewIntVar(-totalStaff, totalStaff, "excess");
                            var excessAbs = model.NewIntVar(0, totalStaff, "excessAbs");

                            //hệ số xác định tình trạng ca là under / over
                            var a = model.NewIntVar(0, totalStaff, "alpha");
                            // b = reality - demand
                            // a = (b + |b|)/2 => a = 0 if under || a = b if over
                            // excess panalty = a*overPanalty + (|b| - a)*underPanalty
                            model.Add(excess == worked - demand);
                            model.AddAbsEquality(excessAbs, excess);
                            model.Add(2 * a == (excess + excessAbs));
                            model.Add(excessPanalty == a * overCoverPenalty + (excessAbs - a) * underCoverPenalty);

                            objIntVars.Add(excessPanalty);
                            objIntCoeffs.Add(1);
                        }*/


                    }
                }
            }

            //OT minimize
            /*            foreach (int s in Range(numFTStaffs))
                        {
                            foreach (int d in Range(numDays))
                            {

                                var works = new List<IntVar>();
                                foreach (int p in Range(numPosition))
                                {
                                    foreach (int t in Range(numTimeFrames))
                                    {
                                        works.Add(work_ft[s, p, d, t]);
                                    }

                                }

                                //đếm số giờ làm việc
                                var workedTimesInday = model1.NewIntVar(0, numTimeFrames, "");
                                model1.Add(LinearExpr.Sum(works) == workedTimesInday);

                                var oT = model1.NewIntVar(-numTimeFrames, numTimeFrames, "ot");
                                var oT_Abs = model1.NewIntVar(0, numTimeFrames, "ot_Abs");
                                //hệ số xác định số giờ OT

                                var a = model1.NewIntVar(0, numTimeFrames, "oT_Times");

                                // số giờ làm việc - 8 => 
                                model1.Add(oT == workedTimesInday - ConstraintData.MaxNormalHour);
                                model1.AddAbsEquality(oT_Abs, oT);
                                model1.Add(2 * a == (oT + oT_Abs));

                                objIntVars.Add(a);
                                objIntCoeffs.Add(1);

                            }
                        }*/

            // Objective
            var objIntSum = LinearExpr.ScalProd(objIntVars, objIntCoeffs);

            model.Minimize(objIntSum);

            CpSolver solver = new CpSolver
            {
                // Adds a time limit. Parameters are stored as strings in the solver.
                StringParameters = $"num_search_workers:8, max_time_in_seconds:{timeLimit}"
            };

            CpSolverStatus status1 = solver.Solve(model);

            response.Status = status1;
            response.Conflicts = solver.NumConflicts();
            response.Branches = solver.NumBranches();
            response.WallTime = solver.WallTime();

            //work_ft[s, p, d, t] = model1.NewBoolVar($"workFT{s}_{p}_{d}_{t}");
            if (status1 == CpSolverStatus.Optimal || status1 == CpSolverStatus.Feasible)
            {


                foreach (int d in Range(numDays))
                {

                    foreach (int p in Range(numPosition))
                    {


                        foreach (int t in Range(numTimeFrames))
                        {
                            foreach (int s in Range(numFTStaffs))
                            {
                                sch_ft[s, p, d, t] = (int)solver.Value(work_ft[s, p, d, t]);
                            }

                            foreach (int s in Range(numPTStaffs))
                            {
                                sch_pt[s, p, d, t] = (int)solver.Value(work_pt[s, p, d, t]);
                            }
                        }

                    }
                }

                //sch_pt[s, p, d, t] = (int)solver.Value(work_pt[s, p, d, t]);
                List<ShiftAssignment> shifts = new();
                shifts.AddRange(ConvertToShiftAssignment(sch_ft, numPosition, numDays, numTimeFrames, numFTStaffs,
                                           DataInput.StaffDic[TypeStaff.FULL_TIME], DataInput.Skills));
                shifts.AddRange(ConvertToShiftAssignment(sch_pt, numPosition, numDays, numTimeFrames, numPTStaffs,
                                            DataInput.StaffDic[TypeStaff.PART_TIME], DataInput.Skills));
                response.ShiftAssignments = shifts;
            }
        }

        private List<ShiftAssignment> ConvertToShiftAssignment(int[,,,] sch, int numPosition, int numDays,
            int numTimeFrames, int numStaffs, List<Staff> staffs, List<Skill> skills)
        {
            List<ShiftAssignment> shifts = new();
            foreach (int d in Range(numDays))
            {

                foreach (int p in Range(numPosition))
                {

                    foreach (int s in Range(numStaffs))
                    {
                        var start = UNASSIGNED;
                        var end = UNASSIGNED;
                        foreach (int t in Range(numTimeFrames))
                        {
                            if (sch[s, p, d, t] == 1 && start == UNASSIGNED)
                            {
                                start = t;
                            }

                            if ((sch[s, p, d, t] == 0 || t == numTimeFrames - 1) && t != 0 && start != UNASSIGNED)
                            {
                                end = t - 1;
                            }
                            if (start < end)
                            {
                                //new shift 
                                DateTime StartTime = DateStart.AddDays(d).AddHours((double)start / 2);
                                DateTime EndTime;
                                if (end == numTimeFrames - 1)
                                {
                                    EndTime = DateStart.AddDays(d).AddHours((double)end / 2).AddMinutes(29);
                                } 
                                else
                                { 
                                    EndTime = DateStart.AddDays(d).AddHours((double)(end + 1) / 2);
                                }
                               
                                ShiftAssignment shift = new()
                                {
                                    SkillId = skills[p].Id,
                                    Username = staffs[s].Username,
                                    SkillName = skills[p].Name,
                                    TimeStart = StartTime,
                                    TimeEnd = EndTime

                                };
                                shifts.Add(shift);
                                //reset
                                start = UNASSIGNED;
                                end = UNASSIGNED;
                            }


                        }
                    }
                }
            }

            return shifts;
        }

        private static IntVar[,,,] NewBoolVars(CpModel model, string namePrefix,
            int numStaffs, int numDays, int numPosition,
            int numTimeFrames)
        {
            IntVar[,,,] work = new IntVar[numStaffs, numPosition, numDays, numTimeFrames];
            foreach (int s in Range(numStaffs))
            {
                foreach (int p in Range(numPosition))
                {
                    foreach (int d in Range(numDays))
                    {
                        foreach (int t in Range(numTimeFrames))
                            work[s, p, d, t] = model.NewBoolVar($"{namePrefix} ({s}_{p}_{d}_{t}");
                    }
                }
            }
            return work;
        }

        /// <summary>
        ///    Nhân viên chỉ làm việc trong khoảng thời gian availableTimes cho trước
        /// </summary>
        /// <param name="model"></param>
        /// <param name="work_sche"></param>
        /// <param name="availableTimes"></param>
        /// <param name="numStaffs"></param>
        /// <param name="numPosition"></param>
        /// <param name="numDays"></param>
        /// <param name="numTimeFrames"></param>
        private static void AddWokingInAvailableTimeConstrain(
            CpModel model, IntVar[,,,] work_sche,
            int[,,] availableTimes, int numStaffs,
            int numPosition, int numDays, int numTimeFrames)
        {
            foreach (int e in Range(numStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    foreach (int p in Range(numPosition))
                    {
                        foreach (int t in Range(numTimeFrames))
                        {
                            model.Add(work_sche[e, p, d, t] <= availableTimes[e, d, t]);
                        }
                    }
                }
            }
        }

        private static void AddLimitWokingTimeByWeekConstraint(CpModel model, IntVar[,,,] work_ft,
            int staffIndex, int numPosition, int numDays,
            int numTimeFrames, int min, int max)
        {
            var sumWorkTimeByWeek = new List<IntVar>();
            foreach (int d in Range(numDays))
            {
                foreach (int p in Range(numPosition))
                {

                    foreach (int t in Range(numTimeFrames))
                    {
                        sumWorkTimeByWeek.Add(work_ft[staffIndex, p, d, t]);
                    }
                }
            }
            model.AddLinearConstraint(LinearExpr.Sum(sumWorkTimeByWeek), min, max);
            // model.Add(LinearExpr.Sum(sumWorkTimeByWeek) >= min);
            // model.Add(sumWorkTimeByWeek <= max);
        }

        private static void AddWorkBySkillConstraint(CpModel model, IntVar[,,,] work_ft,
            int numStaffs, int numPosition, int numDays,
            int numTimeFrames, int[,] skillStaffs)
        {
            foreach (int s in Range(numStaffs))
            {
                foreach (int p in Range(numPosition))
                {
                    if (skillStaffs[s, p] == 0)
                    {
                        var sequence = new List<ILiteral>();
                        foreach (int d in Range(numDays))
                        {
                            foreach (int t in Range(numTimeFrames))
                            {
                                sequence.Add(work_ft[s, p, d, t].Not());
                            }
                        }

                        model.AddBoolOr(sequence);
                    }
                }
            }
        }
        private static void AddUniqueWorkConstraint(CpModel model, IntVar[,,,] work_ft,
            int numStaffs, int numPosition, int numDays, int numTimeFrames)
        {
            foreach (int s in Range(numStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    foreach (int t in Range(numTimeFrames))
                    {
                        var sum = new IntVar[numPosition];

                        foreach (int k in Range(numPosition))
                        {
                            sum[k] = work_ft[s, k, d, t];
                        }

                        model.Add(LinearExpr.Sum(sum) <= 1);
                    }
                }
            }
        }

        private static void AddDomainWokingTimeConstraints(CpModel model, IntVar[,,,] work_ft,
            int numStaffs, int numPosition, int numDays,
            int numTimeFrames, int timeStart, int timeEnd)
        {
            foreach (int s in Range(numStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    var sumStart = new List<IntVar>();
                    var sumEnd = new List<IntVar>();
                    foreach (int p in Range(numPosition))
                    {
                        foreach (int t in Range(numTimeFrames))
                        {
                            if (t < timeStart)
                            {
                                sumStart.Add(work_ft[s, p, d, t]);
                            }

                            if (t > timeEnd)
                            {
                                sumStart.Add(work_ft[s, p, d, t]);
                            }
                        }
                    }
                    model.Add(LinearExpr.Sum(sumStart) == 0);
                    model.Add(LinearExpr.Sum(sumEnd) == 0);
                }
            }
        }

        private static void AddMaxWorkingTimeInDayConstraints(CpModel model, IntVar[,,,] work_sch,
            int numStaffs, int numPosition, int numDays,
            int numTimeFrames, int maxWorkingTimeInDay)
        {
            foreach (int s in Range(numStaffs))
            {
                foreach (int d in Range(numDays))
                {
                    var sumWorkTimeByDay = new List<IntVar>();
                    foreach (int p in Range(numPosition))
                    {
                        foreach (int t in Range(numTimeFrames))
                        {
                            sumWorkTimeByDay.Add(work_sch[s, p, d, t]);
                        }
                    }
                    model.Add(LinearExpr.Sum(sumWorkTimeByDay) <= maxWorkingTimeInDay);
                }
            }
        }

        private static void AddMinDayOffConstrains(CpModel model, IntVar[,,,] work,
            int numStaffs, int numPosition, int numDays,
            int numTimeFrames, int mindayOff, int maxdayOff)
        {
            foreach (int s in Range(numStaffs))
            {
                //Giá trị của dayWork = 1 nếu ngày d có làm và ngược lại
                IntVar[] dayWorks = new IntVar[numDays];

                foreach (int d in Range(numDays))
                {
                    var name = $"workDay(staff={s},day={d})";
                    dayWorks[d] = model.NewBoolVar(name);

                    //var sequence = new List<ILiteral>();
                    var temp = new List<IntVar>();
                    foreach (int t in Range(numTimeFrames))
                    {
                        foreach (int k in Range(numPosition))
                        {
                            temp.Add(work[s, k, d, t]);
                        }
                    }
                    model.AddMaxEquality(dayWorks[d], temp);
                    //workDays[s, d] = dayWorks[d];
                }

                // tổng ngày làm việc ít hơn tổng ngày trong tuần trừ số ngày nghỉ tối thiểu
                model.Add(LinearExpr.Sum(dayWorks) <= numDays - mindayOff);
                model.Add(LinearExpr.Sum(dayWorks) >= numDays - maxdayOff);
            }
        }

        private static void AddSequenceConstraint(CpModel model, IntVar[] works,
            int maxShiftsInDay, int minShiftDuration, int maxShiftDuration,
            int numTimeFrames, IntVar count, IntVar isWortAt)
        {
            //Đếm số sub-sequence(ca làm việc)
            var n = numTimeFrames;
            var arrTemp = new IntVar[n + 1];
            var arrTemp1 = new IntVar[n + 1];

            foreach (int t in Range(n + 1))
            {
                arrTemp[t] = model.NewIntVar(0, 2, $"d_subsequence{t}");
                arrTemp1[t] = model.NewIntVar(0, 2, $"d_mod{t}");
            }

            //count
            foreach (int t in Range(n + 1))
            {
                var start = t - 1;
                var lenght = 2;
                // cặp 2 phần từ liên tiếp
                //nếu 2 phần từ liên tiếp khác nhau => có sự bắt đầu ca hoặc kết thúc ca
                var transition = new List<IntVar>();
                foreach (var i in Range(lenght))
                {
                    //trường hợp đặc biệt
                    // phần từ đầu tiên
                    if (start == -1 && i == 0)
                    {
                        continue;
                    }

                    // phần từ đầu cuối cùng
                    if (start == n - 1 && i == lenght - 1)
                    {
                        continue;
                    }
                    transition.Add(works[start + i]);
                }
                model.Add(arrTemp[t] == LinearExpr.Sum(transition));
                model.AddModuloEquality(arrTemp1[t], arrTemp[t], 2);
            }

            //count subsequence

            model.Add(count == LinearExpr.Sum(arrTemp1));
            //model.Add(count <= maxShiftsInDay * 2);

            //Mỗi (ca làm việc) bao gồm những những sub-sequence liên tục có độ dài > minShift
            //check các element sub-sequence tất cả có = 0 <=>không có ca làm nào tại vị trí đó vào ngày hôm đó không           
            model.AddMaxEquality(isWortAt, works);
            ILiteral check = isWortAt;

            // cấm các ca làm việc có thời gian nhỏ hơn minShiftDuration hoặc không có ca nào diễn ra
            foreach (var length in Range(1, minShiftDuration))
            {
                foreach (var start in Range(works.Length - length + 1))
                {
                    model.AddBoolOr(NegatedBoundedSpan(works, start, length)).OnlyEnforceIf(check);
                }
            }

            //  cấm các ca làm việc có thời gian maxShiftDuration + 1
            foreach (var start in Range(works.Length - maxShiftDuration))
            {
                var temp = new List<ILiteral>();

                foreach (var i in Range(start, start + maxShiftDuration + 1))
                {
                    temp.Add(works[i].Not());
                }
                model.AddBoolOr(temp).OnlyEnforceIf(check);
            }
        }

        /// <summary>
        /// Filters an isolated sub-sequence of variables assigned to True.
        /// Extract the span of Boolean variables[start, start + length), negate them,
        /// and if there is variables to the left / right of this span, surround the
        /// span by them in non negated form.
        /// </summary>
        /// <param name="works">A list of variables to extract the span from.</param>
        /// <param name="start">The start to the span.</param>
        /// <param name="length">The length of the span.</param>
        /// <returns>An array of variables which conjunction will be false if the
        /// sub-list is assigned to True, and correctly bounded by variables assigned
        /// to False, or by the start or end of works.</returns>
        private static ILiteral[] NegatedBoundedSpan(IntVar[] works, int start, int length)
        {
            var sequence = new List<ILiteral>();

            if (start > 0)
                sequence.Add(works[start - 1]);

            foreach (var i in Range(length))
                sequence.Add(works[start + i].Not());

            if (start + length < works.Length)
                sequence.Add(works[start + length]);

            return sequence.ToArray();
        }

        /// <summary>
        /// C# equivalent of Python range (start, stop)
        /// </summary>
        /// <param name="start">The inclusive start.</param>
        /// <param name="stop">The exclusive stop.</param>
        /// <returns>A sequence of integers.</returns>
        private static IEnumerable<int> Range(int start, int stop)
        {
            foreach (var i in Enumerable.Range(start, stop - start))
                yield return i;
        }

        /// <summary>
        /// C# equivalent of Python range (stop)
        /// </summary>
        /// <param name="stop">The exclusive stop.</param>
        /// <returns>A sequence of integers.</returns>
        private static IEnumerable<int> Range(int stop)
        {
            return Range(0, stop);
        }
    }
}
