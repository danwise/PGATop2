using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class PlayerMatchups
    {
        
        public Guid MatchupGolfTeamId { get; set; }
        public string MatchupGolfTeamName { get; set; }
        public decimal MatchupGolfTeamWinLoss { get; set; }
        public decimal EventMatchupsWinLoss { get; set; }
        public decimal ThursdayMatchupsWinLoss { get; set; }
        public decimal FridayMatchupsWinLoss { get; set; }
        public decimal SaturdayMatchupsWinLoss { get; set; }
        public decimal SundayMatchupsWinLoss { get; set; }
        public List<PlayerMatchup> EventMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> ThursdayMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> FridayMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> SaturdayMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> SundayMatchups = new List<PlayerMatchup>();

        public decimal CompletedMatchupsWinLoss { get; set; }
        public List<PlayerMatchup> ActiveMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> ProposedMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> InProgressMatchups = new List<PlayerMatchup>();
        public List<PlayerMatchup> CompletedMatchups = new List<PlayerMatchup>();

    }
}