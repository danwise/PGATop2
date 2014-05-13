

namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class Hole
    {
        public Guid HoleId { get; set; }
        public Guid RoundId { get; set; }
        public string RoundName { get; set; }
        public int RoundScore { get; set; }
        public int RoundShots { get; set; }
        public int RoundThru { get; set; }
        public string RoundEventStatus { get; set; }
        public Guid GolfTeamId { get; set; }
        public Guid HoleGolferId { get; set; }
        public string HoleGolfer { get; set; }
        public string HoleGolfTeam { get; set; }
        public string Name { get; set; }
        public int HoleNumber { get; set; }
        public int Shots { get; set; }
        public string ScoreName { get; set; }
        public bool BestBall { get; set; }
        public bool Skin { get; set; }
        public OptionSetValue HoleDay { get; set; }
        public int CalculatedHandicapRank { get; set; }
        public int CourseHandicap { get; set; }
        public int CourseHandicapDayNumber { get; set; }
        public OptionSetValue CalculatedHandicapDay { get; set; }
        public decimal CalculatedHandicap { get; set; }
        public int Par { get; set; }
        public List<PossibleHole> TeamPlays { get; set; }
        public List<PossibleHole> EventPlays { get; set; }
        public int NumberOfPlays { get; set; }
        public bool BestBallTieBreaker { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CourseHoleId { get; set; }
        public int DayNumber { get; set; }
        public int ActiveGolferCount { get; set; }

        public int SkinBirdieCount_Sat { get; set; }
        public int SkinEagleCount_Sat { get; set; }
        public int SkinParCount_Sat { get; set; }
        public int SkinBogeyCount_Sat { get; set; }

        public int SkinBirdieCount_Sun { get; set; }
        public int SkinEagleCount_Sun { get; set; }
        public int SkinParCount_Sun { get; set; }
        public int SkinBogeyCount_Sun { get; set; }


        public Hole()
        {
            TeamPlays = new List<PossibleHole>();
            EventPlays = new List<PossibleHole>();
           
        }
        

        private String getScoreName(int par, int shots)
        {
            String rtn = "";
            switch (shots - par)
            {
                case -3:
                    rtn = "Double Eagle";
                    break;
                case -2:
                    rtn = "Eagle";
                    break;
                case -1:
                    rtn = "Birdie";
                    break;
                case 0:
                    rtn = "Par";
                    break;
                case 1:
                    rtn = "Bogey";
                    break;
                case 2:
                    rtn = "Double Bogey";
                    break;
                case 3:
                    rtn = "Triple Bogey";
                    break;
            }
            if (shots == 1)
                rtn = "Hole in One!!!!!";

            return rtn;
        }
    }
}