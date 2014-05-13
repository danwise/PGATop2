using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class PlayerMatchupProposal
    {
        public Guid PlayerMatchupId { get; set; }
        public Guid GolfTeamId { get; set; }
        public decimal BaselineAmount { get; set; }
        public decimal GolferAToWinAmount { get; set; }
        public decimal GolferBToWinAmount { get; set; }

        public DateTime CreatedOn { get; set; }
        public string ProposedBy { get; set; }
        public string GolferA { get; set; }
        public string GolferB { get; set; }
        public string Name { get; set; }
    }
}