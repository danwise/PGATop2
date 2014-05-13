

namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class AllData
    {
        //Event
        
        public string EventStatusName { get; set; }
        //Team
        public Guid GolfTeamId { get; set; }
        public string GolfTeam { get; set; }
        public bool OnTheClock { get; set; }
        public DateTime OnTheClockSince { get; set; }
        //Golfer
        public Guid GolferId { get; set; }
        public string Golfer { get; set; }
        public string GolferImgUrl { get; set; }
        public string GolferRotoWorldUrl { get; set; }
        public int PickNumber { get; set; }
        public bool GolferMissedCut { get; set; }
        public string GolferStatus { get; set; }

        //Round
        public Guid RoundId { get; set; }
        public string RoundName { get; set; }
        public int RoundScore { get; set; }
        public int TotalScore { get; set; }
        public int RoundShots { get; set; }
        public int Thru { get; set; }
        public string RoundInProgress { get; set; }
        public string TeeTime { get; set; }
        public DateTime dtTeeTime { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ScorecardUrl { get; set; }
        public OptionSetValue Day { get; set; }
        public string DayName { get; set; }
        public String RoundEventStatus { get; set; }
        public String TournamentRank { get; set; }

        //Hole
        public Guid HoleId { get; set; }
        public int HoleNumber { get; set; }
        public int HoleShots { get; set; }
        public decimal HoleHandicap { get; set; }
        public int HoleHandicapRank { get; set; }
        public int CourseHoleHandicap { get; set; }
        public bool Skin {get;set;}
        public bool BestBall { get; set; }
    }
}