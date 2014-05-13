using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class AllGolfers
    {
        public List<Golfer> Golfers { get; set; }
        public string EventStatusName { get; set; }
        public AllGolfers()
        {
            Golfers = new List<Golfer>();
        }
    }
}