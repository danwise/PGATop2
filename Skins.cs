using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class Skins
    {
        public int ActiveGolferCount { get; set; }
        public List<Hole> Skins_Sat { get; set; }
        public List<Hole> Skins_Sun { get; set; }
        

        public Skins()
        {
            Skins_Sat = new List<Hole>();
            Skins_Sun = new List<Hole>();
        }
    }
}