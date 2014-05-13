namespace Golf.Web
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
    using Microsoft.Xrm.Sdk;
    public class AllEventData
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public string EventStatusName { get; set; }

        public Guid GolfTeamId { get; set; }
        public string GolfTeam { get; set; }
        public bool OnTheClock { get; set; }
        public DateTime OnTheClockSince { get; set; }

        public Guid GolferId { get; set; }
        public string Golfer { get; set; }
        public string GolferImgUrl { get; set; }
        public int PickNumber { get; set; }
        public DateTime DraftPickCompletedOn { get; set; }
        public DateTime DraftPickCreatedOn { get; set; }

        public Guid CourseId { get; set; }
        public string CourseName { get; set; }
        public Guid CourseHoleId { get; set; }
        public int CourseHoleNumber { get; set; }
        public int Par { get; set; }
        public int CourseHandicap { get; set; }

        public Guid CourseHoleHandicapId { get; set; }
        public int CourseHoleHandicapHoleNumber { get; set; }
        public OptionSetValue Day { get; set; }
        public decimal CalculatedHandicap { get; set; }
        public int CalculatedHandicapRank { get; set; }

    }
}