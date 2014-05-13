using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class AllLeagues
    {
        public List<League> Leagues { get; set; }

        public AllLeagues()
        {
            Leagues = new List<League>();
        }
    }
}