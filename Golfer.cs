using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class Golfer
    {
        public Guid GolferId { get; set; }
        public string GolferName { get; set; }
        public string GolferImgUrl { get; set; }
        public int DraftPickNumber { get; set; }
        public DateTime DraftPickCompletedOn { get; set; }
        public int Odds { get; set; }
        public bool MissedCut { get; set; }
        public string GolferStatus { get; set; }
        public string GolferRotoWorldUrl { get; set; }
        public int TotalScore { get; set; }
        public DateTime LastUpdate { get; set; }
        public int Thru { get; set; }
        public int RoundScore { get; set; }
        public bool isCutLine { get; set; }
       // public Guid GolfTeamId { get; set; }

        //not gonna bew needed
        public Guid GolfTeamId { get; set; }
        public string GolferGolfTeam { get; set; }
        public int GolferScore { get; set; }
        public int HolesCompleted { get; set; }
        public string RoundInProgress { get; set; }
        public string TeamPageUrl { get; set; }
        public string GolferPageUrl { get; set; }
        public bool TeamTop2 { get; set; }
        public int TeamRank { get; set; }
        public string TournamentRank { get; set; }
        public int CutLineRank { get; set; }
        public string NextTeeTime { get; set; }
        public DateTime dtNextTeeTime { get; set; }
        public bool TodaysRoundStarted { get; set; }
        public int DayNumber { get; set; }
        public string ShotTrackerUrl { get; set; }
       


        //List last
        public List<Round> Rounds { get; set; }
        public Golfer()
        {
            Rounds = new List<Round>();
        }
    }
}