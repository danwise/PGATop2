using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class League
    {
        public string LeagueName { get; set; }
        public Guid LeagueId { get; set; }
        public AllGolfTeams GolfTeams {get;set;}
    }
}