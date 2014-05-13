using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class BestBallHelper
    {
        public Contact contact { get; set; }
        public int BestBallScore { get; set; }
        public zz_golfteam golfteam { get; set; }
        public Boolean OwnsTieBreaker { get; set; }
        public zz_hole TieBreaker { get; set; }
        public int TotalHoles { get; set; }
        public int HolesPlayedOutof18 { get; set; }

        public BestBallHelper()
        {
            BestBallScore = 0;
            OwnsTieBreaker = false;

        }
    }
}