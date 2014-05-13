using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class PlayerMatchupsByTeam
    {
        public Guid EventId { get; set; }
        public List<PlayerMatchups> TeamPlayerMatchups { get; set; }
        public decimal TotalWL { get; set; }
        public decimal AcceptedW { get; set; }
        public decimal AcceptedL { get; set; }

        public PlayerMatchupsByTeam()
        {
            TeamPlayerMatchups = new List<PlayerMatchups>();
        }
    }
}