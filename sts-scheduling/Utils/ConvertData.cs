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
                  DemandBySkills = ConvertFromRequest(demandDayRequest.DemandBySkills,NumTimeFrames).ToArray()
                };
                result.Add(demandDay);
            }

            return result;
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
                    End = ConvertDateTimeToIndexTime(demandDayRequest.End, NumTimeFrames)
                }

            }).ToList();

            return result;
        }


        public static IEnumerable<AvailableDay> ConvertFromRequest(IEnumerable<AvalailableDayRequest> avalailableDays , int NumTimeFrames)
        {
            List<AvailableDay> result = new();

            foreach(AvalailableDayRequest avalailableDayRequest in avalailableDays)
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

            foreach(AvailableTimeRequest availableTime in availableTimeRequests)
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
                    End = ConvertDateTimeToIndexTime(availableTime.End, NumTimeFrames)
                };
        }

        public static int ConvertDateTimeToIndexTime(DateTime dateTime, int NumTimeFrames)
        {
            return (int)((dateTime.Hour + Math.Ceiling((double)dateTime.Minute / 60))
                                                * ((double)NumTimeFrameNormal / NumTimeFrames));
        }


    }
}
