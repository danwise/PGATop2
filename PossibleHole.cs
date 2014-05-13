

namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class PossibleHole
    {
        public Guid pHoleParentHoleId { get; set; }
        public Guid pHoleId { get; set; }
        public Guid pHoleRoundId { get; set; }
        public Guid pHoleGolfTeamId { get; set; }
        public string pHoleGolfTeam { get; set; }
        public string pHoleHoleGolfer { get; set; }
        public string pHoleName { get; set; }
        public int pHoleNumber { get; set; }
        public int pHoleShots { get; set; }
        public string pHoleScoreName { get; set; }
        public bool pHoleBestBall { get; set; }
        public bool pHoleSkin { get; set; }
        public OptionSetValue pHoleDay { get; set; }
        public DateTime pHoleCreatedOn { get; set; }
        public int CalculatedHandicapRank { get; set; }
        public int pHolePar { get; set; }
        public int pActiveGolfers { get; set; }
        
    }
}
