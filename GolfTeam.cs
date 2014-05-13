using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class GolfTeam:ICloneable
    {
        public string GolfTeamName { get; set; }
        public Guid GolfTeamId { get; set; }
        public string TeamUrl { get; set; }
        public int TeamTop2Score { get; set; }
        public int Top4Score { get; set; }
        public int BestBallScore_Thurs { get; set; }
        public int BestBallScore_Fri { get; set; }
        public int BestBallScore_36 { get; set; }
        public int SkinCount_Sat { get; set; }
        public int SkinCount_Sun { get; set; }
        public bool BestBallTieBreaker_Thurs { get; set; }
        public bool BestBallTieBreaker_Fri { get; set; }
        public bool BestBallTieBreaker_36 { get; set; }
        public bool OnTheClock { get; set; }
        public DateTime OnTheClockSince { get; set; }

        public List<Golfer> Golfers { get; set; }
        public List<Hole> BestBallHoles_Thurs { get; set; }
        public List<Hole> BestBallHoles_Fri { get; set; }
        public List<Hole> BestBallHoles_36 { get; set; }
        public int ActiveGolfers { get; set; }
        public List<GolfTeam> Delegates { get; set; }

        public int BestBallBirdieCount_Thurs { get; set; }
        public int BestBallEagleCount_Thurs { get; set; }
        public int BestBallParCount_Thurs { get; set; }
        public int BestBallBogeyCount_Thurs { get; set; }

        public int BestBallBirdieCount_Fri { get; set; }
        public int BestBallEagleCount_Fri { get; set; }
        public int BestBallParCount_Fri { get; set; }
        public int BestBallBogeyCount_Fri { get; set; }


        public int BestBallBirdieCount_36 { get; set; }
        public int BestBallEagleCount_36 { get; set; }
        public int BestBallParCount_36{ get; set; }
        public int BestBallBogeyCount_36 { get; set; }

        public Guid LeagueId { get; set; }


        public GolfTeam()
        {
            Golfers = new List<Golfer>();
            BestBallHoles_Thurs = new List<Hole>();
            BestBallHoles_Fri = new List<Hole>();
            BestBallHoles_36 = new List<Hole>();
            GolfTeamId = Guid.Empty;
            GolfTeamName = "";
            Delegates = new List<GolfTeam>();
        }

        public object Clone()
        {    
            return this.MemberwiseClone();
        }

        public GolfTeam CloneGolfTeam()
        {
            return (GolfTeam)Clone();
        }


    }

    public class NumberRank
    {
        public int Number { get; set; }
        public int Rank { get; set; }

        public NumberRank(int number, int rank)
        {
            Number = number;
            Rank = rank;
        }
    }

}