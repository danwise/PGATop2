using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class EventXml
    {

        public string CurrentXML { get; set; }
        
        public string CreatedOn { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public List<Scorecard> Scorecards { get; set; }
        public List<GolfTeam> GolfTeams { get; set; }
        public string BestBallLeader_Thurs { get; set; }
        public string BestBallLeader_Fri { get; set; }
        public string GolferLeaderboard { get; set; }
        public string TeamTop2Leader { get; set; }
        public int SkinsCount_Sat { get; set; }
        public int SkinsCount_Sun { get; set; }
        public string GolfTeamOnTheClock { get; set; }
        public string LastDraftPick { get; set; }
        public string EventStatusName { get; set; }
        public string LeagueStatusName { get; set; }
        public decimal SkinsAmount { get; set; }
        public decimal BestBallAmount { get; set; }
        public string PlaySuspendedReason { get; set; }
        public List<LeagueProduct> LeagueProducts { get; set; }

        public EventXml()
        {
            Scorecards = new List<Scorecard>();
            GolfTeams = new List<GolfTeam>();
            LeagueProducts = new List<LeagueProduct>();
        }
    }
}