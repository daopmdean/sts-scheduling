using sts_scheduling.Data;
using sts_scheduling.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sts_scheduling.Utils
{
    public static class ConvertData
    {
        //1 Day = 24h
        public const int NumTimeFrameNormal = 24;
        public static IEnumerable<DemandDay> ConvertFromRequest(IEnumerable<DemandDayRequest> demandDayRequests, int NumTimeFrames)
        {
            List<DemandDay> result = new();

            foreach (DemandDayRequest demandDayRequest in demandDayRequests)
            {
                DemandDay demandDay = new()
                {
                    Day = demandDayRequest.Day,
                    DemandBySkills = ConvertFromRequest(demandDayRequest.DemandBySkills, NumTimeFrames).ToArray()
                };
                result.Add(demandDay);
            }

            return result.ToArray();
        }

        public static IEnumerable<DemandSkill> ConvertFromRequest(IEnumerable<DemandSkillRequest> demandSkillRequests, int NumTimeFrames)
        {
            List<DemandSkill> result = new();

            foreach (DemandSkillRequest demandDayRequest in demandSkillRequests)
            {
                DemandSkill demandDay = new()
                {
                    SkillId = demandDayRequest.SkillId,
                    Demands = ConvertFromRequest(demandDayRequest.Demands, NumTimeFrames).ToArray()
                };
                result.Add(demandDay);
            }

            return result;
        }

        public static IEnumerable<Demand> ConvertFromRequest(IEnumerable<DemandRequest> demandRequests, int NumTimeFrames)
        {
            List<Demand> result = demandRequests.Select(demandDayRequest => new Demand
            {
                Quantity = demandDayRequest.Quantity,
                Level = demandDayRequest.Level,
                Session = new Session
                {
                    Start = ConvertDateTimeToIndexTime(demandDayRequest.Start, NumTimeFrames),
                    End = ConvertDateTimeToIndexTime(demandDayRequest.End, NumTimeFrames) - 1
                }

            }).ToList();

            return result;
        }


        public static IEnumerable<AvailableDay> ConvertFromRequest(IEnumerable<AvalailableDayRequest> avalailableDays, int NumTimeFrames)
        {
            List<AvailableDay> result = new();

            foreach (AvalailableDayRequest avalailableDayRequest in avalailableDays)
            {
                AvailableDay availableDay = new()
                {
                    Day = avalailableDayRequest.Day,
                    AvailableTimes = ConvertFromRequest(avalailableDayRequest.AvailableTimes, NumTimeFrames).ToArray()
                };
                result.Add(availableDay);
            }

            return result.ToArray();
        }

        public static IEnumerable<Session> ConvertFromRequest(IEnumerable<AvailableTimeRequest> availableTimeRequests, int NumTimeFrames)
        {
            List<Session> availableTimes = new List<Session>();

            foreach (AvailableTimeRequest availableTime in availableTimeRequests)
            {
                Session session = ConvertAvailableTimeRequestToSession(availableTime, NumTimeFrames);
                availableTimes.Add(session);
            }

            return availableTimes;
        }

        public static Session ConvertAvailableTimeRequestToSession(AvailableTimeRequest availableTime, int NumTimeFrames)
        {
            return new Session
            {
                Start = ConvertDateTimeToIndexTime(availableTime.Start, NumTimeFrames),
                End = ConvertDateTimeToIndexTime(availableTime.End, NumTimeFrames) - 1
            };
        }

        public static int ConvertDateTimeToIndexTime(DateTime dateTime, int NumTimeFrames)
        {
            double minuteToIndex = 0;
            if(dateTime.Minute > 5)
            {
                minuteToIndex = dateTime.Minute <= 30 ? 0.5 : 1;
            }
            return (int)((double)(dateTime.Hour + minuteToIndex)
                                                * ((double)NumTimeFrames / NumTimeFrameNormal));
        }

        public static ConstraintData ConvertFromRequest(ConstraintDataRequest constraintDataRequest)
        {
            ConstraintData constraintData = new ConstraintData()
            {
                MinDistanceBetweenSession = constraintDataRequest.MinDistanceBetweenSession*2,
                FulltimeConstraints = new()
                {
                    MinDayOff = constraintDataRequest.FulltimeConstraints.MinDayOff,
                    MaxDayOff = constraintDataRequest.FulltimeConstraints.MaxDayOff,

                    MinWorkingTimeOnWeek = (int)constraintDataRequest.FulltimeConstraints.MinHoursPerWeek * 2,
                    MaxWorkingTimeOnWeek = (int)constraintDataRequest.FulltimeConstraints.MaxHoursPerWeek * 2,

                    MinSessionDuration = (int)constraintDataRequest.FulltimeConstraints.MinShiftDuration * 2,
                    MaxSessionDuration = (int)constraintDataRequest.FulltimeConstraints.MaxShiftDuration *2,

                    MinWorkingTimeInDay = (int)constraintDataRequest.FulltimeConstraints.MinHoursPerDay * 2,
                    MaxWorkingTimeInDay = (int)constraintDataRequest.FulltimeConstraints.MaxHoursPerDay * 2,

                    //unuse
                    MaxNormalHour = constraintDataRequest.FulltimeConstraints.MaxNormalHour * 2,
                    MaxShiftInDay = constraintDataRequest.FulltimeConstraints.MaxShiftPerDay,
                },
                ParttimeConstraints = new()
                {
                    MinDayOff = constraintDataRequest.ParttimeConstraints.MinDayOff,
                    MaxDayOff = constraintDataRequest.ParttimeConstraints.MaxDayOff,

                    MinWorkingTimeOnWeek = (int)constraintDataRequest.ParttimeConstraints.MinHoursPerWeek * 2,
                    MaxWorkingTimeOnWeek = (int)constraintDataRequest.ParttimeConstraints.MaxHoursPerWeek * 2,

                    MinSessionDuration = (int)constraintDataRequest.ParttimeConstraints.MinShiftDuration * 2,
                    MaxSessionDuration = (int)constraintDataRequest.ParttimeConstraints.MaxShiftDuration * 2,

                    MinWorkingTimeInDay = (int)constraintDataRequest.ParttimeConstraints.MinHoursPerDay * 2,
                    MaxWorkingTimeInDay = (int)constraintDataRequest.ParttimeConstraints.MaxHoursPerDay * 2,
                    //unuse
                    MaxNormalHour = constraintDataRequest.ParttimeConstraints.MaxNormalHour*2,
                    MaxShiftInDay = constraintDataRequest.ParttimeConstraints.MaxShiftPerDay,
                }
            };

            return constraintData;
        }

    }
}
