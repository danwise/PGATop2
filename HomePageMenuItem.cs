namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xrm.Sdk;
    public class HomePageMenuItem
    {
        public string hRef { get; set; }
        public string Title { get; set; }
        public int count { get; set; }
        public string WinnerText { get; set; }
        public Boolean HasCount { get; set; }
        public int Day { get; set; }
        public String Type { get; set; }
        public Guid EventId { get; set; }
        //public enum BetType { Skins, BestBall, TeamTop2, Golfer };
        public enum Days { Thursday = 1, Friday, Saturday, Sunday };
        private Shared s;


        public HomePageMenuItem()
        {
            count = 0;
            HasCount = false;
            Day = 0;
            s = new Shared();
        }

        public void GetWinner(ServiceContext dc)
        {
            try
            {
                switch (Type)
                {
                    case "BestBall":
                        GetBBWinner(dc);
                        break;
                    case "TeamTop2":
                        GetTT2Winner(dc);
                        break;
                    case "Golfer":
                        GetLeadingGolfer(dc);
                        break;
                }
            }
            catch
            {
                throw;
            }

        }

        private void GetBBWinner(ServiceContext dc)
        {
            try
            {
                List<zz_golfteam> golfTeams = (from gt in dc.zz_golfteamSet
                                               where gt.statecode == zz_golfteamState.Active
                                               select gt).ToList<zz_golfteam>();

                List<BestBallHelper> bbResults = new List<BestBallHelper>();

                foreach (zz_golfteam golfTeam in golfTeams)
                {
                    List<zz_hole> bbHoles = (from r in dc.zz_roundSet
                                             join h in dc.zz_holeSet on r.zz_roundId equals h.zz_RoundId.Id
                                             where r.zz_EventId.Id == EventId
                                             && r.zz_Day == s.osvGetDay(Day)
                                             && r.zz_GolfTeamId.Id == golfTeam.zz_golfteamId
                                             where h.zz_BestBall == true
                                             select h).ToList<zz_hole>();

                    if (bbHoles.Count == 18)
                    {
                        BestBallHelper bbh = new BestBallHelper();
                        bbh.BestBallScore = bbHoles.Sum(s => s.zz_Shots).Value;
                        bbh.golfteam = golfTeam;
                        zz_hole TieBreaker = bbHoles.Where(s => s.zz_BestBallTieBreaker == true).FirstOrDefault();
                        if (TieBreaker != null)
                        {
                            bbh.OwnsTieBreaker = true;
                            bbh.TieBreaker = TieBreaker;
                        }

                        bbResults.Add(bbh);
                    }
                }

                BestBallHelper bbWinner = bbResults.OrderBy(s => s.BestBallScore).ThenByDescending(r => r.OwnsTieBreaker).FirstOrDefault();
                if (bbWinner != null)
                {
                    WinnerText = bbWinner.golfteam.zz_name + " " + bbWinner.BestBallScore;
                    if (bbWinner.OwnsTieBreaker)
                        WinnerText = WinnerText + " *TB Hole #" + bbWinner.TieBreaker.zz_HoleNumber.ToString();
                }
            }
            catch
            {
                throw;
            }

        }

        private void GetTT2Winner(ServiceContext dc)
        {
            try
            {

                List<zz_golfteam> golfTeams = (from gt in dc.zz_golfteamSet
                                               where gt.statecode == zz_golfteamState.Active
                                               select gt).ToList<zz_golfteam>();

                List<zz_round> AllPlayedRounds = (from r in dc.zz_roundSet
                                                  where
                                                     r.zz_EventId.Id == EventId
                                                     && r.zz_EventStatus == s.osvEventStatus("Played")
                                                     && r.zz_CompletedHoles != 0
                                                  select r).ToList<zz_round>();
                int y = 0;
                zz_round MaxDay = new zz_round();
                foreach (zz_round mround in AllPlayedRounds.OrderByDescending(s => s.zz_Day.Value))
                {
                    if (y == 0)
                    {
                        MaxDay = mround;
                        break;
                    }
                }

                List<TeamTop2Helper> TT2_Results = new List<TeamTop2Helper>();
                foreach (zz_golfteam golfTeam in golfTeams)
                {

                    List<zz_round> Rounds = (
                                          from g in dc.zz_golferSet
                                          join r in dc.zz_roundSet on g.zz_golferId equals r.zz_GolferId.Id
                                          where
                                             g.zz_GolfTeamId.Id == golfTeam.zz_golfteamId
                                          where
                                              r.zz_Day == MaxDay.zz_Day
                                              && r.zz_EventStatus == s.osvEventStatus("Played")
                                              && r.zz_EventId.Id == EventId
                                          select r
                                        ).ToList<zz_round>();

                    int x = 0;
                    TeamTop2Helper tt2 = new TeamTop2Helper(golfTeam);

                    foreach (zz_round round in Rounds.OrderBy(s => s.zz_TotalScore))
                    {
                        if (x < 2)
                            tt2.Scores[x] = (int)round.zz_TotalScore;

                        zz_golfer eGolfer = (from g in dc.zz_golferSet where g.zz_golferId == round.zz_GolferId.Id select g).FirstOrDefault();
                        TeamTop2Golfer tGolfer = new TeamTop2Golfer((int)round.zz_CompletedHoles, (DateTime)round.zz_TeeTime);
                        tGolfer.golfer = eGolfer;

                        tt2.golfers.Add(tGolfer);

                        x++;
                    }

                    tt2.TotalScore = tt2.Scores[0] + tt2.Scores[1];
                    TT2_Results.Add(tt2);
                }

                TeamTop2Helper tt2Winner = TT2_Results.OrderBy(s => s.TotalScore).FirstOrDefault();
                WinnerText = tt2Winner.golfTeam.zz_name + " @ " + tt2Winner.TotalScore;

            }
            catch
            {
                throw;
            }

        }

        public void GetSkinsCount(ServiceContext dc)
        {
            try
            {
                int skinHoleCount = (from r in dc.zz_roundSet
                                     join h in dc.zz_holeSet on r.zz_roundId equals h.zz_RoundId.Id
                                     where r.zz_EventId.Id == EventId
                                     && r.zz_Day == s.osvGetDay(Day)
                                     where h.zz_Skins == true
                                     select h).ToList<zz_hole>().Count();
                count = skinHoleCount;
            }
            catch
            {
                throw;
            }

        }

        private void GetLeadingGolfer(ServiceContext dc)
        {
            try
            {
                List<zz_round> AllPlayedRounds = (from r in dc.zz_roundSet
                                                  where
                                                     r.zz_EventId.Id == EventId
                                                     && r.zz_EventStatus == s.osvEventStatus("Played")
                                                     && r.zz_CompletedHoles != 0
                                                  select r).ToList<zz_round>();
                int y = 0;
                zz_round MaxDay = new zz_round();
                foreach (zz_round mround in AllPlayedRounds.OrderByDescending(s => s.zz_Day.Value))
                {
                    if (y == 0)
                    {
                        MaxDay = mround;
                        break;
                    }
                }


                List<zz_round> Rounds = (
                                        from r in dc.zz_roundSet
                                        where
                                            r.zz_Day == MaxDay.zz_Day
                                            && r.zz_EventStatus == s.osvEventStatus("Played")
                                            && r.zz_EventId.Id == EventId
                                        select r
                                    ).ToList<zz_round>();


                zz_golfer golfer = new zz_golfer();
                zz_golfteam golfteam = new zz_golfteam();
                foreach (zz_round round in Rounds.OrderBy(s => s.zz_TotalScore).ThenBy(n => n.zz_CompletedHoles).ThenBy(t => t.zz_TeeTime))
                {
                    golfer = (from g in dc.zz_golferSet
                              where g.zz_golferId == round.zz_GolferId.Id
                              select g).FirstOrDefault();

                    golfteam = (from g in dc.zz_golfteamSet
                                where g.zz_golfteamId == round.zz_GolfTeamId.Id
                                select g).FirstOrDefault();
                    WinnerText = golfer.zz_name + "/" + golfteam.zz_name + " @ " + round.zz_TotalScore;
                    break;
                }
            }
            catch
            {
                throw;
            }

        }


    }
}