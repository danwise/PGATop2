namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class Round
    {
        public Guid RoundId { get; set; }
        public string RoundName { get; set; }
        public string TeeTime { get; set; }
        public DateTime dtTeeTime { get; set; }
        public int RoundScore { get; set; }
        public int RoundShots { get; set; }
        public int TotalScore { get; set; }
        public int Thru { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ScorecardUrl { get; set; }
        public OptionSetValue Day { get; set; }
        public int DayNumber { get; set; }
        public string DayName { get; set; }
        public string RoundEventStatus { get; set; }
        public string RoundGolferEventStatus { get; set; }
        public string TournamentRank { get; set; }
        public bool RoundStarted { get; set; }

        //Golfer
        public Guid GolferId { get; set; }
        public string RoundGolferName { get; set; }
        public string RoundGolferStatus { get; set; }
        
        //GolfTeam
        public Guid RoundGolfTeamId { get; set; }
        public string RoundGolfTeamName { get; set; }


        public List<Hole> Holes { get; set; }

        public Round()
        {
            Holes = new List<Hole>();
        }
    }
}