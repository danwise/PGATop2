using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class TeamTop2Helper
    {
        public int[] Scores { get; set; }
        public int TotalScore { get; set; }
        public Contact contact { get; set; }
        public zz_golfteam golfTeam { get; set; }
        public List<TeamTop2Golfer> golfers { get; set; }
        public int QualifyScore { get; set; }
        public string GolfTeamPage { get; set; }
        public string GolfTeamDirectory { get; set; }
        public zz_round Round { get; set; }
        public TeamTop2Helper(zz_golfteam gt)
        {
            Scores = new int[2];
            Scores[0] = 0;
            Scores[1] = 0;
            golfTeam = new zz_golfteam();
            golfers = new List<TeamTop2Golfer>();
            golfTeam = gt;
            QualifyScore = 100;
        }
    }

    public class TeamTop2Golfer
    {
        public zz_golfer golfer { get; set; }
        public string Thru { get; set; }
        //public int CompletedHoles { get; set; }
        //public DateTime TeeTime { get; set; }

        public TeamTop2Golfer(int ch, DateTime tt)
        {
            golfer = new zz_golfer();
            DateTime dt = new DateTime();
            if (ch == 0)
                Thru = tt.AddHours(-7).ToShortTimeString();
            else
                Thru = ch.ToString() + "/18";

        }
    }
}