using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class AllGolfTeams
    {
        public List<GolfTeam> GolfTeams { get; set; }

        public AllGolfTeams()
        {
            GolfTeams = new List<GolfTeam>();
        }
    }
}