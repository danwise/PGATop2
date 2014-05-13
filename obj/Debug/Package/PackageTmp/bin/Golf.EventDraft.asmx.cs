namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Services;    
    using System.Web.Script.Services;
    using System.Web.Script.Serialization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Discovery;
    using Microsoft.Xrm.Sdk.Messages;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.Xml;
    using System.ServiceModel;

    using Microsoft.Crm.Sdk.Messages;

    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://www.dwise.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    public class WebService1 : System.Web.Services.WebService
    {

        private string _TargetCRMServer = "crm.calcsea.org"; //PROD
        private bool _TargetisHTTPS = true; //PROD
        private AuthenticationProviderType _authType = AuthenticationProviderType.Federation; //PROD
        private OrganizationServiceProxy service;
        private Guid _userid;
        private String _startPath = @"C:\inetpub\wwwroot\dwise.net";

 

        #region Dynamic Site Code

        [WebMethod]
        public EventXml HomePageLeaders(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);
                Shared shared = new Shared();
        
                EventAndLeague Event = (from e in datacontext.zz_eventSet
                         join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                         join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                         join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                         where e.statecode.Value == 0
                         where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                         select new EventAndLeague
                         {
                             EventId = e.zz_eventId.Value,
                             LeagueId = le.zz_leagueid.Value,
                             EventName = e.zz_name,
                             EventStatusCode = e.statuscode,
                             LeagueStatusCode = l.statuscode,
                             PlaySuspendedReason = e.zz_PlaySuspendedReason,
                             StartTime = e.zz_StartDate.Value
                         }).FirstOrDefault();

                //if (Event == null)
                //    Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                //zz_league league = (from l in datacontext.zz_leagueSet
                //                    join gt in datacontext.zz_golfteamSet on l.zz_leagueId.Value equals gt.zz_LeagueId.Id
                //                    where
                //                     gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                //                    select l
                //                     ).FirstOrDefault();

                EventXml LeaderBoard = new EventXml
                {
                    EventId = Event.EventId,
                    EventName = Event.EventName,
                    EventStatusName = shared.sGetEventStatus(Event.EventStatusCode), //should change this to be a league status instead of event
                    LeagueStatusName = shared.sGetEventStatus(Event.LeagueStatusCode), //should change this to be a league status instead of event
                    PlaySuspendedReason = Event.PlaySuspendedReason,
                };

                List<LeagueProduct> leagueProducts = (from lp in datacontext.zz_leagueproductSet
                                                      join p in datacontext.ProductSet on lp.zz_ProductId.Id equals p.ProductId
                                                      where
                                                          lp.zz_LeagueId.Id == Event.LeagueId
                                                      select new LeagueProduct
                                                      {
                                                          ProductName = p.Name,
                                                          ProductNumber = p.ProductNumber,
                                                          ProductId = p.ProductId.Value,
                                                          LeagueId = lp.zz_LeagueId.Id,
                                                          Amount = lp.zz_Amount.Value,
                                                          ProductDisplayOrder = (p.zz_DisplayOrder==null?99:p.zz_DisplayOrder.Value),
                                                      }
                                     ).ToList<LeagueProduct>();

                if (Event.LeagueStatusCode.Value != 419260000)
                {
                    EventDraft lastPick = (
                                            from r in datacontext.zz_eventdraftSet
                                            join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId
                                            join gt in datacontext.zz_golfteamSet on r.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                            join e in datacontext.zz_eventSet on r.zz_EventId.Id equals e.zz_eventId.Value
                                            where
                                                r.zz_EventId.Id == Event.EventId
                                                && r.statuscode != new OptionSetValue(419260000)
                                                && r.statecode == zz_eventdraftState.Active
                                                && r.zz_LeagueId.Id == Event.LeagueId
                                            select new EventDraft
                                            {
                                                EventDraftGolfer = g.zz_name,
                                                EventDraftEvent = e.zz_name,
                                                EventDraftPickNumber = r.zz_PickNumber.Value,
                                                EventDraftGolfTeam = gt.zz_name,
                                                EventDraftPickName = r.zz_name,
                                                EventDraftPickCompletedOn = r.ModifiedOn.Value,
                                                EventDraftPickDuration = FormatDuration(r.ModifiedOn.Value, r.CreatedOn.Value),
                                                EventDraftStatus = r.statuscode.Value
                                            }
                                        ).ToList<EventDraft>().OrderByDescending(ed => ed.EventDraftPickNumber).FirstOrDefault();

                    EventDraft otc = (
                                            from r in datacontext.zz_eventdraftSet
                                            join gt in datacontext.zz_golfteamSet on r.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                            join e in datacontext.zz_eventSet on r.zz_EventId.Id equals e.zz_eventId.Value
                                            where
                                                r.zz_EventId.Id == Event.EventId
                                                && r.statuscode == new OptionSetValue(419260000)
                                                && r.statecode == zz_eventdraftState.Active
                                                && r.zz_LeagueId.Id == Event.LeagueId
                                            select new EventDraft
                                            {
                                                EventDraftEvent = e.zz_name,
                                                EventDraftPickNumber = r.zz_PickNumber.Value,
                                                EventDraftGolfTeam = gt.zz_name,
                                                EventDraftPickName = r.zz_name,
                                                EventDraftPickCompletedOn = r.ModifiedOn.Value,
                                                EventDraftPickDuration = FormatDuration(DateTime.UtcNow, r.CreatedOn.Value),
                                                EventDraftStatus = r.statuscode.Value
                                            }
                                        ).FirstOrDefault();



                    if (otc != null)
                        LeaderBoard.GolfTeamOnTheClock  = otc.EventDraftGolfTeam;

                    if (lastPick != null)
                        LeaderBoard.LastDraftPick  = lastPick.EventDraftPickName;

                    LeagueProduct DraftRoom = new LeagueProduct
                    {
                        Leader = otc.EventDraftGolfTeam + " is on the clock",
                        ProductName = "Draft Room",
                        ProductNumber = "DraftRoom",
                        url = "/DraftRoom/Draft.html",
                    };
                    LeaderBoard.LeagueProducts.Add(DraftRoom);

                    LeagueProduct DraftResults = new LeagueProduct
                    {
                        Leader = (lastPick==null?"1st Pick OTC":lastPick.EventDraftPickName),
                        ProductName = "Draft Results",
                        ProductNumber = "DraftResults",
                        url = "DraftResults.htm",
                    };
                    LeaderBoard.LeagueProducts.Add(DraftResults);

                    
                }
                else
                {
                    //event has not started
                    if (Event.StartTime > DateTime.UtcNow)
                    {
                        LeagueProduct DraftResults = new LeagueProduct
                        {
                            Leader = "Draft Completed",
                            ProductName = "Draft Results",
                            ProductNumber = "DraftResults",
                            url = "DraftResults.htm",
                            ProductDisplayOrder = 0,
                        };
                        LeaderBoard.LeagueProducts.Add(DraftResults);

                    }
                    Round MaxRound = (from rnd in datacontext.zz_roundSet
                                      where
                                          rnd.zz_EventId.Id == Event.EventId
                                          && rnd.zz_EventStatus == shared.osvEventStatus("Played")
                                      orderby rnd.zz_Day
                                      select new Round { Day = rnd.zz_Day, Thru = rnd.zz_CompletedHoles.Value,DayNumber=shared.iGetDay(rnd.zz_Day)}
                        ).ToList<Round>().OrderByDescending(r => r.Day.Value).FirstOrDefault();

                    List<Golfer> allGolfers;
                    if (MaxRound != null)
                    {
                        allGolfers = (

                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                where
                                    ed.zz_EventId.Id == Event.EventId
                                    && ed.zz_LeagueId.Id == Event.LeagueId
                                where
                                    r.zz_Day == MaxRound.Day
                                    && r.zz_EventId.Id == Event.EventId
                                // && r.zz_EventStatus == shared.osvEventStatus("Played")
                                select new Golfer
                                {
                                    GolferName = g.zz_name,
                                    GolferGolfTeam = gt.zz_name,
                                    GolferScore = (r.zz_TotalScore == null ? 0 : r.zz_TotalScore.Value),
                                    HolesCompleted = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    RoundInProgress = r.zz_name,
                                    GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                    TeamPageUrl = "GolfTeam.htm?GolfTeamId=" + gt.zz_golfteamId.ToString(),
                                    GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.ToString(),
                                    GolferId = g.zz_golferId.Value,
                                    GolfTeamId = gt.zz_golfteamId.Value,
                                    NextTeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                    dtNextTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                    GolferStatus = shared.sGolferStatus(g.statuscode)
                                }
                            ).ToList<Golfer>();
                    }
                    else
                    {
                        allGolfers = (from r in datacontext.zz_eventdraftSet
                                      join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId
                                      join gt in datacontext.zz_golfteamSet on r.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                      where
                                          r.zz_EventId.Id == Event.EventId
                                          && r.zz_LeagueId.Id == Event.LeagueId
                                      where
                                           g.statecode == zz_golferState.Active
                                      select new Golfer
                                      {
                                          TotalScore = (g.zz_TotalScore == null ? 0 : g.zz_TotalScore.Value),
                                          GolferName = g.zz_name,
                                          GolferGolfTeam = gt.zz_name,
                                          GolfTeamId = (gt.zz_golfteamId == null ? Guid.Empty : gt.zz_golfteamId.Value),
                                          LastUpdate = g.ModifiedOn.Value
                                      }).ToList<Golfer>();
                    }

                        #region Leaderboard
                        //only show after the 1st round has been created.
                        if (MaxRound != null)
                        {
                            Golfer Leader = allGolfers.OrderBy(gl => gl.GolferScore).OrderByDescending(gl => gl.LastUpdate).FirstOrDefault();
                            if (Leader != null)
                                LeaderBoard.GolferLeaderboard = Leader.GolferName + " @ " + Leader.GolferScore.ToString();

                            LeagueProduct Leaderboard = new LeagueProduct
                            {
                                Leader = LeaderBoard.GolferLeaderboard,
                                ProductName = "Leaderboard",
                                ProductNumber = "Leaderboard",
                                url = "Leaderboard.htm",
                            };
                            LeaderBoard.LeagueProducts.Add(Leaderboard);
                        }
                        #endregion

                        #region BestBall
                        List<Hole> BestBallHoles= new List<Hole>();
                        List<GolfTeam> BBTeams = new List<GolfTeam>();
                        if (leagueProducts.Where(lp => lp.ProductNumber.Contains("BB") == true).Count() > 0)
                        {
                            BestBallHoles = (from gtbbh in datacontext.zz_golfteambestballholeSet
                                                        join h in datacontext.zz_holeSet on gtbbh.zz_HoleId.Id equals h.zz_holeId.Value
                                                        join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                                        join r in datacontext.zz_roundSet on h.zz_RoundId.Id equals r.zz_roundId.Value
                                                        join gt in datacontext.zz_golfteamSet on gtbbh.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                        where
                                                          r.zz_EventId.Id == Event.EventId
                                                          //&& //Thursday or Friday Only 
                                                          //(
                                                          //    r.zz_Day.Value == 419260000
                                                          //    || r.zz_Day.Value == 419260001
                                                          // )
                                                        where
                                                          gt.zz_LeagueId.Id == Event.LeagueId
                                                        select new Hole
                                                        {
                                                            Shots = h.zz_Shots.Value,
                                                            Par = ch.zz_Par.Value,
                                                            HoleGolfTeam = gt.zz_name,
                                                            GolfTeamId = gt.zz_golfteamId.Value,
                                                            HoleDay = r.zz_Day,
                                                            BestBallTieBreaker = (gtbbh.zz_TieBreaker == null ? false : gtbbh.zz_TieBreaker.Value)
                                                        }).ToList<Hole>();

                            BBTeams = BestBallHoles.GroupBy(s => new { s.GolfTeamId, s.HoleGolfTeam }).Select(y => new GolfTeam()
                            {
                                GolfTeamId = y.Key.GolfTeamId,
                                GolfTeamName = y.Key.HoleGolfTeam,
                            }).ToList<GolfTeam>();

                        }
                        #endregion

                        #region Top4
                        List<Round> allRounds = new List<Round>();
                        if (leagueProducts.Where(lp => lp.ProductNumber.Contains("TT4") == true).Count() > 0)
                        {
                            allRounds = (
                                     from ed in datacontext.zz_eventdraftSet
                                     join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                     join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                     join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                     where
                                         ed.zz_EventId.Id == Event.EventId
                                         && ed.zz_LeagueId.Id == Event.LeagueId
                                     where
                                         r.zz_EventId.Id == Event.EventId
                                     select new Round
                                     {
                                         GolferId = g.zz_golferId.Value,
                                         RoundGolferName = g.zz_name,
                                         RoundGolfTeamId = gt.zz_golfteamId.Value,
                                         RoundGolfTeamName = gt.zz_name,
                                         Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                         RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                         RoundShots = (r.zz_Shots == null ? 0 : r.zz_Shots.Value),
                                         DayNumber = shared.iGetDay(r.zz_Day),
                                         dtTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                         TeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                         RoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                                         RoundName = r.zz_name,
                                         TournamentRank = r.zz_EventRank,
                                         RoundGolferStatus = shared.sGolferStatus(g.statuscode),

                                         //HolesCompleted = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                         //Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                         //RoundInProgress = r.zz_name,
                                         //GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                         //TeamPageUrl = "GolfTeam.htm?GolfTeamId=" + gt.zz_golfteamId.ToString(),
                                         //GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.ToString(),
                                         //GolferId = g.zz_golferId.Value,
                                         //GolfTeamId = gt.zz_golfteamId.Value,
                                         //NextTeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                         //dtNextTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                         //GolferStatus = shared.sGolferStatus(g.statuscode),
                                         //RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                         //TodaysRoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                                     }).ToList<Round>();
                        }
                        #endregion

                        #region Skins
                        List<Hole> SkinHoles = new List<Hole>();
                        if (leagueProducts.Where(lp => lp.ProductNumber.Contains("Skins") == true).Count() > 0)
                        {
                            SkinHoles = (from ls in datacontext.zz_leagueskinSet
                                         join h in datacontext.zz_holeSet on ls.zz_HoleId.Id equals h.zz_holeId.Value
                                         join r in datacontext.zz_roundSet on h.zz_RoundId.Id equals r.zz_roundId.Value
                                         //join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId.Value
                                         //join ed in datacontext.zz_eventdraftSet on g.zz_golferId equals ed.zz_GolferId.Id
                                         join gt in datacontext.zz_golfteamSet on ls.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                         where
                                           ls.zz_LeagueId.Id == Event.LeagueId
                                         where
                                                      r.zz_EventId.Id == Event.EventId
                                         //where
                                         //    ed.zz_EventId.Id == Event.EventId
                                         //    && ed.zz_LeagueId.Id == Event.LeagueId
                                         select new Hole
                                         {
                                             HoleId = h.zz_holeId.Value,
                                             HoleNumber = h.zz_HoleNumber.Value,
                                             Shots = h.zz_Shots.Value,
                                             HoleDay = r.zz_Day,
                                             HoleGolfTeam = gt.zz_name,
                                             GolfTeamId = gt.zz_golfteamId.Value,
                                         }
                                               ).ToList<Hole>();
                        }
                        #endregion

                        #region 36hole
                            List<Round> ThirySixRounds = new List<Round>();

                            if (leagueProducts.Where(lp => lp.ProductNumber.Contains("36") == true).Count() > 0)
                            {
                                ThirySixRounds = (
                                                from d in datacontext.zz_eventdraftSet
                                                join gt in datacontext.zz_golfteamSet on d.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                join g in datacontext.zz_golferSet on d.zz_GolferId.Id equals g.zz_golferId
                                                join r in datacontext.zz_roundSet on g.zz_golferId equals r.zz_GolferId.Id
                                                where
                                                    d.zz_EventId.Id == Event.EventId
                                                    && d.zz_LeagueId.Id == Event.LeagueId
                                                where
                                                    r.zz_Day == shared.osvGetDay(2) //Friday Round Only
                                                    && r.zz_EventStatus == shared.osvEventStatus("Played")
                                                    && r.zz_EventId.Id == Event.EventId
                                                select new Round
                                                {
                                                    RoundGolfTeamId = d.zz_GolfTeamId.Id,
                                                    RoundGolfTeamName = gt.zz_name,
                                                    GolferId = g.zz_golferId.Value,
                                                    RoundGolferName = g.zz_name,
                                                    TotalScore = r.zz_TotalScore.Value,
                                                }).ToList<Round>();
                            }
                        #endregion

                    foreach (LeagueProduct lp in leagueProducts.OrderBy(lp=>lp.ProductDisplayOrder))
                    {
                        switch (lp.ProductNumber)
                        {
                            case "TT2":
                                if (MaxRound != null)
                                {
                                    List<NumberRank> Top2 = new List<NumberRank>();
                                    int MaxTeamTop2Score = 0;
                                    bool AllZero = true;
                                    string TeamTop2Leader = "";
                                    //allGolfers.GroupBy(g => g.GolfTeamId).ToList().ForEach(gt =>
                                    List<GolfTeam> T2Teams = allGolfers.GroupBy(s => new { s.GolfTeamId, s.GolferGolfTeam }).Select(y => new GolfTeam()
                                     {
                                         GolfTeamId = y.Key.GolfTeamId,
                                         GolfTeamName = y.Key.GolferGolfTeam,
                                         ActiveGolfers = y.Count(),
                                     }).ToList();

                                    T2Teams.ForEach(gt =>
                                    {
                                        int TeamTop2Score;
                                        switch (gt.ActiveGolfers)
                                        {
                                            case 1:
                                                TeamTop2Score = 99;
                                                break;
                                            case 2:
                                                //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                                                Top2 = allGolfers.Where(ag => ag.GolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                                                TeamTop2Score = Top2.Sum(s => s.Number);
                                                break;
                                            case 3:
                                                //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                                                Top2 = allGolfers.Where(ag => ag.GolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                                                TeamTop2Score = Top2.Sum(s => s.Number);
                                                break;
                                            default:
                                                //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                                                Top2 = allGolfers.Where(ag => ag.GolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                                                TeamTop2Score = Top2.Sum(s => s.Number);
                                                break;
                                        }

                                        gt.TeamTop2Score = TeamTop2Score;
                                        if (MaxTeamTop2Score > TeamTop2Score)
                                        {
                                            MaxTeamTop2Score = TeamTop2Score;
                                            AllZero = false;
                                        }
                                    });

                                    int LeaderCount = T2Teams.Where(gt => gt.TeamTop2Score == MaxTeamTop2Score).Count();
                                    if (LeaderCount > 1)
                                    {
                                        TeamTop2Leader = LeaderCount.ToString() + " tied @ " + MaxTeamTop2Score;
                                    }
                                    else
                                    {
                                        GolfTeam leader = T2Teams.Where(gt => gt.TeamTop2Score == MaxTeamTop2Score).FirstOrDefault();
                                        TeamTop2Leader = leader.GolfTeamName + " @ " + leader.TeamTop2Score.ToString();
                                    }

                                    if (TeamTop2Leader == "" && AllZero)
                                        TeamTop2Leader = "All Square";

                                    LeaderBoard.TeamTop2Leader = TeamTop2Leader;

                                    LeagueProduct tt2 = new LeagueProduct
                                    {
                                        Leader = TeamTop2Leader,
                                        ProductName = lp.ProductName,
                                        ProductNumber = lp.ProductNumber,
                                        url = "teamTop2.htm",
                                    };

                                    LeaderBoard.LeagueProducts.Add(tt2);
                                }
                                break;
                            case "TT4":
                                if (MaxRound != null)
                                {
                                    List<GolfTeam> Teams = allRounds.GroupBy(s => new { s.RoundGolfTeamId, s.RoundGolfTeamName }).Select(y => new GolfTeam()
                                    {
                                        GolfTeamId = y.Key.RoundGolfTeamId,
                                        GolfTeamName = y.Key.RoundGolfTeamName,
                                    }).ToList<GolfTeam>();

                                    List<Golfer> Golfers = allRounds.GroupBy(s => new { s.GolferId, s.RoundGolferName, s.RoundGolfTeamId, s.RoundGolferStatus }).Select(y => new Golfer()
                                    {
                                        GolferId = y.Key.GolferId,
                                        GolferName = y.Key.RoundGolferName,
                                        GolfTeamId = y.Key.RoundGolfTeamId,
                                        GolferStatus = y.Key.RoundGolferStatus,
                                    }).ToList<Golfer>();

                                    int MaxDay = allRounds.Max(al => al.DayNumber);
                                    int MaxTop4Score = 99;
                                    bool AllZero = true;
                                    string Top4Leader = "";
                                    Golfers.ForEach(g =>
                                        {
                                            g.Rounds = allRounds.Where(al => al.GolferId == g.GolferId).ToList();
                                            int MaxScore = 0;
                                            Round tt4MaxRound;
                                            //the golfer has less rounds then the max
                                            if (g.Rounds.Count() != MaxDay)
                                            {
                                                switch (MaxDay)
                                                {
                                                    case 1:
                                                        //this would be where you would add the max round score.
                                                        break;
                                                    case 2:
                                                        break;
                                                    case 3:
                                                        //Missed cut golfers, get thier highest round
                                                        MaxScore = g.Rounds.Max(r => r.RoundShots);
                                                        MaxRound = g.Rounds.Where(r => r.RoundShots == MaxScore).FirstOrDefault();
                                                        g.Rounds.Add(MaxRound);
                                                        break;
                                                    case 4:
                                                        //Missed cut add two rounds of max score
                                                        MaxScore = g.Rounds.Max(r => r.RoundShots);
                                                        MaxRound = g.Rounds.Where(r => r.RoundShots == MaxScore).FirstOrDefault();
                                                        g.Rounds.Add(MaxRound);
                                                        g.Rounds.Add(MaxRound);
                                                        break;
                                                }
                                            }
                                            g.GolferScore = g.Rounds.Sum(r => r.RoundScore);


                                            Round CurrentRound = g.Rounds.OrderByDescending(r => r.DayNumber).FirstOrDefault();

                                            g.Thru = CurrentRound.Thru;
                                            g.RoundScore = CurrentRound.RoundScore;
                                            g.dtNextTeeTime = CurrentRound.dtTeeTime;
                                            g.NextTeeTime = CurrentRound.TeeTime;
                                            g.TodaysRoundStarted = CurrentRound.RoundStarted;
                                            g.RoundInProgress = CurrentRound.RoundName;
                                            g.TournamentRank = CurrentRound.TournamentRank;

                                        });

                                    Teams.ForEach(t =>
                                        {
                                            t.Golfers = Golfers.Where(g => g.GolfTeamId == t.GolfTeamId).ToList();
                                            t.Top4Score = t.Golfers.Sum(g => g.GolferScore);

                                            if (MaxTop4Score > t.Top4Score)
                                            {
                                                MaxTop4Score = t.Top4Score;
                                                Top4Leader = t.GolfTeamName + " @ " + t.Top4Score.ToString();
                                                AllZero = false;
                                            }
                                        });

                                    if (Top4Leader == "" && AllZero)
                                        Top4Leader = "All Square";

                                    LeagueProduct TT4 = new LeagueProduct
                                       {
                                           Leader = Top4Leader,
                                           ProductName = lp.ProductName,
                                           ProductNumber = lp.ProductNumber,
                                           url = "Top4.htm",
                                       };
                                    LeaderBoard.LeagueProducts.Add(TT4);
                                }
                                break;
                            case "SkinsSun":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 4)
                                    {
                                        int iSkinsSun = 0;
                                        iSkinsSun = SkinHoles.Where(sk => sk.HoleDay.Value == 419260003).Count();

                                        List<GolfTeam> SkinSunTeams = SkinHoles.Where(sk => sk.HoleDay.Value == 419260003).GroupBy(s => new { s.GolfTeamId, s.HoleGolfTeam }).Select(y => new GolfTeam()
                                        {
                                            GolfTeamId = y.Key.GolfTeamId,
                                            GolfTeamName = y.Key.HoleGolfTeam,
                                            SkinCount_Sat = y.Count(),
                                        }).ToList<GolfTeam>();

                                        string SkinList = "";
                                        SkinSunTeams.ForEach(s =>
                                        {
                                            if (SkinList == "")
                                                SkinList = s.GolfTeamName + ":" + s.SkinCount_Sat;
                                            else
                                                SkinList = SkinList + ", " + s.GolfTeamName + ":" + s.SkinCount_Sat;
                                        });


                                        LeaderBoard.SkinsCount_Sun = iSkinsSun;
                                        LeagueProduct SkinsSun = new LeagueProduct
                                        {
                                            Leader = SkinList,
                                            Count = iSkinsSun,
                                            ProductName = lp.ProductName,
                                            ProductNumber = lp.ProductNumber,
                                            url = "Skins.htm?Day=Sun",
                                        };
                                        LeaderBoard.LeagueProducts.Add(SkinsSun);
                                    }
                                }
                                break;
                            case "SkinsSat":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 3)
                                    {
                                        int iSkinsSat = 0;
                                        iSkinsSat = SkinHoles.Where(sk => sk.HoleDay.Value == 419260002).Count();

                                        List<GolfTeam> SkinSatTeams = SkinHoles.Where(sk => sk.HoleDay.Value == 419260002).GroupBy(s => new { s.GolfTeamId, s.HoleGolfTeam }).Select(y => new GolfTeam()
                                        {
                                            GolfTeamId = y.Key.GolfTeamId,
                                            GolfTeamName = y.Key.HoleGolfTeam,
                                            SkinCount_Sat = y.Count(),
                                        }).ToList<GolfTeam>();

                                        string SkinList = "";
                                        SkinSatTeams.ForEach(s =>
                                            {
                                                if (SkinList == "")
                                                    SkinList = s.GolfTeamName + ":" + s.SkinCount_Sat;
                                                else
                                                    SkinList = SkinList + ", " + s.GolfTeamName + ":" + s.SkinCount_Sat;
                                            });
                                        LeaderBoard.SkinsCount_Sat = iSkinsSat;
                                        LeagueProduct SkinsSat = new LeagueProduct
                                        {
                                            Leader = SkinList,
                                            Count = iSkinsSat,
                                            ProductName = lp.ProductName,
                                            ProductNumber = lp.ProductNumber,
                                            url = "Skins.htm?Day=Sat",
                                        };
                                        LeaderBoard.LeagueProducts.Add(SkinsSat);
                                    }
                                }
                                break;
                            case "36Ind":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 2)
                                    {
                                        Round thirtySixLeader = ThirySixRounds.OrderBy(r => r.TotalScore).FirstOrDefault();
                                        if (thirtySixLeader != null)
                                        {
                                            LeagueProduct thirtySix = new LeagueProduct
                                            {
                                                Leader = thirtySixLeader.RoundGolferName + " @ " + thirtySixLeader.TotalScore,
                                                ProductName = lp.ProductName,
                                                ProductNumber = lp.ProductNumber,
                                                url = "#",
                                            };
                                            LeaderBoard.LeagueProducts.Add(thirtySix);
                                        }
                                    }
                                }
                                break;
                            case "36TT2":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 2)
                                    {

                                        //ThirySixRounds
                                        
                                        List<NumberRank> Top236 = new List<NumberRank>();
                                        int MaxTeamTop236Score = 0;
                                        bool AllZero = true;
                                        string TeamTop236Leader = "";
                                        //allGolfers.GroupBy(g => g.GolfTeamId).ToList().ForEach(gt =>
                                        List<GolfTeam> T236Teams = ThirySixRounds.GroupBy(s => new { s.RoundGolfTeamId, s.RoundGolfTeamName }).Select(y => new GolfTeam()
                                        {
                                            GolfTeamId = y.Key.RoundGolfTeamId,
                                            GolfTeamName = y.Key.RoundGolfTeamName,
                                            ActiveGolfers = y.Count(),
                                        }).ToList();

                                        T236Teams.ForEach(gt =>
                                        {
                                            int TeamTop2Score;
                                            switch (gt.ActiveGolfers)
                                            {
                                                case 1:
                                                    TeamTop2Score = 99;
                                                    break;
                                                case 2:
                                                    //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                                                    Top236 = ThirySixRounds.Where(ag => ag.RoundGolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.TotalScore).Select((n, i) => new NumberRank(n.TotalScore, i + 1)).ToList().ToList<NumberRank>();
                                                    TeamTop2Score = Top236.Sum(s => s.Number);
                                                    break;
                                                case 3:
                                                    //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                                                    Top236 = ThirySixRounds.Where(ag => ag.RoundGolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.TotalScore).Select((n, i) => new NumberRank(n.TotalScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                                                    TeamTop2Score = Top236.Sum(s => s.Number);
                                                    break;
                                                default:
                                                    //Top2 = gt.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                                                    Top236 = ThirySixRounds.Where(ag => ag.RoundGolfTeamId == gt.GolfTeamId).ToList().OrderByDescending(n => n.TotalScore).Select((n, i) => new NumberRank(n.TotalScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                                                    TeamTop2Score = Top236.Sum(s => s.Number);
                                                    break;
                                            }

                                            gt.TeamTop2Score = TeamTop2Score;
                                            if (MaxTeamTop236Score > TeamTop2Score)
                                            {
                                                MaxTeamTop236Score = TeamTop2Score;
                                                AllZero = false;
                                            }
                                        });

                                        int LeaderCount = T236Teams.Where(gt => gt.TeamTop2Score == MaxTeamTop236Score).Count();
                                        if (LeaderCount > 1)
                                        {
                                            TeamTop236Leader = LeaderCount.ToString() + " tied @ " + MaxTeamTop236Score;
                                        }
                                        else
                                        {
                                            GolfTeam leader = T236Teams.Where(gt => gt.TeamTop2Score == MaxTeamTop236Score).FirstOrDefault();
                                            TeamTop236Leader = leader.GolfTeamName + " @ " + leader.TeamTop2Score.ToString();
                                        }

                                        if (TeamTop236Leader == "" && AllZero)
                                            TeamTop236Leader = "All Square";

                                        //LeaderBoard.TeamTop2Leader = TeamTop2Leader;

                                        LeagueProduct thirtySix = new LeagueProduct
                                        {
                                            Leader = TeamTop236Leader,
                                            ProductName = lp.ProductName,
                                            ProductNumber = lp.ProductNumber,
                                            url = "#",
                                        };
                                        LeaderBoard.LeagueProducts.Add(thirtySix);


                                    }
                                }
                                break;
                            case "BB36":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 1)
                                    {
                                        string sBestBallLeader_36 = "";
                                        BBTeams.ForEach(gt =>
                                        {
                                            gt.BestBallScore_36 = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId).Sum(bbh => bbh.Shots) - BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId).Sum(bbh => bbh.Par);
                                            int bbhTB_36 = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.BestBallTieBreaker == true).ToList().Count();
                                            if (bbhTB_36 != 0)
                                                gt.BestBallTieBreaker_36 = true;
                                        });

                                        GolfTeam BestBallLeader_36 = BBTeams.OrderBy(gt => gt.BestBallScore_36).OrderByDescending(gt => gt.BestBallTieBreaker_36).FirstOrDefault();

                                        if (BestBallLeader_36 != null)
                                            sBestBallLeader_36 = BestBallLeader_36.GolfTeamName + " @ " + BestBallLeader_36.BestBallScore_36 + (BestBallLeader_36.BestBallTieBreaker_36 == true ? " *TB" : "");

                                        //LeaderBoard.BestBallLeader_Fri = sBestBallLeader_36;
                                        LeagueProduct BB36 = new LeagueProduct
                                        {
                                            Leader = sBestBallLeader_36,
                                            ProductName = lp.ProductName,
                                            ProductNumber = lp.ProductNumber,
                                            url = "BestBall.htm?Day=36",
                                        };
                                        LeaderBoard.LeagueProducts.Add(BB36);
                                    }
                                }
                                break;
                            case "BBFr":
                                if (MaxRound != null)
                                {
                                    if (MaxRound.DayNumber >= 2)
                                    {
                                        string sBestBallLeader_Fri = "";
                                        BBTeams.ForEach(gt =>
                                        {
                                            gt.BestBallScore_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001).Sum(bbh => bbh.Shots) - BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001).Sum(bbh => bbh.Par);
                                            int bbhTB_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.BestBallTieBreaker == true).ToList().Count();
                                            if (bbhTB_Fri != 0)
                                                gt.BestBallTieBreaker_Fri = true;
                                        });

                                        GolfTeam BestBallLeader_Fri = BBTeams.OrderBy(gt => gt.BestBallScore_Fri).OrderByDescending(gt => gt.BestBallTieBreaker_Fri).FirstOrDefault();

                                        if (BestBallLeader_Fri != null)
                                            sBestBallLeader_Fri = BestBallLeader_Fri.GolfTeamName + " @ " + BestBallLeader_Fri.BestBallScore_Fri + (BestBallLeader_Fri.BestBallTieBreaker_Fri == true ? " *TB" : "");

                                        LeaderBoard.BestBallLeader_Fri = sBestBallLeader_Fri;
                                        LeagueProduct BBFri = new LeagueProduct
                                        {
                                            Leader = sBestBallLeader_Fri,
                                            ProductName = lp.ProductName,
                                            ProductNumber = lp.ProductNumber,
                                            url = "BestBall.htm?Day=Fri",
                                        };
                                        LeaderBoard.LeagueProducts.Add(BBFri);
                                    }
                                }
                                break;
                            case "BBTh":
                                if (MaxRound != null)
                                {
                                    string sBestBallLeader_Thurs = "";

                                    BBTeams.ForEach(gt =>
                                    {
                                        gt.BestBallScore_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000).Sum(bbh => bbh.Shots) - BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000).Sum(bbh => bbh.Par);
                                        int bbhTB_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.BestBallTieBreaker == true).ToList().Count();
                                        if (bbhTB_Thurs != 0)
                                            gt.BestBallTieBreaker_Thurs = true;
                                    });

                                    GolfTeam BestBallLeader_Thurs = BBTeams.OrderBy(gt => gt.BestBallScore_Thurs).OrderByDescending(gt => gt.BestBallTieBreaker_Thurs).FirstOrDefault();
                                    if (BestBallLeader_Thurs != null)
                                        sBestBallLeader_Thurs = BestBallLeader_Thurs.GolfTeamName + " @ " + BestBallLeader_Thurs.BestBallScore_Thurs + (BestBallLeader_Thurs.BestBallTieBreaker_Thurs == true ? " *TB" : "");
                                    else
                                        sBestBallLeader_Thurs = "Event Not Started";

                                    LeaderBoard.BestBallLeader_Thurs = sBestBallLeader_Thurs;
                                    LeagueProduct BBTh = new LeagueProduct
                                    {
                                        Leader = sBestBallLeader_Thurs,
                                        ProductName = lp.ProductName,
                                        ProductNumber = lp.ProductNumber,
                                        url = "BestBall.htm?Day=Thurs",
                                    };

                                    LeaderBoard.LeagueProducts.Add(BBTh);
                                }
                                break;
                        }
                    }
                }

                //EventXml LeaderBoard = new EventXml
                //{
                //    EventId = Event.zz_eventId.Value,
                //    EventName = Event.zz_name,
                //    LastDraftPick = sLastDraftPick,
                //    GolfTeamOnTheClock = sOnTheClock,
                //    GolferLeaderboard = sGolferLeaderboard,
                //    TeamTop2Leader = sTeamTop2Leader,
                //    BestBallLeader_Thurs = sBestBallLeader_Thurs,
                //    BestBallLeader_Fri = sBestBallLeader_Fri,
                //    SkinsCount_Sat = iSkinsCount_Sat,
                //    SkinsCount_Sun = iSkinsCount_Sun,
                //    EventStatusName = shared.sGetEventStatus(Event.statuscode), //should change this to be a league status instead of event
                //    LeagueStatusName = shared.sGetEventStatus(league.statuscode), //should change this to be a league status instead of event
                //    //SkinsAmount = (league.zz_Skins == null?0:league.zz_Skins.Value),
                //    //BestBallAmount = (league.zz_BestBall == null ? 0 : league.zz_BestBall.Value),
                //    PlaySuspendedReason = Event.zz_PlaySuspendedReason,
                //};

                //TODO:needs to be converted to league logic 
                #region Scorecards 
                if (Event != null)
                {
                    for (int x = 1; x < 5; x++)
                    {
                        string Day = "";
                        String DayName = "";
                        switch (x)
                        {
                            case 1:
                                Day = "419260000";
                                DayName = "Thursday";
                                break;
                            case 2:
                                Day = "419260001";
                                DayName = "Friday";
                                break;
                            case 3:
                                DayName = "Saturday";
                                Day = "419260002";
                                break;
                            case 4:
                                DayName = "Sunday";
                                Day = "419260003";
                                break;
                        }
                        string path = "GolferScorecards_" + Event.EventId.ToString() + "_" + Day + "_" + Event.LeagueId + ".htm";
                        string url = "../puScorecards.htm?EventId=" + Event.EventId.ToString() + "&Day=" + Day + "&LeagueId=" + Event.LeagueId;
                        string fileName = Server.MapPath("~/scorecards/" + path);
                        if (System.IO.File.Exists(fileName))
                        {
                            Scorecard s = new Scorecard();
                            s.Day = int.Parse(Day);
                            s.DayName = DayName;
                            s.url = url;
                            LeaderBoard.Scorecards.Add(s);
                        }
                    }
                }
                #endregion
                return LeaderBoard;
            }

        }

        [WebMethod]
        public List<Golfer> Leaderboard(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            List<Golfer> rtn = new List<Golfer>();
            Shared shared = new Shared();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                            CutLine = (e.zz_CutLine==null?0:e.zz_CutLine.Value),

                                            //EventName = e.zz_name,
                                            //EventStatusCode = e.statuscode,
                                            //LeagueStatusCode = l.statuscode,
                                            //PlaySuspendedReason = e.zz_PlaySuspendedReason,
                                            //StartTime = e.zz_StartDate.Value
                                        }).FirstOrDefault();

                Round MaxRound = (from rnd in datacontext.zz_roundSet
                                  where
                                      rnd.zz_EventId.Id == Event.EventId
                                      && rnd.zz_EventStatus == shared.osvEventStatus("Played")
                                  orderby rnd.zz_Day
                                  select new Round { Day = rnd.zz_Day, Thru = rnd.zz_CompletedHoles.Value , DayNumber = shared.iGetDay(rnd.zz_Day)}
                    ).ToList<Round>().OrderByDescending(r => r.Day.Value).FirstOrDefault();

                
                
                //List<NumberRank> md = rounds.OrderByDescending(n => n.Day).Select((n, i) => new NumberRank(n.Day.Value, i + 1)).ToList().Where(f => f.Rank == 1).ToList<NumberRank>();
                List<Golfer> Golfers = new List<Golfer>();
                if (MaxRound != null)
                {
                   List<Golfer>  AllGolfers = (
                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                where
                                    ed.zz_LeagueId.Id == Event.LeagueId
                                    && ed.zz_EventId.Id == Event.EventId
                                where
                                    r.zz_EventId.Id == Event.EventId
                                    //&& r.zz_Day == MaxRound.Day
                                select new Golfer
                                {
                                    GolferId = g.zz_golferId.Value,
                                    GolferName = g.zz_name,
                                    GolferGolfTeam = gt.zz_name,
                                    GolfTeamId = gt.zz_golfteamId.Value,
                                    GolferScore = (r.zz_TotalScore == null ? 0 : r.zz_TotalScore.Value),
                                    TotalScore = (r.zz_TotalScore == null ? 0 : r.zz_TotalScore.Value),
                                    DraftPickNumber = ( ed.zz_PickNumber == null?0:ed.zz_PickNumber.Value),
                                    GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                    NextTeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                    dtNextTeeTime = (r.zz_TeeTime == null ? DateTime.Now : r.zz_TeeTime.Value.ToLocalTime()),
                                    HolesCompleted = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    GolferStatus = (g.statuscode == null ? "Active" : shared.sGolferStatus(g.statuscode)),
                                    RoundInProgress = r.zz_name,
                                    RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                    TodaysRoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                                    TournamentRank = (r.zz_EventRank == null ? "" : r.zz_EventRank),
                                    CutLineRank = (r.zz_EventRank == null ? 99 : 1), //int.Parse(r.zz_EventRank.Replace("T", ""))),
                                    DayNumber = (r.zz_Day == null ? 0 : shared.iGetDay(r.zz_Day)),
                                    isCutLine = false,
                                    //// ShotTrackerUrl = (g.zz_pgatourId ==null?"":"http://www.pgatour.com/content/pgatour/shottracker.html#/current/r000/1/player/" + g.zz_pgatourId + "/"),
                                    ////ShotTrackerUrl =  (g.zz_pgatourId ==null?"":"http://www.legacy.pgatour.com/shottracker.html#/current/r000/1/player/" + g.zz_pgatourId + "/"),
                                    GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.Value.ToString() + "&GolfTeamId=" + gt.zz_golfteamId.Value.ToString(),
                                }
                            ).ToList<Golfer>().OrderByDescending(r=>r.DayNumber).ToList();
                   
                    AllGolfers.ForEach(g =>
                       {
                           if (g.DayNumber == MaxRound.DayNumber)
                               Golfers.Add(g);
                           else
                           {
                               Golfer glf = Golfers.Where(gf => gf.GolferId == g.GolferId).FirstOrDefault();
                               if (glf == null)
                                   Golfers.Add(g);
                           }
                       });
                }
                else
                {
                    Golfers = (
                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                where
                                    ed.zz_LeagueId.Id == Event.LeagueId
                                    && ed.zz_EventId.Id == Event.EventId
                                select new Golfer
                                {
                                    GolferId = g.zz_golferId.Value,
                                    GolferName = g.zz_name,
                                    GolferGolfTeam = gt.zz_name,
                                    GolferScore = 0,
                                    DraftPickNumber = ed.zz_PickNumber.Value,
                                    GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                    NextTeeTime = "",
                                    HolesCompleted = 0,
                                    GolferStatus = shared.sGolferStatus(g.statuscode),
                                    TodaysRoundStarted = false,
                                    ShotTrackerUrl = "#",
                                }
                            ).ToList<Golfer>();
                }

                if (MaxRound != null)
                {
                    //if (MaxRound.DayNumber == 2 && Event.CutLine != 0) //Only show cut line on Friday
                    if (MaxRound.DayNumber == 2) //Only show cut line on Friday
                    {
                        int CutLineScore = Golfers.Where(g => g.CutLineRank <= 71).Max(g => g.TotalScore);
                        Golfer CutLine = new Golfer
                        {
                            GolferName = "Projected",
                            TotalScore = CutLineScore,
                            GolferScore = CutLineScore,
                            GolferStatus = "Active",
                            GolferImgUrl = "http://www.dwise.net/images/Cutline2.jpg",
                            DraftPickNumber = 0,
                            isCutLine = true,
                            CutLineRank = 71, //pga rule is 78, some events are no cut need to account for that.
                            TournamentRank = "70",
                            GolferPageUrl = "#",
                        };
                        Golfers.Add(CutLine);
                    }
                }

                rtn = Golfers.OrderBy(gl => gl.DraftPickNumber).OrderBy(gl => gl.dtNextTeeTime).OrderBy(gl => gl.CutLineRank).OrderBy(gl => gl.GolferScore).OrderBy(gl => gl.GolferStatus).ToList();
            }
            return rtn;
        }

        [WebMethod]
        public List<GolfTeam> TeamTop2(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                        }).FirstOrDefault();


                Round MaxRound = (from rnd in datacontext.zz_roundSet
                                  where
                                      rnd.zz_EventId.Id == Event.EventId
                                      && rnd.zz_EventStatus == shared.osvEventStatus("Played")
                                  orderby rnd.zz_Day
                                  select new Round { Day = rnd.zz_Day, Thru = rnd.zz_CompletedHoles.Value }
                    ).ToList<Round>().OrderByDescending(r => r.Day.Value).FirstOrDefault();

                List<Golfer> Golfers;
                if (MaxRound != null)
                {
                    Golfers = (

                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                where
                                    ed.zz_EventId.Id == Event.EventId
                                    && ed.zz_LeagueId.Id == Event.LeagueId
                                where
                                    r.zz_Day == MaxRound.Day
                                    && r.zz_EventId.Id == Event.EventId
                                   // && r.zz_EventStatus == shared.osvEventStatus("Played")
                                select new Golfer
                                {
                                    GolferName = g.zz_name,
                                    GolferGolfTeam = gt.zz_name,
                                    GolferScore = (r.zz_TotalScore==null?0:r.zz_TotalScore.Value),
                                    HolesCompleted = (r.zz_CompletedHoles==null?0:r.zz_CompletedHoles.Value),
                                    Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    RoundInProgress = r.zz_name,
                                    GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                    TeamPageUrl = "GolfTeam.htm?GolfTeamId=" + gt.zz_golfteamId.ToString(),
                                    GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.Value.ToString() + "&GolfTeamId=" + gt.zz_golfteamId.Value.ToString(),
                                    GolferId = g.zz_golferId.Value,
                                    GolfTeamId = gt.zz_golfteamId.Value,
                                    NextTeeTime = (r.zz_TeeTime == null?"":r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                    dtNextTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                    GolferStatus = shared.sGolferStatus(g.statuscode),
                                    RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                    TodaysRoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                                }
                            ).ToList<Golfer>();
                }
                else
                {
                    Golfers = (

                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                where
                                    ed.zz_EventId.Id == Event.EventId
                                    && ed.zz_LeagueId.Id == Event.LeagueId
                                select new Golfer
                                {
                                    GolferName = g.zz_name,
                                    GolferGolfTeam = gt.zz_name,
                                    GolferScore = 0,
                                    HolesCompleted = 0,
                                    GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                    TeamPageUrl = "GolfTeam.htm?GolfTeamId=" + gt.zz_golfteamId.ToString(),
                                    GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.Value.ToString() + "&GolfTeamId=" + gt.zz_golfteamId.Value.ToString(),
                                    GolferId = g.zz_golferId.Value,
                                    GolfTeamId = gt.zz_golfteamId.Value,
                                    GolferStatus = shared.sGolferStatus(g.statuscode)
                                }
                            ).ToList<Golfer>();
                }

                List<GolfTeam> Teams = Golfers.GroupBy(s => new { s.GolfTeamId, s.GolferGolfTeam, s.TeamPageUrl }).Select(y => new GolfTeam()
                {
                    GolfTeamId = y.Key.GolfTeamId,
                    GolfTeamName = y.Key.GolferGolfTeam,
                    TeamUrl = y.Key.TeamPageUrl,
                }).ToList<GolfTeam>();

                List<GolfTeam> QualifiedTeams = new List<GolfTeam>();
                foreach (GolfTeam team in Teams)
                {
                    team.Golfers = Golfers.Where(g => g.GolfTeamId == team.GolfTeamId).ToList<Golfer>();
                    List<Golfer> TeamTop2 = team.Golfers.Where(g=>g.GolferStatus == "Active").ToList();
                    List<NumberRank> Top2 = new List<NumberRank>();
                    bool DQ = false;
                    switch (TeamTop2.Count())
                    {
                        case 0:
                            //Start of the event
                           // Top2 = TeamTop2.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                            DQ = true;
                            break;
                        case 1:
                            DQ = true;
                            break;
                        case 2:
                            Top2 = TeamTop2.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                            break;
                        case 3:
                            Top2 = TeamTop2.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                            break;
                        default:
                            Top2 = TeamTop2.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                            break;
                    }
                    if (!DQ)
                    {
                        team.TeamTop2Score = Top2.Sum(s => s.Number);
                        QualifiedTeams.Add(team);
                    }
                    
                }
                return QualifiedTeams;
            }
        }

        [WebMethod]
        public List<GolfTeam> Top4(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                EventAndLeague EvtLg = (from e in datacontext.zz_eventSet
                         join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                         join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                         where e.statecode.Value == 0
                         where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                         select new EventAndLeague
                         {
                             EventId = e.zz_eventId.Value,
                             LeagueId = le.zz_leagueid.Value
                         }).FirstOrDefault();

                
                //should make a rounds class 
                //golfers have rounds
                //how to find highest event round ?
                List<Round> allRounds = (
                             from ed in datacontext.zz_eventdraftSet
                             join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                             join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                             join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                             where
                                 ed.zz_EventId.Id == EvtLg.EventId
                                 && ed.zz_LeagueId.Id == EvtLg.LeagueId
                             where
                                 r.zz_EventId.Id == EvtLg.EventId
                                 && r.zz_EventStatus == shared.osvEventStatus("Played")
                             select new Round
                             {
                                 GolferId = g.zz_golferId.Value,
                                 RoundGolferName = g.zz_name,
                                 RoundGolfTeamId = gt.zz_golfteamId.Value,
                                 RoundGolfTeamName = gt.zz_name,
                                 Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                 RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                 RoundShots = (r.zz_Shots == null ? 0 : r.zz_Shots.Value),
                                 DayNumber = shared.iGetDay(r.zz_Day),
                                 dtTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                 TeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                 RoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                                 RoundName = r.zz_name,
                                 TournamentRank = r.zz_EventRank,
                                 RoundGolferStatus = shared.sGolferStatus(g.statuscode),
                                 //HolesCompleted = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                 //Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                 //RoundInProgress = r.zz_name,
                                 //GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                 //TeamPageUrl = "GolfTeam.htm?GolfTeamId=" + gt.zz_golfteamId.ToString(),
                                 //GolferPageUrl = "Golfer.htm?GolferId=" + g.zz_golferId.ToString(),
                                 //GolferId = g.zz_golferId.Value,
                                 //GolfTeamId = gt.zz_golfteamId.Value,
                                 //NextTeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                 //dtNextTeeTime = (r.zz_TeeTime == null ? new DateTime() : r.zz_TeeTime.Value.ToLocalTime()),
                                 //GolferStatus = shared.sGolferStatus(g.statuscode),
                                 //RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                 //TodaysRoundStarted = (r.zz_CompletedHoles == null ? false : (r.zz_CompletedHoles.Value != 0 ? true : false)),
                             }).ToList<Round>();


                List<GolfTeam> Teams = allRounds.GroupBy(s => new { s.RoundGolfTeamId, s.RoundGolfTeamName}).Select(y => new GolfTeam()
                {
                    GolfTeamId = y.Key.RoundGolfTeamId,
                    GolfTeamName = y.Key.RoundGolfTeamName,
                }).ToList<GolfTeam>();

                List<Golfer> Golfers = allRounds.GroupBy(s => new { s.GolferId, s.RoundGolferName, s.RoundGolfTeamId, s.RoundGolferStatus }).Select(y => new Golfer()
                {
                    GolferId = y.Key.GolferId,
                    GolferName = y.Key.RoundGolferName,
                    GolfTeamId = y.Key.RoundGolfTeamId,
                    GolferStatus = y.Key.RoundGolferStatus,
                }).ToList<Golfer>();

                int MaxDay = allRounds.Max(al => al.DayNumber);
                Golfers.ForEach(g =>
                    {
                        g.Rounds = allRounds.Where(al => al.GolferId == g.GolferId).ToList();
                        int MaxScore = 0;
                        Round MaxRound;
                        //the golfer has less rounds then the max
                        if (g.Rounds.Count() != MaxDay)
                        {
                            switch (MaxDay)
                            {
                                case 1:
                                    //this would be where you would add the max round score.
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    //Missed cut golfers, get thier highest round
                                    MaxScore = g.Rounds.Max(r=>r.RoundShots);
                                    MaxRound = g.Rounds.Where(r => r.RoundShots == MaxScore).FirstOrDefault();
                                    MaxRound.DayNumber = 3;
                                    MaxRound.Day = shared.osvGetDay(3);
                                    g.Rounds.Add(MaxRound);
                                    break;
                                case 4:
                                    //Missed cut add two rounds of max score
                                    MaxScore = g.Rounds.Max(r=>r.RoundShots);
                                    MaxRound = g.Rounds.Where(r => r.RoundShots == MaxScore).FirstOrDefault();
                                    MaxRound.DayNumber = 3;
                                    MaxRound.Day = shared.osvGetDay(3);
                                    g.Rounds.Add(MaxRound);
                                    MaxRound.DayNumber = 4;
                                    MaxRound.Day = shared.osvGetDay(4);
                                    g.Rounds.Add(MaxRound);
                                    break;
                            }
                        }
                        g.GolferScore = g.Rounds.Sum(r => r.RoundScore);

                        
                        Round CurrentRound = g.Rounds.OrderByDescending(r=>r.DayNumber).FirstOrDefault();

                        g.Thru = CurrentRound.Thru;
                        g.RoundScore = CurrentRound.RoundScore;
                        g.dtNextTeeTime = CurrentRound.dtTeeTime;
                        g.NextTeeTime = CurrentRound.TeeTime;
                        g.TodaysRoundStarted = CurrentRound.RoundStarted;
                        g.RoundInProgress = CurrentRound.RoundName;
                        g.TournamentRank = CurrentRound.TournamentRank;
                        g.GolferPageUrl = "Golfer.htm?GolferId=" + g.GolferId.ToString() + "&GolfTeamId=" + GolfTeamId.ToString();
                        
                    });

                Teams.ForEach(t =>
                    {
                        t.Golfers = Golfers.Where(g => g.GolfTeamId == t.GolfTeamId).OrderBy(g => g.GolferScore).ToList();
                        t.Top4Score = t.Golfers.Sum(g => g.GolferScore);
                    });

                
                return Teams.OrderBy(t=>t.Top4Score).ToList();
            }
        }

        [WebMethod]
        public List<EventDraft> DraftResults(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                        }).FirstOrDefault();

                List<EventDraft> draftpicks = (
                                        from r in datacontext.zz_eventdraftSet
                                        join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId
                                        join eo in datacontext.zz_eventoddsSet on g.zz_golferId equals eo.zz_GolferId.Id
                                        join gt in datacontext.zz_golfteamSet on r.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                        join e in datacontext.zz_eventSet on r.zz_EventId.Id equals e.zz_eventId.Value
                                        where
                                            r.zz_EventId.Id == Event.EventId
                                            && r.statuscode != new OptionSetValue(419260000)
                                            && r.zz_LeagueId.Id == Event.LeagueId
                                        where
                                            eo.zz_EventId.Id == Event.EventId
                                        select new EventDraft
                                        {
                                            EventDraftGolfer = g.zz_name,
                                            EventDraftEvent = e.zz_name,
                                            EventDraftPickNumber = r.zz_PickNumber.Value,
                                            EventDraftGolfTeam = gt.zz_name,
                                            GolferImgUrl =   @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                            EventDraftPickCompletedOn = r.ModifiedOn.Value,
                                            EventDraftPickDuration = FormatDuration(r.ModifiedOn.Value, r.CreatedOn.Value),
                                            Odds = eo.zz_Odds.Value,
                                        }
                                    ).ToList<EventDraft>();


                return draftpicks;     
            }
            
        }

        [WebMethod]
        public List<EventOdd> UndraftedGolfers(String GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                Guid golfTeamId = Guid.Parse(GolfTeamId);
                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                            LeagueType = l.zz_LeagueType,
                                        }).FirstOrDefault();

                
                GolfTeam otc = (from gt in datacontext.zz_golfteamSet
                                where
                                 gt.statuscode == new OptionSetValue(419260000)
                                 && gt.zz_LeagueId.Id == Event.LeagueId
                                select new GolfTeam
                                {
                                    GolfTeamName = gt.zz_name,
                                    GolfTeamId = gt.zz_golfteamId.Value
                                }).FirstOrDefault();

                Guid otc_GolfTeamId = Guid.Empty;
                if (otc != null)
                    otc_GolfTeamId = otc.GolfTeamId;

                //zz_event Event = (from e in datacontext.zz_eventSet where e.zz_eventId == Guid.Parse("9D4B5EF9-DED1-E211-BDB6-3EBB89D5579F") select e).FirstOrDefault();
                
                //Get list of odds for event
                List<EventOdd> Odds  = (
                                        from r in datacontext.zz_eventoddsSet
                                        // join e in datacontext.zz_eventSet on r.zz_EventId.Id equals e.zz_eventId
                                        join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId
                                        //where                                        
                                        //    r.statuscode != new OptionSetValue(419260000)
                                        //where
                                        //  e.statecode == zz_eventState.Active
                                        where r.zz_EventId.Id == Event.EventId
                                        && r.statecode.Value == 0 //active
                                        orderby r.zz_Odds, r.zz_name
                                        select new EventOdd
                                        {
                                            odds = int.Parse(r.zz_Odds.ToString()),
                                            Golfer = g.zz_name,
                                            GolferId = g.zz_golferId.Value,
                                            EventOddsId = r.zz_eventoddsId.Value,
                                            GolferImgUrl = (g.zz_CBSSportsGolferId.ToString() == null ? "" : @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg"),
                                            GolferRotoWorldUrl = (g.zz_rotoworldId.ToString() == null ? "" : @"http://www.rotoworld.com/player/gol/" + g.zz_rotoworldId.ToString()),
                                            OnTheClock_GolfTeamId = otc_GolfTeamId,
                                            EventId = r.zz_EventId.Id,
                                        }
                                    ).ToList<EventOdd>();
                

                //Get list of draft picks
                List<DraftPick> draftPicks = new List<DraftPick>();
                if (Event.LeagueType.Value == 419260003)
                {
                    //Exclude the parent draft picks as well
                    List<EventAndLeague> ParentLeagues =
                           (from e in datacontext.zz_eventSet
                            join el in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals el.zz_eventid.Value
                            join l in datacontext.zz_leagueSet on el.zz_leagueid.Value equals l.zz_leagueId.Value
                            join pl in datacontext.zz_zz_league_zz_leagueSet on l.zz_leagueId.Value equals pl.zz_leagueidTwo.Value
                            join pll in datacontext.zz_leagueSet on pl.zz_leagueidOne.Value equals pll.zz_leagueId.Value
                            where
                                e.zz_eventId.Value == Event.EventId
                            where
                                l.zz_leagueId.Value == Event.LeagueId
                            //where
                            //    l.zz_leagueId.Value == Event.LeagueId
                            select new EventAndLeague
                            {
                                EventId = e.zz_eventId.Value,
                                LeagueId = pll.zz_leagueId.Value,
                            }).ToList<EventAndLeague>();
                            
                    ParentLeagues.ForEach(pl=>
                        {
                            List<DraftPick> ParentLeaguePicks =
                           (from e in datacontext.zz_eventSet
                            join ed in datacontext.zz_eventdraftSet on e.zz_eventId.Value equals ed.zz_EventId.Id
                            join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                            where
                                e.zz_eventId.Value == Event.EventId
                            where
                                ed.zz_LeagueId.Id == pl.LeagueId
                            select new DraftPick
                            {
                                EventId = e.zz_eventId.Value,
                                EventName = e.zz_name,
                                GolferId = g.zz_golferId.Value,
                                GolferName = g.zz_name,
                            }
                           ).ToList<DraftPick>();

                            ParentLeaguePicks.ForEach(plp=>
                                {
                                    draftPicks.Add(plp);
                                });
                        });

                    List<DraftPick> LeagueDraftPicks =
                          (from e in datacontext.zz_eventSet
                           join ed in datacontext.zz_eventdraftSet on e.zz_eventId.Value equals ed.zz_EventId.Id
                           join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                           where
                               e.zz_eventId.Value == Event.EventId
                           where
                               ed.zz_LeagueId.Id == Event.LeagueId
                           select new DraftPick
                           {
                               EventId = e.zz_eventId.Value,
                               EventName = e.zz_name,
                               GolferId = g.zz_golferId.Value,
                               GolferName = g.zz_name,
                           }
                          ).ToList<DraftPick>();

                    LeagueDraftPicks.ForEach(plp =>
                    {
                        draftPicks.Add(plp);
                    });
                }
                else
                {
                    draftPicks =
                           (from e in datacontext.zz_eventSet
                            join ed in datacontext.zz_eventdraftSet on e.zz_eventId.Value equals ed.zz_EventId.Id
                            join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                            where
                                e.zz_eventId.Value == Event.EventId
                            where
                                ed.zz_LeagueId.Id == Event.LeagueId
                            select new DraftPick
                            {
                                EventId = e.zz_eventId.Value,
                                EventName = e.zz_name,
                                GolferId = g.zz_golferId.Value,
                                GolferName = g.zz_name,
                            }
                           ).ToList<DraftPick>();
                }


                //Remove Any drafted golfers from odds list
                draftPicks.ForEach(dp =>
                {
                    if (dp.GolferId != null)
                    {
                        Odds.RemoveAll(o => o.EventId == dp.EventId && o.GolferId == dp.GolferId);
                    }
                });

                return Odds;
            }

        }

        [WebMethod]
        public Skins GetSkins(string GolfTeamId, String Day)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                Skins skins = new Skins();

                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                        }).FirstOrDefault();

                List<Hole> SkinHoles = (from ls in datacontext.zz_leagueskinSet
                                        join h in datacontext.zz_holeSet on ls.zz_HoleId.Id equals h.zz_holeId.Value
                                        join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                        join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId.Value equals chh.zz_CourseHoleId.Id
                                        join r in datacontext.zz_roundSet on h.zz_RoundId.Id equals r.zz_roundId.Value
                                        join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId.Value
                                       // join ed in datacontext.zz_eventdraftSet on g.zz_golferId.Value equals ed.zz_GolferId.Id
                                        join gt in datacontext.zz_golfteamSet on ls.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                        where
                                          ls.zz_LeagueId.Id == Event.LeagueId
                                        where
                                            chh.zz_Day.Value == (Day == "Sat" ? 419260002 : 419260003)
                                            && chh.zz_LeagueId.Id == Event.LeagueId
                                        where
                                          r.zz_EventId.Id == Event.EventId
                                          && r.zz_Day.Value == (Day=="Sat"?419260002:419260003)
                                        //where
                                        //  ed.zz_EventId.Id == Event.EventId
                                        //  && ed.zz_LeagueId.Id == Event.LeagueId
                                        select new Hole
                                        {
                                            HoleId = h.zz_holeId.Value,
                                            HoleNumber = h.zz_HoleNumber.Value,
                                            Shots = h.zz_Shots.Value,
                                            HoleDay = r.zz_Day,
                                            GolfTeamId = gt.zz_golfteamId.Value,
                                            HoleGolfTeam = gt.zz_name,
                                            HoleGolfer = g.zz_name,
                                            ScoreName = getScoreName(ch.zz_Par.Value,h.zz_Shots.Value),
                                            //CalculatedHandicapRank = (ch.zz_Handicap==null?0:ch.zz_Handicap.Value),
                                            CalculatedHandicapRank = chh.zz_HandicapCalculatedRank.Value,
                                            CalculatedHandicap = chh.zz_HandicapCalculated.Value,
                                            Par = ch.zz_Par.Value,
                                        }).ToList<Hole>();

                List<PossibleHole> AllHoles = (from ed in datacontext.zz_eventdraftSet
                                               join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                                               join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                               join h in datacontext.zz_holeSet on r.zz_roundId.Value equals h.zz_RoundId.Id
                                               join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                               join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                               join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId.Value equals chh.zz_CourseHoleId.Id
                                               where
                                                   ed.zz_EventId.Id == Event.EventId
                                                   && ed.zz_LeagueId.Id == Event.LeagueId
                                               where
                                                    r.zz_EventId.Id == Event.EventId
                                                    && r.zz_Day.Value == (Day == "Sat" ? 419260002 : 419260003)
                                               where
                                                    gt.zz_LeagueId.Id == Event.LeagueId
                                               where
                                                    chh.zz_Day.Value == (Day == "Sat" ? 419260002 : 419260003)
                                                    && chh.zz_LeagueId.Id == Event.LeagueId
                                               select new PossibleHole
                                               {
                                                   pHoleShots = h.zz_Shots.Value,
                                                   pHoleGolfTeam = gt.zz_name,
                                                   pHoleGolfTeamId = gt.zz_golfteamId.Value,
                                                   pHoleHoleGolfer = g.zz_name,
                                                   pHoleDay = r.zz_Day,
                                                   pHoleScoreName = getScoreName(ch.zz_Par.Value, h.zz_Shots.Value),
                                                   pHoleNumber = h.zz_HoleNumber.Value,
                                                   pHoleCreatedOn = h.CreatedOn.Value,
                                                   CalculatedHandicapRank = chh.zz_HandicapCalculatedRank.Value,
                                                   pHolePar = ch.zz_Par.Value,
                                               }).ToList<PossibleHole>();

                List<Golfer> ActiveGolfers = (from ed in datacontext.zz_eventdraftSet
                                              join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                              join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                                              where
                                                 ed.zz_EventId.Id == Event.EventId
                                                 && ed.zz_LeagueId.Id == Event.LeagueId
                                              where
                                                g.statuscode.Value == 1
                                              select new Golfer
                                              {
                                                  GolfTeamId = gt.zz_golfteamId.Value,
                                              }).ToList<Golfer>();

                List<Hole> Holes = new List<Hole>();
                for (int x = 1; x < 19; x++)
                {

                    PossibleHole ph = AllHoles.Where(al => al.pHoleNumber == x).OrderBy(al=>al.pHoleShots).FirstOrDefault();
                    if (ph != null)
                    {
                        Hole h = new Hole
                        {
                            HoleNumber = x,
                            EventPlays = AllHoles.Where(al => al.pHoleNumber == x).OrderByDescending(al => al.pHoleCreatedOn).ToList(),
                            HoleGolfTeam = "Tied",
                            ScoreName = ph.pHoleScoreName,
                            CalculatedHandicapRank = (ph.CalculatedHandicapRank == null ? 0 : ph.CalculatedHandicapRank),
                            Par = ph.pHolePar,
                        };

                        if (Day == "Sat")
                        {
                            h.SkinBirdieCount_Sat = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Birdie").Count();
                            h.SkinEagleCount_Sat = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Eagle").Count();
                            h.SkinBogeyCount_Sat = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Bogey").Count();
                            h.SkinParCount_Sat = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Par").Count();
                        }
                        else
                        {
                            h.SkinBirdieCount_Sun = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Birdie").Count();
                            h.SkinEagleCount_Sun = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Eagle").Count();
                            h.SkinBogeyCount_Sun = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Bogey").Count();
                            h.SkinParCount_Sun = AllHoles.Where(al => al.pHoleNumber == x && al.pHoleScoreName == "Par").Count();
                        }

                        Hole skin = SkinHoles.Where(sh => sh.HoleNumber == x).FirstOrDefault();
                        if (skin != null)
                        {
                            h.Skin = true;
                            h.HoleId = skin.HoleId;

                            h.Shots = skin.Shots;
                            h.HoleDay = skin.HoleDay;
                            h.GolfTeamId = skin.GolfTeamId;
                            h.HoleGolfTeam = skin.HoleGolfTeam;
                            h.HoleGolfer = skin.HoleGolfer;
                            h.ScoreName = skin.ScoreName;
                            h.CalculatedHandicapRank = skin.CalculatedHandicapRank;
                            h.CalculatedHandicap = skin.CalculatedHandicap;
                            h.Par = skin.Par;
                        }

                        int AllHolesGolfersCount = AllHoles.Where(al => al.pHoleNumber == x).GroupBy(ah => ah.pHoleHoleGolfer).Count();
                        int ActiveGolfersCount = ActiveGolfers.Count();
                        if (AllHolesGolfersCount > ActiveGolfersCount)
                            h.ActiveGolferCount = AllHolesGolfersCount;
                        else
                            h.ActiveGolferCount = ActiveGolfersCount;

                        Holes.Add(h);
                    }
                }

                //SkinHoles.ForEach(sh =>
                //    {
                //        sh.EventPlays = AllHoles.Where(al => al.pHoleNumber == sh.HoleNumber).OrderByDescending(al=>al.pHoleCreatedOn).ToList();
                        
                //        int AllHolesGolfersCount= AllHoles.Where(al => al.pHoleNumber == sh.HoleNumber).GroupBy(ah => ah.pHoleHoleGolfer).Count();
                //        int ActiveGolfersCount = ActiveGolfers.Count();
                //        if (AllHolesGolfersCount > ActiveGolfersCount)
                //            sh.ActiveGolferCount = AllHolesGolfersCount;
                //        else
                //            sh.ActiveGolferCount = ActiveGolfersCount;
                //    });

                switch (Day)
                {
                    case "Sat":
                        //skins.Skins_Sat = SkinHoles.OrderBy(sh=>sh.HoleNumber).ToList();
                        skins.Skins_Sat = Holes.OrderBy(sh => sh.HoleNumber).ToList();
                        break;
                    case "Sun":
                        //skins.Skins_Sun = SkinHoles.OrderBy(sh => sh.HoleNumber).ToList();
                        skins.Skins_Sun = Holes.OrderBy(sh => sh.HoleNumber).ToList();
                        break;
                }

                return skins;
            }
        }

        [WebMethod]
        public AllGolfTeams BestBall(string GolfTeamId,String Day)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                AllGolfTeams golfTeams = new AllGolfTeams();

                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                        }).FirstOrDefault();

                
                
                    List<Hole> allBBHoles = (from gtbbh in datacontext.zz_golfteambestballholeSet
                                                join h in datacontext.zz_holeSet on gtbbh.zz_HoleId.Id equals h.zz_holeId.Value
                                                join r in datacontext.zz_roundSet on h.zz_RoundId.Id equals r.zz_roundId.Value
                                                join g in datacontext.zz_golferSet on r.zz_GolferId.Id equals g.zz_golferId.Value
                                                join gt in datacontext.zz_golfteamSet on gtbbh.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                                join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId.Value equals chh.zz_CourseHoleId.Id
                                                where
                                                  r.zz_EventId.Id == Event.EventId
                                  //                && r.zz_Day.Value == (Day == "Thurs" ? 419260000 : 419260001)
                                                where
                                                  gt.zz_LeagueId.Id == Event.LeagueId
                                                where
                                    //                chh.zz_Day.Value == (Day == "Thurs" ? 419260000 : 419260001)
                                                   // && 
                                                   chh.zz_LeagueId.Id == Event.LeagueId
                                                select new Hole
                                                {
                                                    Shots = h.zz_Shots.Value,
                                                    HoleGolfTeam = gt.zz_name, // + (gtbbh.zz_TieBreaker == null ? "" : (gtbbh.zz_TieBreaker.Value==true?" *TB":"")),
                                                    GolfTeamId = gt.zz_golfteamId.Value,
                                                    HoleDay = r.zz_Day,
                                                    BestBallTieBreaker = (gtbbh.zz_TieBreaker == null ? false : gtbbh.zz_TieBreaker.Value),
                                                    Par = (ch.zz_Par == null ? 0 : ch.zz_Par.Value),
                                                    ScoreName = getScoreName(ch.zz_Par.Value, h.zz_Shots.Value),
                                                    HoleNumber = h.zz_HoleNumber.Value,
                                                    CalculatedHandicapRank = (chh.zz_HandicapCalculatedRank == null ? 0 : chh.zz_HandicapCalculatedRank.Value),
                                                    CalculatedHandicap = (chh.zz_HandicapCalculated == null ? 0 : chh.zz_HandicapCalculated.Value),
                                                    CalculatedHandicapDay = chh.zz_Day,
                                                    HoleId = h.zz_holeId.Value,
                                                    CreatedOn = h.CreatedOn.Value,
                                                    HoleGolfer = g.zz_name,
                                                }).ToList<Hole>();

                    List<PossibleHole> allPHoles = (from ed in datacontext.zz_eventdraftSet
                                                   join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                                                   join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                                   join h in datacontext.zz_holeSet on r.zz_roundId.Value equals h.zz_RoundId.Id
                                                   join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                   join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                                   where
                                                       ed.zz_EventId.Id == Event.EventId
                                                   where
                                                     r.zz_EventId.Id == Event.EventId
                //                                     && r.zz_Day.Value == (Day == "Thurs" ? 419260000 : 419260001)
                                                   where
                                                     gt.zz_LeagueId.Id == Event.LeagueId
                                                   select new PossibleHole
                                                {
                                                    pHoleShots = h.zz_Shots.Value,
                                                    pHoleGolfTeam = gt.zz_name,
                                                    pHoleGolfTeamId = gt.zz_golfteamId.Value,
                                                    pHoleHoleGolfer = g.zz_name,
                                                    pHoleDay = r.zz_Day,
                                                    pHoleScoreName = getScoreName(ch.zz_Par.Value, h.zz_Shots.Value),
                                                    pHoleNumber = h.zz_HoleNumber.Value,
                                                }).ToList<PossibleHole>();
                List<Hole> BestBallHoles = new List<Hole>();
                List<PossibleHole> AllHoles = new List<PossibleHole>();

                if (Day == "Thurs" || Day == "Fri")
                {
                    BestBallHoles = allBBHoles.Where(r => r.HoleDay.Value == (Day == "Thurs" ? 419260000 : 419260001) && r.CalculatedHandicapDay.Value == (Day == "Thurs" ? 419260000 : 419260001)).ToList();
                    AllHoles = allPHoles.Where(r => r.pHoleDay.Value == (Day == "Thurs" ? 419260000 : 419260001)).ToList();

                    BestBallHoles.ForEach(bbh =>
                    {
                        bbh.TeamPlays = AllHoles.Where(ah => ah.pHoleGolfTeamId == bbh.GolfTeamId && bbh.HoleNumber == ah.pHoleNumber).ToList();
                        bbh.TeamPlays.ForEach(bbtp => { bbtp.pHoleParentHoleId = bbh.HoleId; bbtp.pActiveGolfers = 4; });
                    });

                }
                else
                {
                    //look through best ball holes and find min for each hole
                    //for (int i = 1; i < 19; i++)
                    //{
                    //    int MinShots = allBBHoles.Where(bbh => bbh.HoleNumber == i).Min(bbh => bbh.Shots);
                    //    Hole h = allBBHoles.Where(bbh => bbh.HoleNumber == i && bbh.Shots == MinShots).FirstOrDefault();
                    //    BestBallHoles.Add(h);
                    //}
                    BestBallHoles = allBBHoles.ToList();
                    AllHoles = allPHoles.ToList();
                }

                    List<Golfer> ActiveGolfers = (from ed in datacontext.zz_eventdraftSet
                                                  join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                  join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                                                  where
                                                     ed.zz_EventId.Id == Event.EventId
                                                     && ed.zz_LeagueId.Id == Event.LeagueId
                                                  where
                                                    g.statuscode.Value == 1
                                                  select new Golfer
                                                  {
                                                      GolfTeamId = gt.zz_golfteamId.Value,
                                                  }).ToList<Golfer>();
                

                

                List<GolfTeam> teams = BestBallHoles.GroupBy(s => new { s.GolfTeamId, s.HoleGolfTeam }).Select(y => new GolfTeam()
                {
                    GolfTeamId = y.Key.GolfTeamId,
                    GolfTeamName = y.Key.HoleGolfTeam,
                }).ToList<GolfTeam>();

                

                teams.ForEach(gt =>
                {
                    //need to acutally calculate this
                    gt.BestBallTieBreaker_Fri = false;
                    gt.BestBallTieBreaker_Thurs = false;
                    gt.BestBallTieBreaker_36 = false;

                    int AllHolesGolfersCount = AllHoles.Where(ah => ah.pHoleGolfTeamId == gt.GolfTeamId).GroupBy(ah => ah.pHoleHoleGolfer).Count();
                    int ActiveGolfersCount = ActiveGolfers.Where(ag => ag.GolfTeamId == gt.GolfTeamId).Count();

                    if(AllHolesGolfersCount > ActiveGolfersCount)
                        gt.ActiveGolfers = AllHolesGolfersCount;
                    else
                        gt.ActiveGolfers = ActiveGolfersCount;

                    int bbhTB = 0;
                    switch (Day)
                    {
                        case "Thurs":
                            gt.BestBallScore_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000).Sum(bbh => bbh.Shots) - BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000).Sum(bbh => bbh.Par);
                            gt.BestBallHoles_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000).OrderBy(bbh => bbh.HoleNumber).ToList();
                            bbhTB = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.BestBallTieBreaker == true).ToList().Count();
                            if (bbhTB != 0)
                                gt.BestBallTieBreaker_Thurs = true;
                            gt.BestBallBirdieCount_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.ScoreName == "Birdie").ToList().Count();
                            gt.BestBallEagleCount_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.ScoreName == "Eagle").ToList().Count();
                            gt.BestBallParCount_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.ScoreName == "Par").ToList().Count();
                            gt.BestBallBogeyCount_Thurs = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260000 && bbh.ScoreName.Contains("Bogey")).ToList().Count();


                            

                            break;
                        case "Fri":
                            gt.BestBallScore_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001).Sum(bbh => bbh.Shots) - BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001).Sum(bbh => bbh.Par);
                            gt.BestBallHoles_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001).OrderBy(bbh => bbh.HoleNumber).ToList();

                            bbhTB = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.BestBallTieBreaker == true).ToList().Count();
                            if (bbhTB != 0)
                                gt.BestBallTieBreaker_Fri = true;

                            gt.BestBallBirdieCount_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.ScoreName == "Birdie").ToList().Count();
                            gt.BestBallEagleCount_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.ScoreName == "Eagle").ToList().Count();
                            gt.BestBallParCount_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.ScoreName == "Par").ToList().Count();
                            gt.BestBallBogeyCount_Fri = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.HoleDay.Value == 419260001 && bbh.ScoreName.Contains("Bogey")).ToList().Count();
                            break;

                        case "36":
                            
                            //gt.BestBallHoles_36 = BestBallHoles.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId).OrderBy(bbh => bbh.HoleNumber).ToList();
                            for (int i = 1; i < 19; i++)
                            {
                                int MinShots = allBBHoles.Where(bbh => bbh.HoleNumber == i && bbh.GolfTeamId == gt.GolfTeamId).Min(bbh => bbh.Shots);
                                Hole h = allBBHoles.Where(bbh => bbh.HoleNumber == i && bbh.Shots == MinShots && bbh.GolfTeamId == gt.GolfTeamId).FirstOrDefault();
                                gt.BestBallHoles_36.Add(h);
                            }

                            int sumOfShots = gt.BestBallHoles_36.Sum(bbh => bbh.Shots);
                            int sumOfPar = gt.BestBallHoles_36.Sum(bbh => bbh.Par);
                            gt.BestBallScore_36 = gt.BestBallHoles_36.Sum(bbh => bbh.Shots) - gt.BestBallHoles_36.Sum(bbh => bbh.Par);

                            bbhTB = gt.BestBallHoles_36.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.BestBallTieBreaker == true).ToList().Count();
                            if (bbhTB != 0)
                                gt.BestBallTieBreaker_36 = true;

                            gt.BestBallBirdieCount_36 = gt.BestBallHoles_36.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.ScoreName == "Birdie").ToList().Count();
                            gt.BestBallEagleCount_36 = gt.BestBallHoles_36.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.ScoreName == "Eagle").ToList().Count();
                            gt.BestBallParCount_36 = gt.BestBallHoles_36.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.ScoreName == "Par").ToList().Count();
                            gt.BestBallBogeyCount_36 = gt.BestBallHoles_36.Where(bbh => bbh.GolfTeamId == gt.GolfTeamId && bbh.ScoreName.Contains("Bogey")).ToList().Count();

                            if (BestBallHoles.Max(bbh => bbh.HoleDay.Value) == 419260001)
                                gt.ActiveGolfers = ActiveGolfersCount * 2;
                            
                            gt.BestBallHoles_36.ForEach(bbh =>
                            {
                                bbh.TeamPlays = AllHoles.Where(ah => ah.pHoleGolfTeamId == bbh.GolfTeamId && bbh.HoleNumber == ah.pHoleNumber).ToList();
                                bbh.TeamPlays.ForEach(bbtp => { bbtp.pHoleParentHoleId = bbh.HoleId; bbtp.pActiveGolfers = gt.ActiveGolfers; });
                            });


                            //OptionSetValue MaxDay = BestBallHoles.Max(bbh => bbh.HoleDay);
                            //if this is friday then multiple the active golfer count by 2 for the total hole calculation
                            
                            break;
                    }
                });

                switch (Day)
                {
                    case "Thurs":
                        golfTeams.GolfTeams = teams.OrderBy(gt => gt.BestBallScore_Thurs).OrderByDescending(gt => gt.BestBallTieBreaker_Thurs).ToList();
                        break;
                    case "Fri":
                        golfTeams.GolfTeams = teams.OrderBy(gt => gt.BestBallScore_Fri).OrderByDescending(gt => gt.BestBallTieBreaker_Fri).ToList();
                        break;
                    case "36":
                        golfTeams.GolfTeams = teams.OrderBy(gt => gt.BestBallScore_36).OrderByDescending(gt => gt.BestBallTieBreaker_36).ToList();
                        break;
                }
                return golfTeams;
            }
        }
        private string getScoreName(int par, int shots)
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
            }
            if (shots == 1)
                rtn = "Hole in One!!!!!";

            return rtn;
        }

        [WebMethod]
        public AllGolfers Golfer(string GolfTeamId, string GolferId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                AllGolfers allGolfers = new AllGolfers();
                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join l in datacontext.zz_leagueSet on le.zz_leagueid.Value equals l.zz_leagueId.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                        }).FirstOrDefault();

                List<Hole> AllHoles = (from ed in datacontext.zz_eventdraftSet
                                       join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId.Value
                                       join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                       join h in datacontext.zz_holeSet on r.zz_roundId.Value equals h.zz_RoundId.Id
                                       join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                       join ch in datacontext.zz_courseholeSet on h.zz_CourseHoleId.Id equals ch.zz_courseholeId.Value
                                       //join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId.Value equals chh.zz_CourseHoleId.Id
                                       where
                                           ed.zz_EventId.Id == Event.EventId
                                       where 
                                            g.zz_golferId.Value == Guid.Parse(GolferId)
                                       where
                                         r.zz_EventId.Id == Event.EventId
                                         && r.zz_GolferId.Id == Guid.Parse(GolferId)
                                       where
                                         gt.zz_LeagueId.Id == Event.LeagueId
                                       //where
                                       //    chh.zz_LeagueId.Id == Event.LeagueId
                                       select new Hole
                                       {
                                           Shots = h.zz_Shots.Value,
                                           HoleGolfTeam = gt.zz_name,
                                           GolfTeamId = gt.zz_golfteamId.Value,
                                           HoleDay = r.zz_Day,
                                           Par = ch.zz_Par.Value,
                                           ScoreName = getScoreName(ch.zz_Par.Value, h.zz_Shots.Value),
                                           HoleNumber = h.zz_HoleNumber.Value,
                                           HoleId = h.zz_holeId.Value,
                                           RoundId = r.zz_roundId.Value,
                                           RoundName = r.zz_name,
                                           RoundScore = r.zz_Score.Value,
                                           RoundShots = r.zz_Shots.Value,
                                           RoundThru = r.zz_CompletedHoles.Value,
                                           RoundEventStatus = shared.sGetRoundEventStatus(r.zz_EventStatus),
                                           HoleGolfer = g.zz_name,
                                           HoleGolferId = g.zz_golferId.Value,
                                           CreatedOn = h.CreatedOn.Value,
                                           CourseHoleId = ch.zz_courseholeId.Value,
                                           DayNumber = shared.iGetDay(r.zz_Day),
                                           CalculatedHandicapRank = (ch.zz_Handicap==null?0:ch.zz_Handicap.Value),
                                           //CalculatedHandicapRank = (chh.zz_HandicapCalculatedRank == null ? 0 : chh.zz_HandicapCalculatedRank.Value),
                                          // CourseHandicapDayNumber = shared.iGetDay(chh.zz_Day),
                                       }).ToList<Hole>();

                //List<Hole> CourseHoles = (from e in datacontext.zz_eventSet
                //                          join c in datacontext.zz_courseSet on e.zz_CourseId.Id equals c.zz_courseId
                //                          join ch in datacontext.zz_courseholeSet on c.zz_courseId.Value equals ch.zz_CourseId.Id
                //                          join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId.Value equals chh.zz_CourseHoleId.Id
                //                          where 
                //                            e.zz_eventId == Event.EventId
                //                          where
                //                            chh.zz_LeagueId.Id ==Event.LeagueId
                //                          select new Hole
                //                          {
                //                              CourseHoleId = ch.zz_courseholeId.Value,
                //                              CalculatedHandicap = chh.zz_HandicapCalculated.Value,
                //                              CalculatedHandicapRank = chh.zz_HandicapCalculatedRank.Value,
                //                              HoleDay = chh.zz_Day
                //                          }).ToList<Hole>();

                //AllHoles.ForEach(ah =>
                //{
                //    Hole HandicapHole = CourseHoles.Where(ch => ch.CourseHoleId == ah.CourseHoleId && ch.HoleDay.Value == ah.HoleDay.Value).FirstOrDefault();
                //    if (HandicapHole != null)
                //        ah.CalculatedHandicapRank = HandicapHole.CalculatedHandicapRank;
                //});

                allGolfers.Golfers = AllHoles.GroupBy(s => new { s.HoleGolferId, s.HoleGolfer }).Select(y => new Golfer()
                {
                    GolferId = y.Key.HoleGolferId,
                    GolferName = y.Key.HoleGolfer,
                }).ToList<Golfer>();

                allGolfers.Golfers.ForEach(g =>
                {
                    g.Rounds = AllHoles.GroupBy(s => new { s.RoundId, s.RoundName, s.RoundScore, s.RoundShots, s.RoundEventStatus, s.DayNumber, s.RoundThru }).Select(y => new Round()
                    {
                        RoundScore= y.Key.RoundScore,
                        RoundId = y.Key.RoundId,
                        RoundName = y.Key.RoundName,
                        RoundShots = y.Key.RoundShots,
                        RoundEventStatus = y.Key.RoundEventStatus,
                        DayNumber = y.Key.DayNumber,
                        //Thru = y.Key.RoundThru,
                    }).OrderByDescending(r => r.DayNumber).ToList<Round>();

                    g.Rounds.ForEach(r =>
                    {
                        r.Holes = AllHoles.Where(ah => ah.RoundId == r.RoundId).OrderByDescending(ah => ah.CreatedOn).ToList();
                        r.Thru = r.Holes.Count();
                    });
                });

                return allGolfers;
            }
        }

        [WebMethod]
        public PlayerMatchupsByTeam PlayerMatchups(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);


                zz_event Event;

                Event = (from e in datacontext.zz_eventSet
                         join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                         join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                         where e.statecode.Value == 0
                         where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                         select e).FirstOrDefault();

                if (Event == null)
                    Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                GolfTeam golfTeam = (from l in datacontext.zz_leagueSet
                                    join gt in datacontext.zz_golfteamSet on l.zz_leagueId.Value equals gt.zz_LeagueId.Id
                                    where
                                     gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                    select new GolfTeam
                                    {
                                        GolfTeamName = gt.zz_name,
                                        GolfTeamId = gt.zz_golfteamId.Value,
                                        LeagueId = l.zz_leagueId.Value,
                                    }
                                 ).FirstOrDefault();

                List<Round> Rounds = (
                                from ed in datacontext.zz_eventdraftSet
                                join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                where
                                    ed.zz_EventId.Id == Event.zz_eventId.Value
                                where
                                    r.zz_EventId.Id == Event.zz_eventId.Value
                                select new Round
                                {
                                    GolferId = g.zz_golferId.Value,
                                    TotalScore = (r.zz_TotalScore == null ? 0 : r.zz_TotalScore.Value),
                                    RoundName = r.zz_name,
                                    RoundId = r.zz_roundId.Value,
                                    RoundShots = r.zz_Shots.Value,
                                    RoundEventStatus = shared.sGetRoundEventStatus(r.zz_EventStatus),
                                    Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                    TournamentRank = r.zz_EventRank,
                                    TeeTime = (r.zz_TeeTime==null?"":r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                    Day = r.zz_Day,
                                    DayNumber = shared.iGetDay(r.zz_Day),
                                    dtTeeTime = (r.zz_TeeTime==null?DateTime.Now:r.zz_TeeTime.Value.ToLocalTime()),
                                    LastUpdated = r.ModifiedOn.Value.ToLocalTime(),
                                    RoundGolferEventStatus = shared.sGolferStatus(g.statuscode),
                                }
                            ).ToList<Round>();

                PlayerMatchupsByTeam TeamMatchups = new PlayerMatchupsByTeam
                    {
                        EventId = Event.zz_eventId.Value
                    };

                List<PlayerMatchup> allMatchups = (
                                                    from sb in datacontext.zz_sidebetSet
                                                    join sbt in datacontext.zz_sidebetteamsSet on sb.zz_sidebetId equals sbt.zz_SideBetId.Id
                                                    //join e in datacontext.zz_eventSet on sb.zz_EventId.Id equals e.zz_eventId
                                                    join u in datacontext.zz_golferSet on sb.zz_aGolferId.Id equals u.zz_golferId
                                                    join f in datacontext.zz_golferSet on sb.zz_bGolferId.Id equals f.zz_golferId
                                                    join ugt in datacontext.zz_golfteamSet on sbt.zz_aGolfTeamId.Id equals ugt.zz_golfteamId
                                                    join fgt in datacontext.zz_golfteamSet on sbt.zz_bGolfTeamId.Id equals fgt.zz_golfteamId
                                                    where
                                                        sb.zz_EventId.Id == Event.zz_eventId
                                                        //(
                                                        //    ugt.zz_LeagueId.Id == golfTeam.LeagueId
                                                        //    && ugt.zz_golfteamId.Value == golfTeam.GolfTeamId
                                                        //)
                                                        //||
                                                        //(
                                                        //    fgt.zz_LeagueId.Id == golfTeam.LeagueId
                                                        //    && fgt.zz_golfteamId.Value == golfTeam.GolfTeamId
                                                        //)
                                                    select new PlayerMatchup
                                                    {
                                                        FavoriteGolferId = f.zz_golferId.Value,
                                                        FavoriteGolfer = f.zz_name,
                                                        FavoriteGolfTeam = fgt.zz_name,
                                                        FavoriteGolfTeamId = fgt.zz_golfteamId.Value,
                                                        BetAmount = Math.Round(sbt.zz_BetAmount.Value, 2),
                                                        BetResult = (sbt.zz_BetResult != null ? Math.Round(sbt.zz_BetResult.Value, 2) : 0),
                                                        BetStatus = shared.sBetStatus(sbt.statuscode),
                                                        FavoriteOdds = (sb.zz_bOdds != null ? int.Parse(sb.zz_bOdds.ToString()) : 0),
                                                        FavoriteAmountPerDollar = sb.zz_bAmountWonPerDollar.Value,
                                                        // FavoriteAmount = (sb.zz_bAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolferId = u.zz_golferId.Value,
                                                        UnderdogGolfer = u.zz_name,
                                                        UnderdogOdds = (sb.zz_aOdds != null ? int.Parse(sb.zz_aOdds.ToString()) : 0),
                                                        UnderdogAmountPerDollar = sb.zz_aAmountWonPerDollar.Value,
                                                        // UnderdogAmount = (sb.zz_aAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolfTeamId = ugt.zz_golfteamId.Value,
                                                        UnderdogGolfTeam = ugt.zz_name,
                                                        UnderdogAccepted = (sbt.zz_aAccepted != null ? sbt.zz_aAccepted.Value : false),
                                                        FavoriteAccepted = (sbt.zz_bAccepted != null ? sbt.zz_bAccepted.Value : false),
                                                        TeamGolferName = (sbt.zz_aGolfTeamId.Id == golfTeam.GolfTeamId? u.zz_name:f.zz_name),
                                                        EventId = sb.zz_EventId.Id,
                                                        Day = sb.zz_Day,
                                                        isEventMatchup = (sb.zz_BetType.Value == 419260001 ? true : false),
                                                        //PlayerMatchupTitle = sb.zz_name,
                                                        PlayerMatchupTitle = (sbt.zz_aGolfTeamId.Id == golfTeam.GolfTeamId ? f.zz_name + " (" + (sb.zz_bOdds.Value==0?"E":sb.zz_bOdds.ToString()) + ") vs. " + u.zz_name + " (" + (sb.zz_aOdds.Value==0?"E":sb.zz_aOdds.ToString()) + ")" : u.zz_name + " (" + (sb.zz_aOdds.Value==0?"E":sb.zz_aOdds.ToString()) + ") vs. " + f.zz_name + " (" + (sb.zz_bOdds.Value==0?"E":sb.zz_bOdds.ToString()) + ")"),
                                                        PlayerMatchupId = sbt.zz_sidebetteamsId.Value,
                                                        UnderdogWon = (sb.zz_Winner == false ? true : false),
                                                        FavoriteWon = (sb.zz_Winner == true ? true : false),
                                                        isTie = (sb.zz_Winner == null ? true : false),
                                                        DayName = shared.sGetDay(sb.zz_Day),
                                                        DayNumber = shared.iGetDay(sb.zz_Day),
                                                        FavoriteAmount = Math.Round((sb.zz_bAmountWonPerDollar.Value * (sbt.zz_BetAmount==null?0:sbt.zz_BetAmount.Value)), 2),
                                                        UnderdogAmount = Math.Round((sb.zz_aAmountWonPerDollar.Value * (sbt.zz_BetAmount == null ? 0 : sbt.zz_BetAmount.Value)), 2),
                                                    }
                                                ).ToList<PlayerMatchup>().OrderBy(pm => pm.TeamGolferName).ToList();

                PlayerMatchups AllTeamMatchups = new PlayerMatchups
                    {
                        MatchupGolfTeamId = golfTeam.GolfTeamId,
                        MatchupGolfTeamName = golfTeam.GolfTeamName,
                    };

                AllTeamMatchups.EventMatchups = allMatchups.Where(am => am.DayNumber == 0 && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.ThursdayMatchups = allMatchups.Where(am => am.DayNumber == 1 && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.FridayMatchups = allMatchups.Where(am => am.DayNumber == 2 && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.SaturdayMatchups = allMatchups.Where(am => am.DayNumber == 3 && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.SundayMatchups = allMatchups.Where(am => am.DayNumber == 4 && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();

                decimal TotalWL = 0;
                decimal TotalWL_EventMatchups = 0;
                decimal TotalWL_ThursdayMatchups = 0;
                decimal TotalWL_FridayMatchups = 0;
                decimal TotalWL_SaturdayMatchups = 0;
                decimal TotalWL_SundayMatchups = 0;

                TotalWL_EventMatchups = AllTeamMatchups.EventMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_EventMatchups = TotalWL_EventMatchups + AllTeamMatchups.EventMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_ThursdayMatchups = AllTeamMatchups.ThursdayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups + AllTeamMatchups.ThursdayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_FridayMatchups = AllTeamMatchups.FridayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_FridayMatchups = TotalWL_FridayMatchups + AllTeamMatchups.FridayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_SaturdayMatchups = AllTeamMatchups.SaturdayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups + AllTeamMatchups.SaturdayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_SundayMatchups = AllTeamMatchups.SundayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_SundayMatchups = TotalWL_SundayMatchups + AllTeamMatchups.SundayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);

                TotalWL_EventMatchups = TotalWL_EventMatchups - AllTeamMatchups.EventMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_EventMatchups = TotalWL_EventMatchups - AllTeamMatchups.EventMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups - AllTeamMatchups.ThursdayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups - AllTeamMatchups.ThursdayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_FridayMatchups = TotalWL_FridayMatchups - AllTeamMatchups.FridayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_FridayMatchups = TotalWL_FridayMatchups - AllTeamMatchups.FridayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups - AllTeamMatchups.SaturdayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups - AllTeamMatchups.SaturdayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                TotalWL_SundayMatchups = TotalWL_SundayMatchups - AllTeamMatchups.SundayMatchups.Where(tm => tm.FavoriteGolfTeamId == golfTeam.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                TotalWL_SundayMatchups = TotalWL_SundayMatchups - AllTeamMatchups.SundayMatchups.Where(tm => tm.UnderdogGolfTeamId == golfTeam.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);

                TotalWL = TotalWL_EventMatchups + TotalWL_ThursdayMatchups + TotalWL_FridayMatchups + TotalWL_SaturdayMatchups + TotalWL_SundayMatchups;

                AllTeamMatchups.EventMatchupsWinLoss = TotalWL_EventMatchups;
                AllTeamMatchups.ThursdayMatchupsWinLoss = TotalWL_ThursdayMatchups;
                AllTeamMatchups.FridayMatchupsWinLoss = TotalWL_FridayMatchups;
                AllTeamMatchups.SaturdayMatchupsWinLoss = TotalWL_SaturdayMatchups;
                AllTeamMatchups.SundayMatchupsWinLoss = TotalWL_SundayMatchups;

                AllTeamMatchups.MatchupGolfTeamWinLoss = TotalWL;

                //Add Rounds - if this is a day matchup only add the one round
                AllTeamMatchups.ThursdayMatchups.ForEach(tm =>
                    {
                        if (Rounds.Count() != 0)
                        {
                            tm.Favorite.Rounds.Add(Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == 1).FirstOrDefault());
                            tm.Underdog.Rounds.Add(Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == 1).FirstOrDefault());
                        }
                    });

                AllTeamMatchups.FridayMatchups.ForEach(tm =>
                {
                    if (Rounds.Count() != 0)
                    {
                        tm.Favorite.Rounds.Add(Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == 2).FirstOrDefault());
                        tm.Underdog.Rounds.Add(Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == 2).FirstOrDefault());
                    }
                });

                AllTeamMatchups.SaturdayMatchups.ForEach(tm =>
                {
                    if (Rounds.Count() != 0)
                    {
                        tm.Favorite.Rounds.Add(Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == 3).FirstOrDefault());
                        tm.Underdog.Rounds.Add(Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == 3).FirstOrDefault());
                    }
                });

                AllTeamMatchups.SundayMatchups.ForEach(tm =>
                {
                    if (Rounds.Count() != 0)
                    {
                        tm.Favorite.Rounds.Add(Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == 4).FirstOrDefault());
                        tm.Underdog.Rounds.Add(Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == 4).FirstOrDefault());
                    }
                });

                AllTeamMatchups.EventMatchups.ForEach(tm =>
                {
                    if (Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).Count() != 0 && Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Count() != 0)
                    {
                        int MaxRound_Favorite = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Max(r => r.DayNumber);
                        int MaxRound_Underdog = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).ToList().Max(r => r.DayNumber);
                        tm.Favorite.Rounds.Add(Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == MaxRound_Favorite).FirstOrDefault());
                        tm.Underdog.Rounds.Add(Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == MaxRound_Underdog).FirstOrDefault());
                    }
                });

                TeamMatchups.TeamPlayerMatchups.Add(AllTeamMatchups);


                return TeamMatchups;                
            }
        }

        [WebMethod]
        public PlayerMatchupsByTeam PlayerMatchups2(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);




                EventAndLeague Event = (from e in datacontext.zz_eventSet
                                        join le in datacontext.zz_zz_event_zz_leagueSet on e.zz_eventId.Value equals le.zz_eventid.Value
                                        join gt in datacontext.zz_golfteamSet on le.zz_leagueid.Value equals gt.zz_LeagueId.Id
                                        where e.statecode.Value == 0
                                        where gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                        select new EventAndLeague
                                        {
                                            EventId = e.zz_eventId.Value,
                                            LeagueId = le.zz_leagueid.Value,
                                            EventName = e.zz_name
                                        }).FirstOrDefault();

                GolfTeam golfTeam = (from l in datacontext.zz_leagueSet
                                     join gt in datacontext.zz_golfteamSet on l.zz_leagueId.Value equals gt.zz_LeagueId.Id
                                     where
                                      gt.zz_golfteamId.Value == Guid.Parse(GolfTeamId)
                                     select new GolfTeam
                                     {
                                         GolfTeamName = gt.zz_name,
                                         GolfTeamId = gt.zz_golfteamId.Value,
                                         LeagueId = l.zz_leagueId.Value,
                                     }
                                 ).FirstOrDefault();

                List<Round> Rounds = (
                                from g in datacontext.zz_golferSet
                                join r in datacontext.zz_roundSet on g.zz_golferId.Value equals r.zz_GolferId.Id
                                where
                                    r.zz_EventId.Id == Event.EventId
                                select new Round
                                {
                                    GolferId = g.zz_golferId.Value,
                                    //RoundGolfTeamId = gt.zz_golfteamId.Value,
                                    TotalScore = (r.zz_TotalScore == null ? 0 : r.zz_TotalScore.Value),
                                    RoundName = r.zz_name,
                                    RoundId = r.zz_roundId.Value,
                                    RoundShots = r.zz_Shots.Value,
                                    RoundEventStatus = shared.sGetRoundEventStatus(r.zz_EventStatus),
                                    Thru = (r.zz_CompletedHoles == null ? 0 : r.zz_CompletedHoles.Value),
                                    RoundScore = (r.zz_Score == null ? 0 : r.zz_Score.Value),
                                    TournamentRank = r.zz_EventRank,
                                    TeeTime = (r.zz_TeeTime == null ? "" : r.zz_TeeTime.Value.ToLocalTime().ToShortTimeString()),
                                    Day = r.zz_Day,
                                    DayNumber = shared.iGetDay(r.zz_Day),
                                    DayName = shared.sGetDay(r.zz_Day),
                                    dtTeeTime = (r.zz_TeeTime == null ? DateTime.Now : r.zz_TeeTime.Value.ToLocalTime()),
                                    LastUpdated = r.ModifiedOn.Value.ToLocalTime(),
                                    RoundGolferEventStatus = shared.sGolferStatus(g.statuscode),
                                }
                            ).ToList<Round>();

                PlayerMatchupsByTeam TeamMatchups = new PlayerMatchupsByTeam
                {
                    EventId = Event.EventId 
                };

                List<PlayerMatchup> allMatchups = (
                                                    from sb in datacontext.zz_sidebetSet
                                                    join sbt in datacontext.zz_sidebetteamsSet on sb.zz_sidebetId equals sbt.zz_SideBetId.Id
                                                    //join e in datacontext.zz_eventSet on sb.zz_EventId.Id equals e.zz_eventId
                                                    join u in datacontext.zz_golferSet on sb.zz_aGolferId.Id equals u.zz_golferId
                                                    join f in datacontext.zz_golferSet on sb.zz_bGolferId.Id equals f.zz_golferId
                                                    join ugt in datacontext.zz_golfteamSet on sbt.zz_aGolfTeamId.Id equals ugt.zz_golfteamId
                                                    join fgt in datacontext.zz_golfteamSet on sbt.zz_bGolfTeamId.Id equals fgt.zz_golfteamId
                                                    where
                                                        sb.zz_EventId.Id == Event.EventId
                                                    //(
                                                    //    ugt.zz_LeagueId.Id == golfTeam.LeagueId
                                                    //    && ugt.zz_golfteamId.Value == golfTeam.GolfTeamId
                                                    //)
                                                    //||
                                                    //(
                                                    //    fgt.zz_LeagueId.Id == golfTeam.LeagueId
                                                    //    && fgt.zz_golfteamId.Value == golfTeam.GolfTeamId
                                                    //)
                                                    select new PlayerMatchup
                                                    {
                                                        FavoriteGolferId = f.zz_golferId.Value,
                                                        FavoriteGolfer = f.zz_name,
                                                        FavoriteGolfTeam = fgt.zz_name,
                                                        FavoriteGolfTeamId = fgt.zz_golfteamId.Value,
                                                        BetAmount = Math.Round(sbt.zz_BetAmount.Value, 0),
                                                        BetResult = (sbt.zz_BetResult != null ? Math.Round(sbt.zz_BetResult.Value, 2) : 0),
                                                        BetStatus = shared.sBetStatus(sbt.statuscode),
                                                        FavoriteOdds = (sb.zz_bOdds != null ? int.Parse(sb.zz_bOdds.ToString()) : 0),
                                                        FavoriteAmountPerDollar = sb.zz_bAmountWonPerDollar.Value,
                                                        // FavoriteAmount = (sb.zz_bAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolferId = u.zz_golferId.Value,
                                                        UnderdogGolfer = u.zz_name,
                                                        UnderdogOdds = (sb.zz_aOdds != null ? int.Parse(sb.zz_aOdds.ToString()) : 0),
                                                        UnderdogAmountPerDollar = sb.zz_aAmountWonPerDollar.Value,
                                                        // UnderdogAmount = (sb.zz_aAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolfTeamId = ugt.zz_golfteamId.Value,
                                                        UnderdogGolfTeam = ugt.zz_name,
                                                        UnderdogAccepted = (sbt.zz_aAccepted != null ? sbt.zz_aAccepted.Value : false),
                                                        FavoriteAccepted = (sbt.zz_bAccepted != null ? sbt.zz_bAccepted.Value : false),
                                                        TeamGolferName = (sbt.zz_aGolfTeamId.Id == golfTeam.GolfTeamId ? u.zz_name : f.zz_name),
                                                        Proposed = ((sbt.zz_aGolfTeamId.Id == golfTeam.GolfTeamId && sbt.zz_aAccepted == true) || (sbt.zz_bGolfTeamId.Id == golfTeam.GolfTeamId && sbt.zz_bAccepted == true) ? true : false),
                                                        EventId = sb.zz_EventId.Id,
                                                        Day = sb.zz_Day,
                                                        isEventMatchup = (sb.zz_BetType.Value == 419260001 ? true : false),
                                                        //PlayerMatchupTitle = sb.zz_name,
                                                        PlayerMatchupTitle = (sbt.zz_aGolfTeamId.Id == golfTeam.GolfTeamId ? f.zz_name + " vs. " + u.zz_name  : u.zz_name + " vs. " + f.zz_name),
                                                        PlayerMatchupId = sbt.zz_sidebetteamsId.Value,
                                                        UnderdogWon = (sb.zz_Winner == false ? true : false),
                                                        FavoriteWon = (sb.zz_Winner == true ? true : false),
                                                        isTie = (sb.zz_Winner == null ? true : false),
                                                        DayName = shared.sGetDay(sb.zz_Day),
                                                        DayNumber = shared.iGetDay(sb.zz_Day),
                                                        FavoriteAmount = Math.Round((sb.zz_bAmountWonPerDollar.Value * (sbt.zz_BetAmount == null ? 0 : sbt.zz_BetAmount.Value)), 2),
                                                        UnderdogAmount = Math.Round((sb.zz_aAmountWonPerDollar.Value * (sbt.zz_BetAmount == null ? 0 : sbt.zz_BetAmount.Value)), 2),
                                                        isFavorite = (fgt.zz_golfteamId.Value == golfTeam.GolfTeamId?true:false),
                                                        CloseTime = (sb.zz_CloseBetOn==null?"":sb.zz_CloseBetOn.Value.ToLocalTime().ToShortTimeString()),
                                                        BetClosed= (sb.zz_CloseBetOn==null?false:(sb.zz_CloseBetOn.Value < DateTime.UtcNow?true:false))
                                                    }
                                                ).ToList<PlayerMatchup>().OrderBy(pm => pm.TeamGolferName).OrderByDescending(em => em.DayNumber).ToList();

                List<PlayerMatchupProposal> AllProposals = (from sbtp in datacontext.zz_sidebetteamsproposalSet
                                                                join gt in datacontext.zz_golfteamSet on sbtp.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                                                join sbt in datacontext.zz_sidebetteamsSet on sbtp.zz_SideBetTeamsId.Id equals sbt.zz_sidebetteamsId.Value
                                                                join sb in datacontext.zz_sidebetSet on sbt.zz_SideBetId.Id equals sb.zz_sidebetId.Value
                                                                where
                                                                    sb.zz_EventId.Id == Event.EventId
                                                                select new PlayerMatchupProposal
                                                                {
                                                                    Name = sbtp.zz_name,
                                                                    PlayerMatchupId = sbt.zz_sidebetteamsId.Value,
                                                                    ProposedBy = gt.zz_name,
                                                                    GolfTeamId = gt.zz_golfteamId.Value,
                                                                    CreatedOn = sbtp.CreatedOn.Value,
                                                                    BaselineAmount = (sbtp.zz_Amount==null?0:sbtp.zz_Amount.Value),
                                                                    //GolferA = tm.UnderdogGolfer,
                                                                    //GolferB = tm.FavoriteGolfer,
                                                                    GolferAToWinAmount = Math.Round((sb.zz_aAmountWonPerDollar.Value * (sbtp.zz_Amount == null ? 0 : sbtp.zz_Amount.Value)), 2),
                                                                    GolferBToWinAmount = Math.Round((sb.zz_bAmountWonPerDollar.Value * (sbtp.zz_Amount == null ? 0 : sbtp.zz_Amount.Value)), 2),
                                                                    //PlayerMatchupId = sbt.zz_sidebetteamsId.Value
                                                                }).ToList<PlayerMatchupProposal>();

                PlayerMatchups AllTeamMatchups = new PlayerMatchups
                {
                    MatchupGolfTeamId = golfTeam.GolfTeamId,
                    MatchupGolfTeamName = golfTeam.GolfTeamName,
                };

                AllTeamMatchups.ActiveMatchups = allMatchups.Where(am => am.BetStatus == "Active" && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.CompletedMatchups = allMatchups.Where(am => am.BetStatus == "Completed" && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.ProposedMatchups = allMatchups.Where(am => am.BetStatus == "Proposed" && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();
                AllTeamMatchups.InProgressMatchups = allMatchups.Where(am => am.BetStatus == "Accepted" && (am.FavoriteGolfTeamId == golfTeam.GolfTeamId || am.UnderdogGolfTeamId == golfTeam.GolfTeamId)).ToList();

                AllTeamMatchups.ProposedMatchups.ForEach(tm =>
                {
                    tm.BetProposals = AllProposals.Where(ap => ap.PlayerMatchupId == tm.PlayerMatchupId).OrderByDescending(bp => bp.CreatedOn).ToList();
                });

                #region Inprogress Bets
                AllTeamMatchups.InProgressMatchups.ForEach(tm =>
                {
                    
                    Round MR_Team = new Round();
                    Round MR_Opponent = new Round();
                    if (tm.isFavorite)
                    {
                        int MaxRound_Team = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Max(r => r.DayNumber);
                        int MaxRound_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).ToList().Max(r => r.DayNumber);
                        MR_Team = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == MaxRound_Team).FirstOrDefault();
                        MR_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == MaxRound_Opponent).FirstOrDefault();
                    }
                    else
                    {
                        int MaxRound_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).ToList().Max(r => r.DayNumber);
                        int MaxRound_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Max(r => r.DayNumber);
                        MR_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == MaxRound_Opponent).FirstOrDefault();
                        MR_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == MaxRound_Team).FirstOrDefault();
                    }

                    //Matchup Scoring
                    if (MR_Team != null && MR_Opponent != null)
                    {
                        if (tm.isEventMatchup == true)
                        {
                            if (MR_Team.TotalScore == MR_Opponent.TotalScore)
                                tm.MatchupInProgress_Tied = true;
                            else
                                tm.MatchupInProgress_Tied = false;

                            if (tm.MatchupInProgress_Tied == false)
                            {
                                tm.MatchupInProgress_ScoreDelta = Math.Abs(MR_Team.TotalScore - MR_Opponent.TotalScore);
                                if (MR_Team.TotalScore < MR_Opponent.TotalScore)
                                {
                                    tm.WinAmount = (tm.isFavorite ? tm.FavoriteAmount : tm.UnderdogAmount);
                                    tm.MatchupInProgress_Winning = true;
                                }
                                else
                                {
                                    tm.MatchupInProgress_Winning = false;
                                    tm.LossAmount = (tm.isFavorite ? tm.UnderdogAmount : tm.FavoriteAmount) * -1;
                                }
                            }
                            else
                                tm.ResultAmount = 0;
                        }
                        else
                        {
                            //this should lookup by the day number of the matchup?
                            Round R_Team = new Round();
                            Round R_Opponent = new Round();

                            if (tm.isFavorite)
                            {
                                R_Team = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                                R_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                            }
                            else
                            {
                                R_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                                R_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                            }

                            if (R_Team != null && R_Opponent != null)
                            {
                                if (R_Team.RoundScore == R_Opponent.RoundScore)
                                    tm.MatchupInProgress_Tied = true;
                                else
                                    tm.MatchupInProgress_Tied = false;

                                if (tm.MatchupInProgress_Tied == false)
                                {
                                    tm.MatchupInProgress_ScoreDelta = Math.Abs(R_Team.RoundScore - R_Opponent.RoundScore);
                                    if (R_Team.RoundScore < R_Opponent.RoundScore)
                                    {
                                        tm.WinAmount = (tm.isFavorite ? tm.FavoriteAmount : tm.UnderdogAmount);
                                        tm.MatchupInProgress_Winning = true;
                                    }
                                    else
                                    {
                                        tm.LossAmount = (tm.isFavorite ? tm.UnderdogAmount : tm.FavoriteAmount) * -1;
                                        tm.MatchupInProgress_Winning = false;
                                    }
                                }
                                else
                                    tm.ResultAmount = 0;
                            }
                        }
                    }
                    else
                    {

                        tm.MatchupInProgress_Tied = true; //Should I leave this as null when no rounds have been created?
                    }

                    if (tm.isEventMatchup == true)
                    {
                        Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).OrderByDescending(r=>r.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Underdog.Rounds.Add(r);
                        });

                        Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).OrderByDescending(r => r.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Favorite.Rounds.Add(r);
                        });
                    }
                    else//if it's a day bet then only show rounds from that day.
                    {
                        Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Underdog.Rounds.Add(r);
                        });

                        Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Favorite.Rounds.Add(r);
                        });
                    }
                });

                #endregion

                #region Completed Bets

                AllTeamMatchups.CompletedMatchups.ForEach(tm =>
                {
                    Round MR_Team = new Round();
                    Round MR_Opponent = new Round();
                    if (tm.isFavorite)
                    {
                        int MaxRound_Team = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Max(r => r.DayNumber);
                        int MaxRound_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).ToList().Max(r => r.DayNumber);
                        MR_Team = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == MaxRound_Team).FirstOrDefault();
                        MR_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == MaxRound_Opponent).FirstOrDefault();
                    }
                    else
                    {
                        int MaxRound_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).ToList().Max(r => r.DayNumber);
                        int MaxRound_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).ToList().Max(r => r.DayNumber);
                        MR_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == MaxRound_Opponent).FirstOrDefault();
                        MR_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == MaxRound_Team).FirstOrDefault();
                    }

                    //Matchup Scoring
                    if (MR_Team != null && MR_Opponent != null)
                    {
                        if (tm.isEventMatchup == true)
                        {
                            if (MR_Team.TotalScore == MR_Opponent.TotalScore)
                                tm.MatchupInProgress_Tied = true;
                            else
                                tm.MatchupInProgress_Tied = false;

                            if (tm.MatchupInProgress_Tied == false)
                            {
                                tm.MatchupInProgress_ScoreDelta = Math.Abs(MR_Team.TotalScore - MR_Opponent.TotalScore);

                                if (MR_Team.TotalScore < MR_Opponent.TotalScore)
                                {
                                    tm.WonMatchup = true;
                                    tm.ResultAmount = (tm.isFavorite?tm.FavoriteAmount:tm.UnderdogAmount);
                                    tm.WinAmount = (tm.isFavorite ? tm.FavoriteAmount : tm.UnderdogAmount);
                                }
                                else
                                {
                                    tm.WonMatchup = false;
                                    tm.ResultAmount = (tm.isFavorite?tm.UnderdogAmount:tm.FavoriteAmount) * -1;
                                    tm.LossAmount = (tm.isFavorite ? tm.UnderdogAmount : tm.FavoriteAmount) * -1;
                                }
                            }
                            else
                                tm.ResultAmount = 0;
                        }
                        else
                        {
                            //this should lookup by the day number of the matchup?
                            Round R_Team = new Round();
                            Round R_Opponent = new Round();

                            if (tm.isFavorite)
                            {
                                R_Team = Rounds.Where(r =>r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                                R_Opponent = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                            }
                            else
                            {
                                R_Team = Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault();
                                R_Opponent = Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).FirstOrDefault(); 
                            }

                            if (R_Team != null && R_Opponent != null)
                            {
                                if (R_Team.RoundScore == R_Opponent.RoundScore)
                                    tm.MatchupInProgress_Tied = true;
                                else
                                    tm.MatchupInProgress_Tied = false;

                                if (tm.MatchupInProgress_Tied == false)
                                {
                                    tm.MatchupInProgress_ScoreDelta = Math.Abs(R_Team.RoundScore - R_Opponent.RoundScore);
                                    if (R_Team.RoundScore < R_Opponent.RoundScore)
                                    {
                                        tm.WonMatchup = true;
                                        tm.ResultAmount = (tm.isFavorite?tm.FavoriteAmount:tm.UnderdogAmount);
                                        tm.WinAmount = (tm.isFavorite ? tm.FavoriteAmount : tm.UnderdogAmount);
                                    }
                                    else
                                    {
                                        tm.WonMatchup = false;
                                        tm.ResultAmount = (tm.isFavorite?tm.UnderdogAmount:tm.FavoriteAmount) * -1;
                                        tm.LossAmount = (tm.isFavorite ? tm.UnderdogAmount : tm.FavoriteAmount) * -1;

                                    }
                                }
                                else
                                    tm.ResultAmount = 0;
                            }
                        }
                    }
                    else
                    {

                        tm.MatchupInProgress_Tied = true; //Should I leave this as null when no rounds have been created?
                    }

                    
                    if (tm.isEventMatchup == true)
                    {
                        Rounds.Where(r => r.GolferId == tm.UnderdogGolferId).OrderByDescending(r => r.DayNumber).ToList().ForEach(r =>
                    {
                        tm.Underdog.Rounds.Add(r);
                    });

                        Rounds.Where(r => r.GolferId == tm.FavoriteGolferId).OrderByDescending(r => r.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Favorite.Rounds.Add(r);
                        });
                    }
                    else//if it's a day bet then only show rounds from that day.
                    {
                        Rounds.Where(r => r.GolferId == tm.UnderdogGolferId && r.DayNumber == tm.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Underdog.Rounds.Add(r);
                        });

                        Rounds.Where(r => r.GolferId == tm.FavoriteGolferId && r.DayNumber == tm.DayNumber).ToList().ForEach(r =>
                        {
                            tm.Favorite.Rounds.Add(r);
                        });
                    }
                });
                #endregion

                TeamMatchups.TotalWL = AllTeamMatchups.CompletedMatchups.Sum(cm => cm.ResultAmount);
                TeamMatchups.AcceptedW = AllTeamMatchups.InProgressMatchups.Sum(cm => cm.WinAmount);
                TeamMatchups.AcceptedL = AllTeamMatchups.InProgressMatchups.Sum(cm => cm.LossAmount);

                TeamMatchups.TeamPlayerMatchups.Add(AllTeamMatchups);


                return TeamMatchups;
            }
        }

        [WebMethod]
        public PlayerMatchupsByTeam PlayerMatchup(string GolfTeamId, String SideBetTeamsId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);


                zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                PlayerMatchupsByTeam TeamMatchups = new PlayerMatchupsByTeam
                {
                    EventId = Event.zz_eventId.Value
                };

                PlayerMatchup Matchup = (
                                                    from sbt in datacontext.zz_sidebetteamsSet
                                                    join sb in datacontext.zz_sidebetSet on sbt.zz_SideBetId.Id equals sb.zz_sidebetId.Value
                                                    //join e in datacontext.zz_eventSet on sb.zz_EventId.Id equals e.zz_eventId
                                                    join u in datacontext.zz_golferSet on sb.zz_aGolferId.Id equals u.zz_golferId
                                                    join f in datacontext.zz_golferSet on sb.zz_bGolferId.Id equals f.zz_golferId
                                                    join ugt in datacontext.zz_golfteamSet on sbt.zz_aGolfTeamId.Id equals ugt.zz_golfteamId
                                                    join fgt in datacontext.zz_golfteamSet on sbt.zz_bGolfTeamId.Id equals fgt.zz_golfteamId
                                                    where
                                                        sb.zz_EventId.Id == Event.zz_eventId
                                                    where
                                                        sbt.zz_sidebetteamsId.Value == Guid.Parse(SideBetTeamsId)
                                                    select new PlayerMatchup
                                                    {
                                                        FavoriteGolferId = f.zz_golferId.Value,
                                                        FavoriteGolfer = f.zz_name,
                                                        FavoriteGolfTeam = fgt.zz_name,
                                                        FavoriteGolfTeamId = fgt.zz_golfteamId.Value,
                                                        BetAmount = Math.Round(sbt.zz_BetAmount.Value, 2),
                                                        BetResult = (sbt.zz_BetResult != null ? Math.Round(sbt.zz_BetResult.Value, 2) : 0),
                                                        BetStatus = shared.sBetStatus(sbt.statuscode),
                                                        FavoriteOdds = (sb.zz_bOdds != null ? int.Parse(sb.zz_bOdds.ToString()) : 0),
                                                        FavoriteAmountPerDollar = sb.zz_bAmountWonPerDollar.Value,
                                                        // FavoriteAmount = (sb.zz_bAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolferId = u.zz_golferId.Value,
                                                        UnderdogGolfer = u.zz_name,
                                                        UnderdogOdds = (sb.zz_aOdds != null ? int.Parse(sb.zz_aOdds.ToString()) : 0),
                                                        UnderdogAmountPerDollar = sb.zz_aAmountWonPerDollar.Value,
                                                        // UnderdogAmount = (sb.zz_aAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolfTeamId = ugt.zz_golfteamId.Value,
                                                        UnderdogGolfTeam = ugt.zz_name,
                                                        UnderdogAccepted = (sbt.zz_aAccepted != null ? sbt.zz_aAccepted.Value : false),
                                                        FavoriteAccepted = (sbt.zz_bAccepted != null ? sbt.zz_bAccepted.Value : false),

                                                        EventId = sb.zz_EventId.Id,
                                                        Day = sb.zz_Day,
                                                        isEventMatchup = (sb.zz_BetType.Value == 419260001 ? true : false),
                                                        PlayerMatchupTitle = sb.zz_name,
                                                        PlayerMatchupId = sbt.zz_sidebetteamsId.Value,
                                                        UnderdogWon = (sb.zz_Winner == false ? true : false),
                                                        FavoriteWon = (sb.zz_Winner == true ? true : false),
                                                        isTie = (sb.zz_Winner == null ? true : false),
                                                        DayName = shared.sGetDay(sb.zz_Day),
                                                        DayNumber = shared.iGetDay(sb.zz_Day),
                                                        FavoriteAmount = Math.Round((sb.zz_bAmountWonPerDollar.Value * (sbt.zz_BetAmount == null ? 0 : sbt.zz_BetAmount.Value)), 2),
                                                        UnderdogAmount = Math.Round((sb.zz_aAmountWonPerDollar.Value * (sbt.zz_BetAmount == null ? 0 : sbt.zz_BetAmount.Value)), 2),
                                                    }
                                                ).FirstOrDefault();
                if (Matchup != null)
                {
                    PlayerMatchups AllTeamMatchups = new PlayerMatchups();

                    //Get all proposals
                    List<PlayerMatchupProposal> AllProposals= (from sbtp in datacontext.zz_sidebetteamsproposalSet
                                            join gt in datacontext.zz_golfteamSet on sbtp.zz_GolfTeamId.Id equals gt.zz_golfteamId.Value
                                            join sbt in datacontext.zz_sidebetteamsSet on sbtp.zz_SideBetTeamsId.Id equals sbt.zz_sidebetteamsId.Value
                                            join sb in datacontext.zz_sidebetSet on sbt.zz_SideBetId.Id equals sb.zz_sidebetId.Value
                                            where
                                                sbtp.zz_SideBetTeamsId.Id == Matchup.PlayerMatchupId
                                            select new PlayerMatchupProposal
                                            {
                                                Name = sbtp.zz_name,
                                                ProposedBy = gt.zz_name,
                                                GolfTeamId = gt.zz_golfteamId.Value,
                                                CreatedOn = sbtp.CreatedOn.Value,
                                                BaselineAmount = sbtp.zz_Amount.Value,
                                                GolferA = Matchup.UnderdogGolfer,
                                                GolferB = Matchup.FavoriteGolfer,
                                                GolferAToWinAmount =  Math.Round((sb.zz_aAmountWonPerDollar.Value * (sbtp.zz_Amount == null ? 0 : sbtp.zz_Amount.Value)), 2),
                                                GolferBToWinAmount =  Math.Round((sb.zz_bAmountWonPerDollar.Value * (sbtp.zz_Amount == null ? 0 : sbtp.zz_Amount.Value)), 2),
                                                //PlayerMatchupId = sbt.zz_sidebetteamsId.Value
                                            }).ToList<PlayerMatchupProposal>();

                    Matchup.BetProposals = AllProposals.OrderByDescending(bp => bp.CreatedOn).ToList();
                    //{
                    //    //MatchupGolfTeamId = Matchup.GolfTeamId,
                    //    //MatchupGolfTeamName = golfTeam.GolfTeamName,
                        
                    //};
                    AllTeamMatchups.EventMatchups.Add(Matchup);
                    TeamMatchups.TeamPlayerMatchups.Add(AllTeamMatchups);
                }

                return TeamMatchups;
            }
        }

        [WebMethod]
        public EventXml GolfTeamDelegates(string GolfTeamId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();
            
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                EventXml xml = new EventXml();
                GolfTeam golfTeam = new GolfTeam();
                Guid golfteamId = new Guid();
                if (Guid.TryParse(GolfTeamId, out golfteamId))
                {
                    //golfTeam = (from gt in datacontext.zz_golfteamSet
                    //            where
                    //              gt.statecode == zz_golfteamState.Active
                    //            where
                    //                gt.zz_golfteamId.Value == golfteamId
                    //            select new GolfTeam
                    //            {
                    //                GolfTeamName = gt.zz_name,
                    //                GolfTeamId = gt.zz_golfteamId.Value,
                    //            }).FirstOrDefault();

                    List<GolfTeamDelegate> Delegates =
                    (from gtds in datacontext.zz_golfteamdelegateSet
                     join gtp in datacontext.zz_golfteamSet on gtds.zz_PrimaryGolfTeamId.Id equals gtp.zz_golfteamId
                     join gtd in datacontext.zz_golfteamSet on gtds.zz_DelegateGolfTeamId.Id equals gtd.zz_golfteamId
                     where gtds.zz_DelegateGolfTeamId.Id == golfteamId
                     where gtd.statecode.Value == 0
                     where gtp.statecode.Value == 0
                     select new GolfTeamDelegate
                     {
                         GolfTeamName = gtd.zz_name,
                         GolfTeamId = gtd.zz_golfteamId.Value,
                         DelegateGolfTeamName = gtp.zz_name,
                         DelegateGolfTeamId = gtp.zz_golfteamId.Value,
                     }).ToList<GolfTeamDelegate>().OrderBy(gtp => gtp.GolfTeamName).ToList();
                    
                     //golfTeam = Delegates.GroupBy(s => new { s.GolfTeamId, s.GolfTeamName }).Select(y => new GolfTeam()
                     //       {
                     //           GolfTeamId = y.Key.GolfTeamId,
                     //           GolfTeamName = y.Key.GolfTeamName,
                     //       }).ToList<GolfTeam>().FirstOrDefault();
                    GolfTeamDelegate Delegate = Delegates.FirstOrDefault();
                    if (Delegate != null)
                    {
                        golfTeam.GolfTeamId = Delegate.GolfTeamId;
                        golfTeam.GolfTeamName = Delegate.GolfTeamName;

                        golfTeam.Delegates = Delegates.Select(y => new GolfTeam()
                        {
                            GolfTeamId = y.DelegateGolfTeamId,
                            GolfTeamName = y.DelegateGolfTeamName,
                        }).ToList<GolfTeam>().OrderBy(d => d.GolfTeamName).ToList();
                    }
                    else
                    {
                        golfTeam = (from gt in datacontext.zz_golfteamSet
                                    where
                                      gt.statecode == zz_golfteamState.Active
                                    where
                                        gt.zz_golfteamId.Value == golfteamId
                                    select new GolfTeam
                                    {
                                        GolfTeamName = gt.zz_name,
                                        GolfTeamId = gt.zz_golfteamId.Value,
                                    }).FirstOrDefault();
                    }
                     

                     

                    xml.GolfTeams.Add(golfTeam);
                }
                else
                {
                    GolfTeam NoTeam = new GolfTeam();
                    NoTeam.Delegates = (from gt in datacontext.zz_golfteamSet
                                          where
                                            gt.statecode == zz_golfteamState.Active
                                          select new GolfTeam
                                          {
                                              GolfTeamName = gt.zz_name,
                                              GolfTeamId = gt.zz_golfteamId.Value,
                                          }).ToList<GolfTeam>();

                    xml.GolfTeams.Add(NoTeam);
                }
                //Event evnt = (from e in datacontext.zz_eventSet where e.statecode.Value == 0 select new Event { GolfEventId = e.zz_eventId.Value, GolfEventName = e.zz_name }).FirstOrDefault();

                

                ;
                //xml.EventId = evnt.GolfEventId;
                //xml.EventName = evnt.GolfEventName;
                
                return xml;
            }
        }

        [WebMethod]
        public EventXml GolfTeams()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);
                EventXml xml = new EventXml();
                List <GolfTeam> golfTeams  = (from gt in datacontext.zz_golfteamSet
                                     where
                                       gt.statecode == zz_golfteamState.Active
                                     select new GolfTeam
                                     {
                                         GolfTeamName = gt.zz_name,
                                         GolfTeamId = gt.zz_golfteamId.Value,
                                     }).ToList<GolfTeam>();

                xml.GolfTeams = golfTeams.OrderBy(g=>g.GolfTeamName).ToList();
                return xml;
            }
        }
      
        #endregion

        #region Draft
        [WebMethod]
        public GolfTeam GolfTeam_OnTheClock()
        {
            /*not in use*/
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            GolfTeam rtn = new GolfTeam();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                //zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                zz_golfteam OnTheClock = (
                                        from gt in datacontext.zz_golfteamSet
                                        where
                                         gt.statuscode != new OptionSetValue(419260000)
                                        select gt
                                    ).FirstOrDefault();
                if (OnTheClock != null)
                {
                    GolfTeam gt = new GolfTeam()
                    {
                        GolfTeamName = OnTheClock.zz_name,
                        GolfTeamId = OnTheClock.zz_golfteamId.Value
                    };

                    rtn = gt;
                }
              
            }
            return rtn;
        }

        [WebMethod]
        public String GolfTeamIdToName(String gtId)
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            String rtn = "";
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                Guid GolfTeamId = new Guid();
                if (Guid.TryParse(gtId, out GolfTeamId))
                {
                    zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.zz_golfteamId==GolfTeamId select gt).FirstOrDefault();
                    rtn = golfTeam.zz_name;
                }
            }
            return rtn;
        }

        [WebMethod]
        public Event EventLoadActive()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Event rtn = new Event();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                zz_event golfEvent = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                rtn.GolfEventId = golfEvent.zz_eventId.Value;
                rtn.GolfEventName = golfEvent.zz_name;
                rtn.StartDate = DateTime.Parse(golfEvent.zz_StartDate.ToString());
            }
            return rtn;
        }

        [WebMethod]
        public Boolean DraftGolfer(String EventOddsId,String GolfTeamId)
        {
            Boolean rtn = false;
            Guid gEventOddsId = new Guid();
            Guid gGolfTeamId = Guid.Parse(GolfTeamId);
            if (Guid.TryParse(EventOddsId, out gEventOddsId))
            {
                try
                {
                    ServerConnection serverConnect = new ServerConnection();
                    ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                    using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                    {
                        service.EnableProxyTypes();
                        OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                        ServiceContext datacontext = new ServiceContext(service);
                        //Get GolfTeam that is on the clock
                        zz_league league = (from gt in datacontext.zz_golfteamSet
                                            join l in datacontext.zz_leagueSet on gt.zz_LeagueId.Id equals l.zz_leagueId.Value
                                            where gt.zz_golfteamId.Value == gGolfTeamId
                                            select l).FirstOrDefault();

                        Guid LeagueId = league.zz_leagueId.Value;

                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.statuscode == new OptionSetValue(419260000) && gt.zz_LeagueId.Id == LeagueId select gt).FirstOrDefault();
                        //Ensure that the team that is on the clock is the one that made the draft request
                        if (Guid.TryParse(GolfTeamId, out gGolfTeamId))
                        {
                            if (golfTeam.zz_golfteamId != gGolfTeamId)
                                throw new SoapException("It is not your draft pick!", SoapException.ClientFaultCode);
                        }
                        else
                            //throw error
                            throw new SoapException("You are not logged in.", SoapException.ClientFaultCode);

                        zz_eventdraft eDraftPick = (from ed in datacontext.zz_eventdraftSet where ed.statuscode == new OptionSetValue(419260000) && ed.zz_LeagueId.Id == LeagueId select ed).FirstOrDefault();
                        int RoundsInDraft = (league.zz_RoundsInDraft==null?4:league.zz_RoundsInDraft.Value);
                        DraftPick draftPick =
                            (from e in datacontext.zz_eventSet
                             join eo in datacontext.zz_eventoddsSet on e.zz_eventId.Value equals eo.zz_EventId.Id
                             join g in datacontext.zz_golferSet on eo.zz_GolferId.Id equals g.zz_golferId.Value
                             where
                                eo.zz_eventoddsId == gEventOddsId
                             select new DraftPick
                             {
                                 EventId = e.zz_eventId.Value,
                                 EventName = e.zz_name,
                                 GolferId = g.zz_golferId.Value,
                                 GolferName = g.zz_name,
                                 RoundsInDraft = RoundsInDraft,
                                 EventOddsId = eo.zz_eventoddsId.Value
                             }
                            ).FirstOrDefault();

                        draftPick.LeagueId = LeagueId;
                        draftPick.GolfTeamId = golfTeam.zz_golfteamId.Value;
                        draftPick.GolfTeamName = golfTeam.zz_name;
                        draftPick.PickNumber = eDraftPick.zz_PickNumber.Value;
                        draftPick.EventDraftId = eDraftPick.zz_eventdraftId.Value;

                        //zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                        //get selected event odds record
//                        zz_eventodds eventOdd = (from o in datacontext.zz_eventoddsSet where o.zz_eventoddsId == gEventOddsId select o).FirstOrDefault();
                        
                        //get the league the golf team belongs to
                        //zz_league leauge = (from l in datacontext.zz_leagueSet where l.zz_leagueId.Value == golfTeam.zz_LeagueId.Id select l).FirstOrDefault();
                        
                        //Get Draft Pick that is on the clock
                        

                      // zz_golfer golfer = (from g in datacontext.zz_golferSet where g.zz_golferId == eventOdd.zz_GolferId.Id select g).FirstOrDefault();


                        //Update Event Draft with the selected golfer and new name
                        //Entity uEventDraft = service.Retrieve("zz_eventdraft", draftPick.EventDraftId, new ColumnSet(true));
                        //uEventDraft.Attributes["zz_golferid"] = new EntityReference(zz_golfer.EntityLogicalName, draftPick.GolferId);
                        //uEventDraft.Attributes["zz_name"] = draftPick.EventName + ": Pick# " + draftPick.PickNumber + " " + draftPick.GolfTeamName + " selects " + draftPick.GolferName;
                        //if (golfTeam.zz_LeagueId != null)
                        //    uEventDraft.Attributes["zz_leagueid"] = new EntityReference(zz_league.EntityLogicalName, golfTeam.zz_LeagueId.Id);
                        //service.Update(uEventDraft);

                        eDraftPick.zz_GolferId = new EntityReference(zz_golfer.EntityLogicalName, draftPick.GolferId);
                        eDraftPick.zz_name = draftPick.EventName + ": Pick# " + draftPick.PickNumber + " " + draftPick.GolfTeamName + " selects " + draftPick.GolferName;
                        eDraftPick.zz_LeagueId = new EntityReference(zz_league.EntityLogicalName, golfTeam.zz_LeagueId.Id);
                        datacontext.UpdateObject(eDraftPick);
                        service.Update(eDraftPick);

                        #region comment out when league logic in place
                        ////Update Golfer
                        //Entity uGolfer = service.Retrieve("zz_golfer", draftPick.GolferId, new ColumnSet(true));
                        //uGolfer.Attributes["zz_golfteamid"] = new EntityReference(zz_golfteam.EntityLogicalName, (Guid)golfTeam.zz_golfteamId);
                        //service.Update(uGolfer);

                        ////Event Odds
                        //SetStateRequest request = new SetStateRequest();
                        //request.EntityMoniker = new EntityReference(zz_eventodds.EntityLogicalName, draftPick.EventOddsId);
                        //request.State = new OptionSetValue((int)zz_eventoddsState.Active);
                        //request.Status = new OptionSetValue(419260000); //Inactive
                        //SetStateResponse response = (SetStateResponse)service.Execute(request);

                        #endregion
                        //Draft Pick
                        SetStateRequest requestdp = new SetStateRequest();
                        requestdp.EntityMoniker = new EntityReference(zz_eventdraft.EntityLogicalName, draftPick.EventDraftId);
                        requestdp.State = new OptionSetValue((int)zz_eventdraftState.Active);
                        requestdp.Status = new OptionSetValue(419260001); 
                        SetStateResponse responsedp = (SetStateResponse)service.Execute(requestdp);

                        //Golf Team - Take off clock
                        SetStateRequest requestgt = new SetStateRequest();
                        requestgt.EntityMoniker = new EntityReference(zz_golfteam.EntityLogicalName, golfTeam.zz_golfteamId.Value);
                        requestgt.State = new OptionSetValue((int)zz_golfteamState.Active);
                        requestgt.Status = new OptionSetValue(1); //Active
                        SetStateResponse responsegt = (SetStateResponse)service.Execute(requestgt);


                        ////Golfer - Set to Drafted
                        SetStateRequest requestg = new SetStateRequest();
                        requestg.EntityMoniker = new EntityReference(zz_golfer.EntityLogicalName, draftPick.GolferId);
                        requestg.State = new OptionSetValue((int)zz_golferState.Active);
                        requestg.Status = new OptionSetValue(1); //Active/Drafted
                        SetStateResponse responseg = (SetStateResponse)service.Execute(requestg);

                        List<zz_golfteam> GolfTeams = (from gt in datacontext.zz_golfteamSet where gt.statecode == zz_golfteamState.Active && gt.zz_DraftPosition != null && gt.zz_LeagueId.Id == golfTeam.zz_LeagueId.Id select gt).ToList<zz_golfteam>();
                        Int32 numberOfTeams = GolfTeams.Count();
                        Int32 NextDraftPosition = GetNextPickDraftPosition(draftPick.PickNumber, numberOfTeams);
                        zz_golfteam OnTheClock = GolfTeams.Where(s => s.zz_DraftPosition == NextDraftPosition).FirstOrDefault();

                        //Check to see if the draft is completed.
                        if ((draftPick.PickNumber / GolfTeams.Count) == draftPick.RoundsInDraft)
                        {
                            //complete draft
                            SetStateRequest requestdc = new SetStateRequest();
                            requestdc.EntityMoniker = new EntityReference(zz_league.EntityLogicalName, draftPick.LeagueId);
                            requestdc.State = new OptionSetValue((int)zz_leagueState.Active);
                            requestdc.Status = new OptionSetValue(419260000); //Draft Completed
                            SetStateResponse responsedc = (SetStateResponse)service.Execute(requestdc);
                        }
                        else
                        {
                            Guid pk = CreateDraftPick(service, draftPick, OnTheClock);

                            #region Set On the Clock Status
                            SetStateRequest requestotcED = new SetStateRequest();
                            requestotcED.EntityMoniker = new EntityReference(zz_eventdraft.EntityLogicalName, pk);
                            requestotcED.State = new OptionSetValue((int)zz_eventdraftState.Active);
                            requestotcED.Status = new OptionSetValue(419260000); //Inactive
                            SetStateResponse responseotcDP = (SetStateResponse)service.Execute(requestotcED);

                            SetStateRequest requestotcGT = new SetStateRequest();
                            requestotcGT.EntityMoniker = new EntityReference(zz_golfteam.EntityLogicalName, (Guid)OnTheClock.zz_golfteamId);
                            requestotcGT.State = new OptionSetValue((int)zz_golfteamState.Active);
                            requestotcGT.Status = new OptionSetValue(419260000); //Active
                            SetStateResponse responseotcGT = (SetStateResponse)service.Execute(requestotcGT);
                        }
                        #endregion
                    }
                }
                catch
                {
                    throw;
                }
            }

            return rtn;
        }

        private Guid CreateDraftPick(OrganizationServiceProxy service, DraftPick draftPick, zz_golfteam golfTeam)
        {
            Guid pk = new Guid();
            try
            {
                int NextPick = draftPick.PickNumber + 1;
                zz_eventdraft ed = new zz_eventdraft()
                {
                    zz_EventId =  new EntityReference(zz_event.EntityLogicalName,draftPick.EventId),
                    zz_GolfTeamId =  golfTeam.ToEntityReference(),
                    zz_PickNumber = NextPick,
                    zz_name = draftPick.EventName + ": Pick# " + NextPick.ToString(),
                    zz_Skins = true,
                    zz_LeagueId  = new EntityReference(zz_league.EntityLogicalName, draftPick.LeagueId),
                };
                
                pk = service.Create(ed);
            }
            catch
            {
                throw;
            }
            return pk;
        }

        private Int32 GetNextPickDraftPosition(Int32 last_pick_number, Int32 number_of_teams)
        {

            Int32 round_number = ((last_pick_number) / number_of_teams) + 1;
            Int32 pick_number_of_round = ((last_pick_number) % number_of_teams) + 1;
            if (round_number % 2 == 0)
            {
                pick_number_of_round = number_of_teams - pick_number_of_round + 1;
            }

            return pick_number_of_round;
        }

        #endregion

        #region Player Matchups
        [WebMethod]
        public Boolean AcceptBet(String PlayerMatchupId, String GolfTeamId)
        {
            Boolean rtn = false;
            Guid gPlayerMatchupId = new Guid();
            if (Guid.TryParse(PlayerMatchupId, out gPlayerMatchupId))
            {
                try
                {
                    ServerConnection serverConnect = new ServerConnection();
                    ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                    using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                    {
                        service.EnableProxyTypes();
                        OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                        ServiceContext datacontext = new ServiceContext(service);
                        //Get GolfTeam that is on the clock
                        zz_sidebetteams PlayerMatchupBet = (from sbt in datacontext.zz_sidebetteamsSet where sbt.zz_sidebetteamsId == gPlayerMatchupId select sbt).FirstOrDefault();
                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.zz_golfteamId == Guid.Parse(GolfTeamId) select gt).FirstOrDefault();
                        
                        //Team A Accepts Bet
                        
                            //Update Bet that player A accepts Bet
                            Entity uPlayerMatchup = service.Retrieve("zz_sidebetteams", gPlayerMatchupId, new ColumnSet(true));
                            if (PlayerMatchupBet.zz_bAccepted == true && PlayerMatchupBet.zz_aContactId.Id == golfTeam.zz_ContactId.Id)
                                uPlayerMatchup.Attributes["zz_aaccepted"] = true;
                            if (PlayerMatchupBet.zz_aAccepted == true && PlayerMatchupBet.zz_bContactId.Id == golfTeam.zz_ContactId.Id)
                                uPlayerMatchup.Attributes["zz_baccepted"] = true;

                            service.Update(uPlayerMatchup);
                        
                            //Update Status To Active/Accepted
                            SetStateRequest request = new SetStateRequest();
                            request.EntityMoniker = new EntityReference(zz_sidebetteams.EntityLogicalName, gPlayerMatchupId);
                            request.State = new OptionSetValue((int)zz_sidebetteamsState.Active);
                            request.Status = new OptionSetValue(419260000); //Accepted
                            SetStateResponse response = (SetStateResponse)service.Execute(request);

                            zz_sidebetteamsproposal proposal = new zz_sidebetteamsproposal
                            {
                                zz_SideBetTeamsId = new EntityReference(zz_sidebetteams.EntityLogicalName, PlayerMatchupBet.zz_sidebetteamsId.Value),
                                zz_GolfTeamId = new EntityReference(zz_golfteam.EntityLogicalName, golfTeam.zz_golfteamId.Value),
                                zz_name = "Accepted By " + golfTeam.zz_name,
                            };
                            service.Create(proposal);

                            ////Update Side Bet to Accepted
                            //SetStateRequest request2 = new SetStateRequest();
                            //request2.EntityMoniker = new EntityReference(zz_sidebet.EntityLogicalName, PlayerMatchupBet.zz_SideBetId.Id);
                            //request2.State = new OptionSetValue((int)zz_sidebetState.Active);
                            //request2.Status = new OptionSetValue(419260000); //Accepted
                            //SetStateResponse response2 = (SetStateResponse)service.Execute(request2);
                        

                    }
                }
                catch
                {
                    throw;
                }
            }

            return rtn;
        }

        [WebMethod]
        public Boolean DeclineBet(String PlayerMatchupId, String GolfTeamId)
        {
            Boolean rtn = false;
            Guid gPlayerMatchupId = new Guid();
            if (Guid.TryParse(PlayerMatchupId, out gPlayerMatchupId))
            {
                try
                {
                    ServerConnection serverConnect = new ServerConnection();
                    ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                    using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                    {
                        service.EnableProxyTypes();
                        OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                        ServiceContext datacontext = new ServiceContext(service);
                        //Get GolfTeam that is on the clock
                        zz_sidebetteams PlayerMatchupBet = (from sbt in datacontext.zz_sidebetteamsSet where sbt.zz_sidebetteamsId == gPlayerMatchupId select sbt).FirstOrDefault();
                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.zz_golfteamId == Guid.Parse(GolfTeamId) select gt).FirstOrDefault();

                        //Update accepted to false for both to "reset" the bet.
                        Entity uPlayerMatchup = service.Retrieve("zz_sidebetteams", gPlayerMatchupId, new ColumnSet(true));
                        uPlayerMatchup.Attributes["zz_baccepted"] = false;
                        uPlayerMatchup.Attributes["zz_aaccepted"] = false;
                        service.Update(uPlayerMatchup);

                        //Update Status To Inactive/Declined
                        SetStateRequest request = new SetStateRequest();
                        request.EntityMoniker = new EntityReference(zz_sidebetteams.EntityLogicalName, gPlayerMatchupId);
                        request.State = new OptionSetValue((int)zz_sidebetteamsState.Inactive);
                        request.Status = new OptionSetValue(419260001); //Declined
                        SetStateResponse response = (SetStateResponse)service.Execute(request);


                        ////Update Side Bet to Closed Not Accepted
                        //SetStateRequest request2 = new SetStateRequest();
                        //request2.EntityMoniker = new EntityReference(zz_sidebet.EntityLogicalName, PlayerMatchupBet.zz_SideBetId.Id);
                        //request2.State = new OptionSetValue((int)zz_sidebetState.Inactive);
                        //request2.Status = new OptionSetValue(419260001); //Closed Not Accepted
                        //SetStateResponse response2 = (SetStateResponse)service.Execute(request2);
                        

                    }
                }
                catch
                {
                    throw;
                }
            }

            return rtn;
        }

        [WebMethod]
        public Boolean ProposeBet(String PlayerMatchupId, String GolfTeamId, String Amount)
        {
            Boolean rtn = false;
            Guid gPlayerMatchupId = new Guid();
            decimal dAmount;
            if (Guid.TryParse(PlayerMatchupId, out gPlayerMatchupId) && decimal.TryParse(Amount,out dAmount))
            {
                try
                {
                    ServerConnection serverConnect = new ServerConnection();
                    ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                    using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                    {
                        service.EnableProxyTypes();
                        OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                        ServiceContext datacontext = new ServiceContext(service);
                        //Get GolfTeam that is on the clock
                        zz_sidebetteams PlayerMatchupBet = (from sbt in datacontext.zz_sidebetteamsSet where sbt.zz_sidebetteamsId == gPlayerMatchupId select sbt).FirstOrDefault();
                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.zz_golfteamId == Guid.Parse(GolfTeamId) select gt).FirstOrDefault();

                        //No Bet has been proposed yet.
                        if ((PlayerMatchupBet.zz_bAccepted == null || PlayerMatchupBet.zz_bAccepted == false) && (PlayerMatchupBet.zz_aAccepted ==null || PlayerMatchupBet.zz_aAccepted == false))
                        {
                            //Update Bet that player A accepts Bet
                            Entity uPlayerMatchup = service.Retrieve("zz_sidebetteams", gPlayerMatchupId, new ColumnSet(true));
                            if (PlayerMatchupBet.zz_aContactId.Id == golfTeam.zz_ContactId.Id)
                                uPlayerMatchup.Attributes["zz_aaccepted"] = true;
                            if (PlayerMatchupBet.zz_bContactId.Id == golfTeam.zz_ContactId.Id)
                                uPlayerMatchup.Attributes["zz_baccepted"] = true;
                            uPlayerMatchup.Attributes["zz_betamount"] = new Money(dAmount);
                            service.Update(uPlayerMatchup);

                            //Update Status To Active/Proposed
                            SetStateRequest request = new SetStateRequest();
                            request.EntityMoniker = new EntityReference(zz_sidebetteams.EntityLogicalName, gPlayerMatchupId);
                            request.State = new OptionSetValue((int)zz_sidebetteamsState.Active);
                            request.Status = new OptionSetValue(419260002); //Proposed
                            SetStateResponse response = (SetStateResponse)service.Execute(request);
                        }

                        //Create new Proposal Record
                        zz_sidebetteamsproposal proposal = new zz_sidebetteamsproposal
                        {
                            zz_SideBetTeamsId = new EntityReference(zz_sidebetteams.EntityLogicalName, PlayerMatchupBet.zz_sidebetteamsId.Value),
                            zz_GolfTeamId = new  EntityReference(zz_golfteam.EntityLogicalName,golfTeam.zz_golfteamId.Value),
                            zz_Amount = new Money(dAmount),
                            zz_name = "$" + dAmount.ToString() + " By " + golfTeam.zz_name,
                        };
                        service.Create(proposal);
                    }
                }
                catch
                {
                    throw;
                }
            }

            return rtn;
        }

        [WebMethod]
        public Boolean CounterOfferBet(String PlayerMatchupId, String GolfTeamId, String Amount)
        {
            Boolean rtn = false;
            Guid gPlayerMatchupId = new Guid();
            decimal dAmount;
            if (Guid.TryParse(PlayerMatchupId, out gPlayerMatchupId) && decimal.TryParse(Amount, out dAmount))
            {
                try
                {
                    ServerConnection serverConnect = new ServerConnection();
                    ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                    using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                    {
                        service.EnableProxyTypes();
                        OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                        ServiceContext datacontext = new ServiceContext(service);
                        //Get GolfTeam that is on the clock
                        zz_sidebetteams PlayerMatchupBet = (from sbt in datacontext.zz_sidebetteamsSet where sbt.zz_sidebetteamsId == gPlayerMatchupId select sbt).FirstOrDefault();
                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.zz_golfteamId == Guid.Parse(GolfTeamId) select gt).FirstOrDefault();

                        //Update Bet that player A accepts Bet
                        Entity uPlayerMatchup = service.Retrieve("zz_sidebetteams", gPlayerMatchupId, new ColumnSet(true));
                        if (PlayerMatchupBet.zz_aContactId.Id == golfTeam.zz_ContactId.Id)
                        {
                            uPlayerMatchup.Attributes["zz_aaccepted"] = true;
                            uPlayerMatchup.Attributes["zz_baccepted"] = false;
                        }
                        if (PlayerMatchupBet.zz_bContactId.Id == golfTeam.zz_ContactId.Id)
                        {
                            uPlayerMatchup.Attributes["zz_baccepted"] = true;
                            uPlayerMatchup.Attributes["zz_aaccepted"] = false;
                        }
                        uPlayerMatchup.Attributes["zz_betamount"] = new Money(dAmount);
                        service.Update(uPlayerMatchup);


                        //Create new Proposal Record
                        zz_sidebetteamsproposal proposal = new zz_sidebetteamsproposal
                        {
                            zz_SideBetTeamsId = new EntityReference(zz_sidebetteams.EntityLogicalName, PlayerMatchupBet.zz_sidebetteamsId.Value),
                            zz_GolfTeamId = new EntityReference(zz_golfteam.EntityLogicalName, golfTeam.zz_golfteamId.Value),
                            zz_Amount = new Money(dAmount),
                            zz_name = "$" + dAmount.ToString() + " By " + golfTeam.zz_name,
                        };
                        service.Create(proposal);

                    }
                }
                catch
                {
                    throw;
                }
            }

            return rtn;
        }

        #endregion

        #region Legacy Code
        [WebMethod]
        public List<HomePageNotification> HomePageNotifications()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            List<HomePageNotification> rtn = new List<HomePageNotification>();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                //zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                zz_event Event = (from e in datacontext.zz_eventSet where e.zz_eventId == Guid.Parse("9D4B5EF9-DED1-E211-BDB6-3EBB89D5579F") select e).FirstOrDefault();




            }
            return rtn;
        }

        [WebMethod]
        public List<HomePageMenuItem> HomePageBuilder()
        {
            /*not in use!!*/
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            List<HomePageMenuItem> rtn = new List<HomePageMenuItem>();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                //zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                zz_event Event = (from e in datacontext.zz_eventSet where e.zz_eventId == Guid.Parse("9D4B5EF9-DED1-E211-BDB6-3EBB89D5579F") select e).FirstOrDefault();


                HomePageMenuItem Teams = new HomePageMenuItem()
                {
                    hRef = "Teams/Teams.html",
                    Title = "Teams"
                };

                HomePageMenuItem GolferLeaderboard = new HomePageMenuItem()
                {
                    hRef = "GolferLeaderboard/GolferLeaderboard.html",
                    Title = "Leaderboard",
                    Type = "Golfer",
                    EventId = (Guid)Event.zz_eventId
                };
                GolferLeaderboard.GetWinner(datacontext);

                HomePageMenuItem TeamTop2 = new HomePageMenuItem()
                {
                    hRef = "TeamTop2/TeamTop2.html",
                    Title = "Team Top 2",
                    Type = "TeamTop2",
                    Day = 4,
                    EventId = (Guid)Event.zz_eventId
                };
                TeamTop2.GetWinner(datacontext);
                HomePageMenuItem BestBallThru = new HomePageMenuItem()
                {
                    hRef = "BestBall/Thursday/Teams.html",
                    Title = "Best Ball Thursday",
                    Day = 1,
                    Type = "BestBall",
                    EventId = (Guid)Event.zz_eventId
                };
                BestBallThru.GetWinner(datacontext);

                HomePageMenuItem BestBallFri = new HomePageMenuItem()
                {
                    hRef = "BestBall/Friday/Teams.html",
                    Title = "Best Ball Friday",
                    Day = 2,
                    Type = "BestBall",
                    EventId = (Guid)Event.zz_eventId
                };
                BestBallFri.GetWinner(datacontext);

                HomePageMenuItem SkinsSat = new HomePageMenuItem()
                {
                    hRef = "Skins/Saturday/Skins.html",
                    Title = "Skins Saturday",
                    HasCount = true,
                    Day = 3,
                    Type = "Skins",
                    EventId = (Guid)Event.zz_eventId
                };
                SkinsSat.GetSkinsCount(datacontext);

                HomePageMenuItem SkinsSun = new HomePageMenuItem()
                {
                    hRef = "Skins/Sunday/Skins.html",
                    Title = "Skins Sunday",
                    HasCount = true,
                    Day = 4,
                    Type = "Skins",
                    EventId = (Guid)Event.zz_eventId
                };
                SkinsSun.GetSkinsCount(datacontext);

                rtn.Add(Teams);
                rtn.Add(GolferLeaderboard);
                rtn.Add(TeamTop2);
                rtn.Add(BestBallThru);
                rtn.Add(BestBallFri);
                rtn.Add(SkinsSat);
                rtn.Add(SkinsSun);

            }
            return rtn;
        }

        [WebMethod]
        public PlayerMatchupsByTeam AllPlayerMatchups()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                PlayerMatchups AllMatchups = new PlayerMatchups();
                PlayerMatchupsByTeam TeamMatchups = new PlayerMatchupsByTeam();
                Shared shared = new Shared();
                List<AllData> AllData = (
                                            from ed in datacontext.zz_eventdraftSet
                                            join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                            join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                            join r in datacontext.zz_roundSet on g.zz_golferId equals r.zz_GolferId.Id
                                            where
                                                ed.zz_EventId.Id == Event.zz_eventId
                                            where
                                                r.zz_EventId.Id == Event.zz_eventId
                                            select new AllData
                                            {
                                                GolfTeamId = gt.zz_golfteamId.Value,
                                                GolfTeam = gt.zz_name,
                                                GolferId = g.zz_golferId.Value,
                                                Golfer = g.zz_name,
                                                GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                                GolferMissedCut = (g.statecode == zz_golferState.Inactive && g.statuscode.Value == 2 ? true : false),
                                                RoundId = r.zz_roundId.Value,
                                                RoundName = r.zz_name,
                                                RoundScore = int.Parse(r.zz_Score.ToString()),
                                                RoundShots = int.Parse(r.zz_Shots.ToString()),
                                                TotalScore = (int)r.zz_TotalScore,
                                                Thru = (int)r.zz_CompletedHoles,
                                                TeeTime = (r.zz_TeeTime == null ? "" : DateTime.Parse(r.zz_TeeTime.ToString()).AddHours(-7).ToShortTimeString().ToString()),
                                                dtTeeTime = r.zz_TeeTime.Value,
                                                LastUpdated = (r.zz_AutoScoreLastUpdatedOn == null ? new DateTime() : DateTime.Parse(r.zz_AutoScoreLastUpdatedOn.ToString()).AddHours(-7)),
                                                //ScorecardUrl = r.zz_ScorecardUrl,
                                                Day = r.zz_Day,
                                                RoundEventStatus = shared.sGetRoundEventStatus(r.zz_EventStatus),
                                                //PickNumber = int.Parse(ed.zz_PickNumber.ToString()),
                                            }
                                        ).ToList<AllData>();


                List<PlayerMatchup> allMatchups = (
                                                    from sb in datacontext.zz_sidebetSet
                                                    join sbt in datacontext.zz_sidebetteamsSet on sb.zz_sidebetId equals sbt.zz_SideBetId.Id
                                                    join e in datacontext.zz_eventSet on sb.zz_EventId.Id equals e.zz_eventId
                                                    join u in datacontext.zz_golferSet on sb.zz_aGolferId.Id equals u.zz_golferId
                                                    join f in datacontext.zz_golferSet on sb.zz_bGolferId.Id equals f.zz_golferId
                                                    join ugt in datacontext.zz_golfteamSet on sbt.zz_aGolfTeamId.Id equals ugt.zz_golfteamId
                                                    join fgt in datacontext.zz_golfteamSet on sbt.zz_bGolfTeamId.Id equals fgt.zz_golfteamId
                                                    where
                                                        sb.zz_EventId.Id == Event.zz_eventId
                                                    //    && sb.statuscode.Value != 419260001 //Inactive/Closed Not Accepted
                                                    //where
                                                    //    sbt.statuscode.Value != 419260001 //Inactive/Declined
                                                    select new PlayerMatchup
                                                    {
                                                        FavoriteGolferId = f.zz_golferId.Value,
                                                        FavoriteGolfer = f.zz_name,
                                                        FavoriteGolfTeam = fgt.zz_name,
                                                        FavoriteGolfTeamId = fgt.zz_golfteamId.Value,
                                                        BetAmount = Math.Round(sbt.zz_BetAmount.Value, 2),
                                                        BetResult = (sbt.zz_BetResult != null ? Math.Round(sbt.zz_BetResult.Value, 2) : 0),
                                                        BetStatus = shared.sBetStatus(sbt.statuscode),
                                                        FavoriteOdds = (sb.zz_bOdds != null ? int.Parse(sb.zz_bOdds.ToString()) : 0),
                                                        FavoriteAmountPerDollar = sb.zz_bAmountWonPerDollar.Value,
                                                        // FavoriteAmount = (sb.zz_bAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolferId = u.zz_golferId.Value,
                                                        UnderdogGolfer = u.zz_name,
                                                        UnderdogOdds = (sb.zz_aOdds != null ? int.Parse(sb.zz_aOdds.ToString()) : 0),
                                                        UnderdogAmountPerDollar = sb.zz_aAmountWonPerDollar.Value,
                                                        // UnderdogAmount = (sb.zz_aAmountWonPerDollar.Value * sbt.zz_BetAmount.Value),
                                                        UnderdogGolfTeamId = ugt.zz_golfteamId.Value,
                                                        UnderdogGolfTeam = ugt.zz_name,
                                                        UnderdogAccepted = (sbt.zz_aAccepted != null ? sbt.zz_aAccepted.Value : false),
                                                        FavoriteAccepted = (sbt.zz_bAccepted != null ? sbt.zz_bAccepted.Value : false),

                                                        EventId = e.zz_eventId.Value,
                                                        Day = sb.zz_Day,
                                                        isEventMatchup = (sb.zz_BetType.Value == 419260001 ? true : false),
                                                        PlayerMatchupTitle = sb.zz_name,
                                                        PlayerMatchupId = sbt.zz_sidebetteamsId.Value,
                                                        UnderdogWon = (sb.zz_Winner == false ? true : false),
                                                        FavoriteWon = (sb.zz_Winner == true ? true : false),
                                                        isTie = (sb.zz_Winner == null ? true : false),
                                                        DayName = shared.sGetDay(sb.zz_Day),
                                                        DayNumber = shared.iGetDay(sb.zz_Day)
                                                    }
                                                ).ToList<PlayerMatchup>();

                List<GolfTeam> GolfTeams = new List<GolfTeam>();

                allMatchups.GroupBy(s => new { s.FavoriteGolfTeamId, s.FavoriteGolfTeam }).Select(y => new GolfTeam()
                {
                    GolfTeamId = y.Key.FavoriteGolfTeamId,
                    GolfTeamName = y.Key.FavoriteGolfTeam,
                }).ToList<GolfTeam>().ForEach(gt => GolfTeams.Add(gt));


                allMatchups.GroupBy(s => new { s.UnderdogGolfTeamId, s.UnderdogGolfTeam }).Select(y => new GolfTeam()
                {
                    GolfTeamId = y.Key.UnderdogGolfTeamId,
                    GolfTeamName = y.Key.UnderdogGolfTeam,
                }).ToList<GolfTeam>().ForEach(gt => GolfTeams.Add(gt));

                //GolfTeams = GolfTeams.Distinct().ToList<GolfTeam>();
                GolfTeams = GolfTeams.GroupBy(x => x.GolfTeamId).Select(y => y.First()).ToList();
                List<PlayerMatchups> unOrderedMatchups = new List<PlayerMatchups>();
                foreach (GolfTeam gt in GolfTeams)
                {
                    PlayerMatchups TeamMatchup = new PlayerMatchups();
                    TeamMatchup.MatchupGolfTeamId = gt.GolfTeamId;
                    TeamMatchup.MatchupGolfTeamName = gt.GolfTeamName;

                    List<PlayerMatchup> filterMatchups = new List<PlayerMatchup>();


                    allMatchups.Where(am => am.FavoriteGolfTeamId == gt.GolfTeamId).ToList().ForEach(pm => filterMatchups.Add(pm));
                    allMatchups.Where(am => am.UnderdogGolfTeamId == gt.GolfTeamId).ToList().ForEach(pm => filterMatchups.Add(pm));

                    TeamMatchup.EventMatchups = populateMatchups(AllData, filterMatchups, 0);
                    TeamMatchup.ThursdayMatchups = populateMatchups(AllData, filterMatchups, 1);
                    TeamMatchup.FridayMatchups = populateMatchups(AllData, filterMatchups, 2);
                    TeamMatchup.SaturdayMatchups = populateMatchups(AllData, filterMatchups, 3);
                    TeamMatchup.SundayMatchups = populateMatchups(AllData, filterMatchups, 4);

                    decimal TotalWL = 0;
                    decimal TotalWL_EventMatchups = 0;
                    decimal TotalWL_ThursdayMatchups = 0;
                    decimal TotalWL_FridayMatchups = 0;
                    decimal TotalWL_SaturdayMatchups = 0;
                    decimal TotalWL_SundayMatchups = 0;

                    TotalWL_EventMatchups = TeamMatchup.EventMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_EventMatchups = TotalWL_EventMatchups + TeamMatchup.EventMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_ThursdayMatchups = TeamMatchup.ThursdayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups + TeamMatchup.ThursdayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_FridayMatchups = TeamMatchup.FridayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_FridayMatchups = TotalWL_FridayMatchups + TeamMatchup.FridayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_SaturdayMatchups = TeamMatchup.SaturdayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups + TeamMatchup.SaturdayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_SundayMatchups = TeamMatchup.SundayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_SundayMatchups = TotalWL_SundayMatchups + TeamMatchup.SundayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);

                    TotalWL_EventMatchups = TotalWL_EventMatchups - TeamMatchup.EventMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_EventMatchups = TotalWL_EventMatchups - TeamMatchup.EventMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups - TeamMatchup.ThursdayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_ThursdayMatchups = TotalWL_ThursdayMatchups - TeamMatchup.ThursdayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_FridayMatchups = TotalWL_FridayMatchups - TeamMatchup.FridayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_FridayMatchups = TotalWL_FridayMatchups - TeamMatchup.FridayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups - TeamMatchup.SaturdayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_SaturdayMatchups = TotalWL_SaturdayMatchups - TeamMatchup.SaturdayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);
                    TotalWL_SundayMatchups = TotalWL_SundayMatchups - TeamMatchup.SundayMatchups.Where(tm => tm.FavoriteGolfTeamId == gt.GolfTeamId && tm.UnderdogWon).Sum(b => b.BetResult);
                    TotalWL_SundayMatchups = TotalWL_SundayMatchups - TeamMatchup.SundayMatchups.Where(tm => tm.UnderdogGolfTeamId == gt.GolfTeamId && tm.FavoriteWon).Sum(b => b.BetResult);

                    TotalWL = TotalWL_EventMatchups + TotalWL_ThursdayMatchups + TotalWL_FridayMatchups + TotalWL_SaturdayMatchups + TotalWL_SundayMatchups;

                    //TeamMatchup.EventMatchupsWinLoss = (TotalWL_EventMatchups >= 0? "$" + TotalWL_EventMatchups.ToString():"-$" + (TotalWL_EventMatchups * -1).ToString());
                    //TeamMatchup.ThursdayMatchupsWinLoss = (TotalWL_ThursdayMatchups >= 0 ? "$" + TotalWL_ThursdayMatchups.ToString() : "-$" + (TotalWL_ThursdayMatchups * -1).ToString());
                    //TeamMatchup.FridayMatchupsWinLoss = (TotalWL_FridayMatchups >= 0 ? "$" + TotalWL_FridayMatchups.ToString() : "-$" + (TotalWL_FridayMatchups * -1).ToString());
                    //TeamMatchup.SaturdayMatchupsWinLoss = (TotalWL_SaturdayMatchups >= 0 ? "$" + TotalWL_SaturdayMatchups.ToString() : "-$" + (TotalWL_SaturdayMatchups * -1).ToString());
                    //TeamMatchup.SundayMatchupsWinLoss = (TotalWL_SundayMatchups >= 0 ? "$" + TotalWL_SundayMatchups.ToString() : "-$" + (TotalWL_SundayMatchups * -1).ToString());
                    //TeamMatchup.MatchupGolfTeamWinLoss = (TotalWL >= 0 ? "$" + TotalWL.ToString() : "-$" + (TotalWL * -1).ToString());
                    unOrderedMatchups.Add(TeamMatchup);
                }
                TeamMatchups.EventId = Event.zz_eventId.Value;
                TeamMatchups.TeamPlayerMatchups = unOrderedMatchups.OrderBy(tm => tm.MatchupGolfTeamName).ToList();

                return TeamMatchups;
            }
        }
        private List<PlayerMatchup> populateMatchups(List<AllData> AllData, List<PlayerMatchup> allMatchups, int DayNumber)
        {
            Shared shared = new Shared();
            List<PlayerMatchup> rtn = new List<PlayerMatchup>();
            foreach (PlayerMatchup pm in allMatchups.Where(pm => pm.DayNumber == DayNumber))
            {
                pm.FavoriteAmount = Math.Round((pm.FavoriteAmountPerDollar * pm.BetAmount), 2);
                pm.UnderdogAmount = Math.Round((pm.UnderdogAmountPerDollar * pm.BetAmount), 2);

                pm.Favorite = AllData.Where(s => s.GolferId == pm.FavoriteGolferId).GroupBy(s => new
                {
                    s.GolferId,
                    s.Golfer,
                    s.GolferImgUrl,
                    s.GolfTeamId,
                    s.GolfTeam,
                    s.PickNumber,
                    s.GolferMissedCut
                }).Select(y => new Golfer()
                {
                    GolferId = y.Key.GolferId,
                    GolferName = y.Key.Golfer,
                    GolferImgUrl = y.Key.GolferImgUrl,
                    GolfTeamId = y.Key.GolfTeamId,
                    GolferGolfTeam = y.Key.GolfTeam,
                    DraftPickNumber = y.Key.PickNumber,
                    MissedCut = y.Key.GolferMissedCut
                }).FirstOrDefault();

                pm.Underdog = AllData.Where(s => s.GolferId == pm.UnderdogGolferId).GroupBy(s => new
                {
                    s.GolferId,
                    s.Golfer,
                    s.GolferImgUrl,
                    s.GolfTeamId,
                    s.GolfTeam,
                    s.PickNumber,
                    s.GolferMissedCut
                }).Select(y => new Golfer()
                {
                    GolferId = y.Key.GolferId,
                    GolferName = y.Key.Golfer,
                    GolferImgUrl = y.Key.GolferImgUrl,
                    GolfTeamId = y.Key.GolfTeamId,
                    GolferGolfTeam = y.Key.GolfTeam,
                    DraftPickNumber = y.Key.PickNumber,
                    MissedCut = y.Key.GolferMissedCut
                }).FirstOrDefault();


                //Add Player Round or Rounds if this is an event bet.
                if (pm.Favorite != null)
                {
                    pm.Favorite.Rounds = AllData.Where(s => s.GolferId == pm.FavoriteGolferId).OrderByDescending(y => y.Day.Value).GroupBy(s => new
                    {
                        s.RoundId,
                        s.RoundName,
                        s.RoundScore,
                        s.TeeTime,
                        s.dtTeeTime,
                        s.Thru,
                        s.RoundShots,
                        s.TotalScore,
                        s.LastUpdated,
                        s.ScorecardUrl,
                        s.Day,
                        s.RoundEventStatus,
                        s.GolferId
                    }).Select(y => new Round()
                    {
                        RoundId = y.Key.RoundId,
                        RoundName = y.Key.RoundName,
                        RoundScore = y.Key.RoundScore,
                        TeeTime = y.Key.TeeTime,
                        dtTeeTime = y.Key.dtTeeTime,
                        Thru = y.Key.Thru,
                        RoundShots = y.Key.RoundShots,
                        TotalScore = y.Key.TotalScore,
                        ScorecardUrl = y.Key.ScorecardUrl,
                        Day = y.Key.Day,
                        RoundEventStatus = y.Key.RoundEventStatus,
                        GolferId = y.Key.GolferId,
                        DayNumber = shared.iGetDay(y.Key.Day)
                    }).ToList<Round>();

                    //if not an event bet remove unneeded rounds?
                    if (pm.isEventMatchup == false)
                    {
                        List<Round> rnds = pm.Favorite.Rounds.ToList<Round>();
                        foreach (Round rnd in rnds)
                        {
                            if (pm.Day != null)
                            {
                                if (rnd.Day.Value != pm.Day.Value)
                                    pm.Favorite.Rounds.Remove(rnd);
                            }
                        }
                    }
                    Round r = pm.Favorite.Rounds.OrderByDescending(s => s.DayNumber).FirstOrDefault();
                    if (r != null)
                    {
                        pm.Favorite.GolferScore = r.TotalScore;
                        pm.Favorite.RoundInProgress = r.RoundName;
                        pm.Favorite.NextTeeTime = r.TeeTime;
                        if (r.Thru > 0)
                            pm.Favorite.TodaysRoundStarted = true;
                    }
                }

                if (pm.Underdog != null)
                {
                    //Add Player Round or Rounds if this is an event bet.
                    pm.Underdog.Rounds = AllData.Where(s => s.GolferId == pm.UnderdogGolferId).OrderByDescending(y => y.Day.Value).GroupBy(s => new
                    {
                        s.RoundId,
                        s.RoundName,
                        s.RoundScore,
                        s.TeeTime,
                        s.dtTeeTime,
                        s.Thru,
                        s.RoundShots,
                        s.TotalScore,
                        s.LastUpdated,
                        s.ScorecardUrl,
                        s.Day,
                        s.RoundEventStatus,
                        s.GolferId
                    }).Select(y => new Round()
                    {
                        RoundId = y.Key.RoundId,
                        RoundName = y.Key.RoundName,
                        RoundScore = y.Key.RoundScore,
                        TeeTime = y.Key.TeeTime,
                        dtTeeTime = y.Key.dtTeeTime,
                        Thru = y.Key.Thru,
                        RoundShots = y.Key.RoundShots,
                        TotalScore = y.Key.TotalScore,
                        ScorecardUrl = y.Key.ScorecardUrl,
                        Day = y.Key.Day,
                        RoundEventStatus = y.Key.RoundEventStatus,
                        GolferId = y.Key.GolferId,
                        DayNumber = shared.iGetDay(y.Key.Day)
                    }).ToList<Round>();

                    //if not an event bet remove unneeded rounds?
                    if (pm.isEventMatchup == false)
                    {
                        List<Round> rnds = pm.Underdog.Rounds.ToList<Round>();
                        foreach (Round rnd in rnds)
                        {
                            if (pm.Day != null)
                            {
                                if (rnd.Day.Value != pm.Day.Value)
                                    pm.Underdog.Rounds.Remove(rnd);
                            }
                        }
                    }

                    Round ur = pm.Underdog.Rounds.OrderByDescending(s => s.DayNumber).FirstOrDefault();
                    if (ur != null)
                    {
                        pm.Underdog.GolferScore = ur.TotalScore;
                        pm.Underdog.RoundInProgress = ur.RoundName;
                        pm.Underdog.NextTeeTime = ur.TeeTime;
                        if (ur.Thru > 0)
                            pm.Underdog.TodaysRoundStarted = true;
                    }
                }

                rtn.Add(pm);
            }
            return rtn.OrderBy(m => m.PlayerMatchupTitle).ToList();
        }

        [WebMethod]
        public String PlayerMatchupsRefresh()
        {
            try
            {
                PlayerMatchupsByTeam Matchups = AllPlayerMatchups();
                //PlayerMatchup pm = Matchups.TeamPlayerMatchups.FirstOrDefault().EventMatchups.FirstOrDefault();
                foreach (PlayerMatchups pms in Matchups.TeamPlayerMatchups)
                {
                    string Path = "xml/" + Matchups.EventId + "_" + pms.MatchupGolfTeamId + "_PlayerMatchups.xml";
                    BuildXmlFile(Path, pms);
                }
                return "Success!";
            }
            catch
            {
                return "Failed";
                throw;
            }
        }
        private void BuildXmlFile(String fileName, PlayerMatchupsByTeam xMatchups)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(xMatchups.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, xMatchups);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, PlayerMatchups xMatchups)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(xMatchups.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, xMatchups);
                }
            }
            catch
            {
                throw;
            }
        }


        [WebMethod]
        public Event AllData()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();
            Event E = new Event();
            try
            {
                using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                {
                    service.EnableProxyTypes();
                    OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                    ServiceContext datacontext = new ServiceContext(service);

                    // zz_event fEvent = (from e in datacontext.zz_eventSet where e.zz_eventId == Guid.Parse("B12691B0-9FD7-E211-BDB6-3EBB89D5579F") select e).FirstOrDefault();
                    zz_event fEvent = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                    List<AllEventData> AllEvent = (from e in datacontext.zz_eventSet
                                                   join ed in datacontext.zz_eventdraftSet on e.zz_eventId equals ed.zz_EventId.Id
                                                   join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                                   join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                                   join c in datacontext.zz_courseSet on e.zz_CourseId.Id equals c.zz_courseId
                                                   join ch in datacontext.zz_courseholeSet on c.zz_courseId equals ch.zz_CourseId.Id
                                                   join chh in datacontext.zz_courseholehandicapSet on ch.zz_courseholeId equals chh.zz_CourseHoleId.Id
                                                   where e.zz_eventId == fEvent.zz_eventId
                                                   where ed.zz_EventId.Id == fEvent.zz_eventId
                                                   select new AllEventData
                                                   {
                                                       EventId = e.zz_eventId.Value,
                                                       EventName = e.zz_name,
                                                       EventStartDate = DateTime.Parse(e.zz_StartDate.ToString()),
                                                       EventStatusName = shared.sGetEventStatus(e.statuscode),
                                                       GolfTeamId = gt.zz_golfteamId.Value,
                                                       GolfTeam = gt.zz_name,
                                                       GolferId = g.zz_golferId.Value,
                                                       Golfer = g.zz_name,
                                                       GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                                       PickNumber = int.Parse(ed.zz_PickNumber.ToString()),
                                                       CourseId = c.zz_courseId.Value,
                                                       CourseName = c.zz_name,
                                                       CourseHoleId = ch.zz_courseholeId.Value,
                                                       CourseHoleNumber = int.Parse(ch.zz_HoleNumber.ToString()),
                                                       Par = int.Parse(ch.zz_Par.ToString()),
                                                       CourseHandicap = int.Parse(ch.zz_Handicap.ToString()),
                                                       CourseHoleHandicapId = chh.zz_courseholehandicapId.Value,
                                                       CourseHoleHandicapHoleNumber = int.Parse(ch.zz_HoleNumber.ToString()),
                                                       Day = new OptionSetValue(int.Parse(chh.zz_Day.Value.ToString())),
                                                       CalculatedHandicap = decimal.Parse(chh.zz_HandicapCalculated.ToString()),
                                                       CalculatedHandicapRank = int.Parse(chh.zz_HandicapCalculatedRank.ToString()),
                                                   }
                                       ).ToList<AllEventData>();
                    //Event has not started yet.
                    if (AllEvent.Count() == 0)
                    {
                        AllEvent = (from e in datacontext.zz_eventSet
                                    join c in datacontext.zz_courseSet on e.zz_CourseId.Id equals c.zz_courseId
                                    join ch in datacontext.zz_courseholeSet on c.zz_courseId equals ch.zz_CourseId.Id
                                    join ed in datacontext.zz_eventdraftSet on e.zz_eventId equals ed.zz_EventId.Id
                                    join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                    join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                    where
                                        e.zz_eventId == fEvent.zz_eventId
                                    where
                                        ed.zz_EventId.Id == fEvent.zz_eventId
                                    select new AllEventData
                                    {
                                        EventId = e.zz_eventId.Value,
                                        EventName = e.zz_name,
                                        EventStartDate = DateTime.Parse(e.zz_StartDate.ToString()),
                                        EventStatusName = shared.sGetEventStatus(e.statuscode),
                                        GolfTeamId = gt.zz_golfteamId.Value,
                                        GolfTeam = gt.zz_name,
                                        GolferId = g.zz_golferId.Value,
                                        Golfer = g.zz_name,
                                        GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                        PickNumber = int.Parse(ed.zz_PickNumber.ToString()),
                                        CourseId = c.zz_courseId.Value,
                                        CourseName = c.zz_name,
                                        CourseHoleId = ch.zz_courseholeId.Value,
                                        DraftPickCreatedOn = ed.CreatedOn.Value,
                                        DraftPickCompletedOn = ed.ModifiedOn.Value,
                                        CourseHoleNumber = int.Parse(ch.zz_HoleNumber.ToString()),
                                    }
                                       ).ToList<AllEventData>();

                    }
                    //first draft pick ..need to figure out left join!
                    if (AllEvent.Count() == 0)
                    {
                        AllEvent = (from e in datacontext.zz_eventSet
                                    join ed in datacontext.zz_eventdraftSet on e.zz_eventId equals ed.zz_EventId.Id
                                    join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                    where e.zz_eventId == fEvent.zz_eventId
                                    select new AllEventData
                                    {
                                        EventId = e.zz_eventId.Value,
                                        EventName = e.zz_name,
                                        EventStartDate = DateTime.Parse(e.zz_StartDate.ToString()),
                                        PickNumber = int.Parse(ed.zz_PickNumber.ToString()),
                                        GolfTeamId = gt.zz_golfteamId.Value,
                                        GolfTeam = gt.zz_name,
                                    }
                                       ).ToList<AllEventData>();
                    }

                    //Pre Draft
                    if (AllEvent.Count() == 0)
                    {
                        AllEvent = (from e in datacontext.zz_eventSet
                                    where e.zz_eventId == fEvent.zz_eventId
                                    select new AllEventData
                                    {
                                        EventId = e.zz_eventId.Value,
                                        EventName = e.zz_name,
                                        EventStartDate = DateTime.Parse(e.zz_StartDate.ToString()),
                                    }
                                       ).ToList<AllEventData>();
                    }
                    Event eEvent = AllEvent.GroupBy(s => new { s.EventId, s.EventName, s.EventStartDate, s.EventStatusName }).Select(y => new Event()
                    {
                        GolfEventId = y.Key.EventId,
                        GolfEventName = y.Key.EventName,
                        StartDate = y.Key.EventStartDate,
                        EventStatusName = y.Key.EventStatusName,
                    }).FirstOrDefault();

                    E.GolfEventName = eEvent.GolfEventName;
                    E.GolfEventId = eEvent.GolfEventId;
                    E.StartDate = eEvent.StartDate;
                    E.EventStatusName = eEvent.EventStatusName;

                    E.EventCourse = AllEvent.GroupBy(s => new { s.CourseId, s.CourseName }).Select(y => new Course()
                    {
                        CourseId = y.Key.CourseId,
                        CourseName = y.Key.CourseName
                    }).FirstOrDefault();

                    E.EventCourse.CourseHoles = AllEvent.GroupBy(s => new
                    {
                        s.CourseHoleId,
                        s.CourseHoleNumber,
                        s.CourseHandicap,
                        s.Par,
                    }).Select(y => new CourseHole()
                    {
                        CourseHoleId = y.Key.CourseHoleId,
                        Par = y.Key.Par,
                        CourseHoleNumber = y.Key.CourseHoleNumber,
                        CourseHandicap = y.Key.CourseHandicap
                    }).ToList<CourseHole>();

                    foreach (CourseHole ch in E.EventCourse.CourseHoles)
                    {
                        ch.HoleHandicaps = AllEvent.Where(s => s.CourseHoleId == ch.CourseHoleId).GroupBy(s => new
                        {
                            s.CourseHoleHandicapId,
                            s.CourseHoleHandicapHoleNumber,
                            s.CalculatedHandicap,
                            s.CalculatedHandicapRank,
                            s.Day
                        }).Select(y => new CourseHoleHandicap()
                        {
                            CourseHoleHandicapId = y.Key.CourseHoleHandicapId,
                            CourseHoleHandicapHoleNumber = y.Key.CourseHoleHandicapHoleNumber,
                            Handicap = y.Key.CalculatedHandicap,
                            HandicapRank = y.Key.CalculatedHandicapRank,
                            Day = y.Key.Day
                        }).ToList<CourseHoleHandicap>();
                    }

                    List<AllData> AllData = (
                                            from ed in datacontext.zz_eventdraftSet
                                            join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                            join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                            join r in datacontext.zz_roundSet on g.zz_golferId equals r.zz_GolferId.Id
                                            where
                                                ed.zz_EventId.Id == eEvent.GolfEventId
                                            where
                                                r.zz_EventId.Id == eEvent.GolfEventId
                                            select new AllData
                                            {
                                                GolfTeamId = gt.zz_golfteamId.Value,
                                                GolfTeam = gt.zz_name,
                                                GolferId = g.zz_golferId.Value,
                                                Golfer = g.zz_name,
                                                GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + g.zz_CBSSportsGolferId.ToString() + @".jpg",
                                                GolferRotoWorldUrl = @"http://www.rotoworld.com/player/gol/" + g.zz_rotoworldId.ToString(),
                                                GolferMissedCut = (g.statecode == zz_golferState.Inactive && g.statuscode.Value == 2 ? true : false),
                                                GolferStatus = shared.sGolferStatus(g.statuscode),
                                                RoundId = r.zz_roundId.Value,
                                                RoundName = r.zz_name,
                                                RoundScore = int.Parse(r.zz_Score.ToString()),
                                                RoundShots = int.Parse(r.zz_Shots.ToString()),
                                                TotalScore = (int)r.zz_TotalScore,
                                                Thru = (int)r.zz_CompletedHoles,
                                                TeeTime = (r.zz_TeeTime == null ? "" : DateTime.Parse(r.zz_TeeTime.ToString()).AddHours(-7).ToShortTimeString().ToString()),
                                                dtTeeTime = r.zz_TeeTime.Value,
                                                LastUpdated = (r.zz_AutoScoreLastUpdatedOn == null ? new DateTime() : DateTime.Parse(r.zz_AutoScoreLastUpdatedOn.ToString()).AddHours(-7)),
                                                ScorecardUrl = r.zz_ScorecardUrl,
                                                Day = r.zz_Day,
                                                RoundEventStatus = shared.sGetRoundEventStatus(r.zz_EventStatus),
                                                EventStatusName = eEvent.EventStatusName,
                                                PickNumber = int.Parse(ed.zz_PickNumber.ToString()),
                                                //OnTheClockSince = ed.CreatedOn.Value,
                                                TournamentRank = r.zz_EventRank,
                                                //HoleId = h.zz_holeId.ToString(),
                                                //BestBall = bool.Parse(h.zz_BestBall.ToString()),
                                                //Skin = bool.Parse(h.zz_Skins.ToString()),
                                                //HoleShots = int.Parse(h.zz_Shots.ToString()),
                                                //HoleNumber = int.Parse(h.zz_HoleNumber.ToString()),

                                                //HoleId = (h == null ? String.Empty : h.zz_holeId.ToString()),
                                                //HoleNumber = (h == null ? 0 : int.Parse(h.zz_HoleNumber.ToString())),
                                                //HoleShots = (h == null ? 0 : int.Parse(h.zz_Shots.ToString())),
                                                //Skin = (h == null ? false : bool.Parse(h.zz_Skins.ToString())),
                                                //BestBall = (h == null ? false : bool.Parse(h.zz_BestBall.ToString())),
                                            }
                                        ).ToList<AllData>();

                    //have to get hole data seperate - thanks to me not know how to do a fucking left join
                    List<Hole> allHoles = (
                                            from ed in datacontext.zz_eventdraftSet
                                            join gt in datacontext.zz_golfteamSet on ed.zz_GolfTeamId.Id equals gt.zz_golfteamId
                                            join g in datacontext.zz_golferSet on ed.zz_GolferId.Id equals g.zz_golferId
                                            join r in datacontext.zz_roundSet on g.zz_golferId equals r.zz_GolferId.Id
                                            join h in datacontext.zz_holeSet on r.zz_roundId equals h.zz_RoundId.Id
                                            where
                                                 ed.zz_EventId.Id == eEvent.GolfEventId
                                            where
                                                r.zz_EventId.Id == eEvent.GolfEventId
                                            select new Hole
                                            {
                                                HoleNumber = int.Parse(h.zz_HoleNumber.ToString()),
                                                Shots = int.Parse(h.zz_Shots.ToString()),
                                                HoleId = h.zz_holeId.Value,
                                                RoundId = r.zz_roundId.Value,
                                                Skin = bool.Parse(h.zz_Skins.ToString()),
                                                BestBall = bool.Parse(h.zz_BestBall.ToString()),
                                                HoleGolfer = g.zz_name,
                                                HoleDay = r.zz_Day,
                                                GolfTeamId = gt.zz_golfteamId.Value,
                                                BestBallTieBreaker = bool.Parse(h.zz_BestBallTieBreaker.ToString()),
                                                HoleGolfTeam = gt.zz_name
                                            }
                                           ).ToList<Hole>();

                    if (AllData.Count != 0)
                    {
                        E.GolfTeams = AllData.GroupBy(s => new { s.GolfTeamId, s.GolfTeam, s.OnTheClock, s.OnTheClockSince }).Select(y => new GolfTeam()
                        {
                            GolfTeamId = y.Key.GolfTeamId,
                            GolfTeamName = y.Key.GolfTeam,
                            OnTheClock = y.Key.OnTheClock,
                            OnTheClockSince = y.Key.OnTheClockSince
                        }).ToList<GolfTeam>();


                        foreach (GolfTeam team in E.GolfTeams)
                        {
                            List<Golfer> usGolfers = AllData.Where(s => s.GolfTeamId == team.GolfTeamId).GroupBy(s => new
                            {
                                s.GolferId,
                                s.Golfer,
                                s.GolferImgUrl,
                                s.GolfTeamId,
                                s.GolfTeam,
                                s.PickNumber,
                                s.GolferMissedCut,
                                s.GolferStatus,
                                s.GolferRotoWorldUrl

                            }).Select(y => new Golfer()
                            {
                                GolferId = y.Key.GolferId,
                                GolferName = y.Key.Golfer,
                                GolferImgUrl = y.Key.GolferImgUrl,
                                GolfTeamId = y.Key.GolfTeamId,
                                GolferGolfTeam = y.Key.GolfTeam,
                                DraftPickNumber = y.Key.PickNumber,
                                MissedCut = y.Key.GolferMissedCut,
                                GolferStatus = y.Key.GolferStatus,
                                GolferRotoWorldUrl = y.Key.GolferRotoWorldUrl
                            }).ToList<Golfer>();



                            foreach (Golfer golfer in usGolfers)
                            {

                                List<Round> usRounds = AllData.Where(s => s.GolferId == golfer.GolferId).OrderByDescending(y => y.Day.Value).GroupBy(s => new
                                {
                                    s.RoundId,
                                    s.RoundName,
                                    s.RoundScore,
                                    s.TeeTime,
                                    s.dtTeeTime,
                                    s.Thru,
                                    s.RoundShots,
                                    s.TotalScore,
                                    s.LastUpdated,
                                    s.ScorecardUrl,
                                    s.Day,
                                    s.RoundEventStatus,
                                    s.GolferId,
                                    s.TournamentRank
                                }).Select(y => new Round()
                                {
                                    RoundId = y.Key.RoundId,
                                    RoundName = y.Key.RoundName,
                                    RoundScore = y.Key.RoundScore,
                                    TeeTime = y.Key.TeeTime,
                                    dtTeeTime = y.Key.dtTeeTime,
                                    Thru = y.Key.Thru,
                                    RoundShots = y.Key.RoundShots,
                                    TotalScore = y.Key.TotalScore,
                                    ScorecardUrl = y.Key.ScorecardUrl,
                                    Day = y.Key.Day,
                                    RoundEventStatus = y.Key.RoundEventStatus,
                                    GolferId = y.Key.GolferId,
                                    DayNumber = shared.iGetDay(y.Key.Day),
                                    TournamentRank = y.Key.TournamentRank
                                }).ToList<Round>();


                                foreach (Round round in usRounds)
                                {
                                    round.Holes = allHoles.Where(s => s.RoundId == round.RoundId).OrderBy(h => h.HoleNumber).ToList<Hole>();
                                    if (round.DayNumber == 2)
                                        E.isDay1 = false;
                                }

                                Round r = usRounds.OrderByDescending(s => s.DayNumber).FirstOrDefault();
                                golfer.GolferScore = r.TotalScore;
                                golfer.RoundInProgress = r.RoundName;
                                golfer.NextTeeTime = r.TeeTime;
                                golfer.dtNextTeeTime = r.dtTeeTime;

                                golfer.TournamentRank = r.TournamentRank;
                                if (r.Thru > 0)
                                    golfer.TodaysRoundStarted = true;


                                //round.Holes
                                golfer.Rounds = usRounds.OrderByDescending(rnd => rnd.DayNumber).ToList();


                            }

                            team.Golfers = usGolfers.OrderBy(g => g.GolferScore).ThenBy(g => g.dtNextTeeTime).ThenBy(g => g.DraftPickNumber).ToList();

                            List<NumberRank> Top2 = new List<NumberRank>();
                            //if(E.isDay1==true)
                            //    Top2 = team.Golfers.Where(g=>g.TodaysRoundStarted ==true).ToList().OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                            //else
                            List<Golfer> ActiveGolfers = team.Golfers.Where(x => x.GolferStatus == "Active").ToList();

                            switch (ActiveGolfers.Count())
                            {
                                case 1:
                                    team.TeamTop2Score = 99;
                                    break;
                                case 2:
                                    Top2 = ActiveGolfers.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().ToList<NumberRank>();
                                    team.TeamTop2Score = Top2.Sum(s => s.Number);
                                    break;
                                case 3:
                                    Top2 = ActiveGolfers.OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 1).ToList<NumberRank>();
                                    team.TeamTop2Score = Top2.Sum(s => s.Number);
                                    break;
                                default:
                                    Top2 = team.Golfers.Where(g => g.GolferStatus == "Active").OrderByDescending(n => n.GolferScore).Select((n, i) => new NumberRank(n.GolferScore, i + 1)).ToList().Where(f => f.Rank > 2).ToList<NumberRank>();
                                    team.TeamTop2Score = Top2.Sum(s => s.Number);
                                    break;
                            }




                            GolfTeam otc = E.GolfTeams.Where(s => s.OnTheClock == true).FirstOrDefault();
                            if (otc != null)
                                E.GolfTeamOnTheClock = otc.GolfTeamName;

                            E.DraftPicks = AllEvent.GroupBy(s => new { s.GolfTeam, s.Golfer, s.PickNumber, s.GolferImgUrl }).Select(y => new EventDraft()
                            {
                                EventDraftGolfer = y.Key.Golfer,
                                EventDraftGolfTeam = y.Key.GolfTeam,
                                EventDraftPickNumber = y.Key.PickNumber,
                                GolferImgUrl = y.Key.GolferImgUrl,
                            }).ToList<EventDraft>();
                        }


                        E.CalculateHoles();
                        E.GolfTeams.ToList().ForEach(u =>
                        {
                            u.BestBallHoles_Thurs = allHoles.Where(s => s.GolfTeamId == u.GolfTeamId && s.BestBall == true && s.HoleDay.Value == 419260000).ToList<Hole>();
                            u.BestBallHoles_Fri = allHoles.Where(s => s.GolfTeamId == u.GolfTeamId && s.BestBall == true && s.HoleDay.Value == 419260001).ToList<Hole>();
                            u.BestBallHoles_Thurs.ToList().ForEach(h =>
                            {
                                h.TeamPlays = (from ah in allHoles
                                               where
                                                   ah.GolfTeamId == u.GolfTeamId
                                                   && ah.HoleDay.Value == h.HoleDay.Value
                                                   && ah.HoleNumber == h.HoleNumber
                                               select new PossibleHole
                                               {
                                                   pHoleNumber = ah.HoleNumber,
                                                   pHoleHoleGolfer = ah.HoleGolfer,
                                                   pHoleShots = ah.Shots,
                                                   pHoleScoreName = ah.ScoreName,
                                                   pHoleBestBall = ah.BestBall,
                                                   pHoleParentHoleId = h.HoleId
                                               }).ToList<PossibleHole>();
                            });
                            u.BestBallScore_Thurs = u.BestBallHoles_Thurs.Sum(s => s.Shots);
                            if (u.BestBallHoles_Thurs.Where(s => s.BestBallTieBreaker == true).Count() > 0)
                                u.BestBallTieBreaker_Thurs = true;

                            u.BestBallHoles_Fri.ToList().ForEach(h =>
                            {
                                h.TeamPlays = (from ah in allHoles
                                               where
                                                  ah.GolfTeamId == u.GolfTeamId
                                                  && ah.HoleDay.Value == h.HoleDay.Value
                                                  && ah.HoleNumber == h.HoleNumber
                                               select new PossibleHole
                                               {
                                                   pHoleNumber = ah.HoleNumber,
                                                   pHoleHoleGolfer = ah.HoleGolfer,
                                                   pHoleShots = ah.Shots,
                                                   pHoleScoreName = ah.ScoreName,
                                                   pHoleBestBall = ah.BestBall,
                                                   pHoleParentHoleId = h.HoleId
                                               }).ToList<PossibleHole>();

                            });
                            u.BestBallScore_Fri = u.BestBallHoles_Fri.Sum(s => s.Shots);
                            if (u.BestBallHoles_Fri.Where(s => s.BestBallTieBreaker == true).Count() > 0)
                                u.BestBallTieBreaker_Fri = true;
                        });
                        E.CalculateHolesBestBall();

                        E.Skins_Sat = allHoles.Where(s => s.Skin == true && s.HoleDay.Value == 419260002).ToList<Hole>();
                        E.Skins_Sat.ToList().ForEach(s =>
                        {
                            s.EventPlays = (from ah in allHoles
                                            where
                                               ah.HoleDay.Value == s.HoleDay.Value
                                               && ah.HoleNumber == s.HoleNumber
                                            select new PossibleHole
                                            {
                                                pHoleNumber = ah.HoleNumber,
                                                pHoleHoleGolfer = ah.HoleGolfer,
                                                pHoleShots = ah.Shots,
                                                pHoleScoreName = ah.ScoreName,
                                                pHoleBestBall = ah.BestBall,
                                                pHoleParentHoleId = s.HoleId,
                                                pHoleGolfTeam = ah.HoleGolfTeam
                                            }).ToList<PossibleHole>();
                        });

                        E.Skins_Sun = allHoles.Where(s => s.Skin == true && s.HoleDay.Value == 419260003).ToList<Hole>();
                        E.Skins_Sun.ToList().ForEach(s =>
                        {
                            s.EventPlays = (from ah in allHoles
                                            where
                                               ah.HoleDay.Value == s.HoleDay.Value
                                               && ah.HoleNumber == s.HoleNumber
                                            select new PossibleHole
                                            {
                                                pHoleNumber = ah.HoleNumber,
                                                pHoleHoleGolfer = ah.HoleGolfer,
                                                pHoleShots = ah.Shots,
                                                pHoleScoreName = ah.ScoreName,
                                                pHoleBestBall = ah.BestBall,
                                                pHoleParentHoleId = s.HoleId,
                                                pHoleGolfTeam = ah.HoleGolfTeam
                                            }).ToList<PossibleHole>();
                        });

                        E.CalculateHolesSkins();
                    }
                    else //Event hasn't started yet populate with draft information
                    {
                        //E.EventStatusName = AllEvent.GroupBy(s => new { s.EventStatusName, s.GolfTeam, s.OnTheClock }).Select(y => new GolfTeam()
                        //{
                        //    GolfTeamId = y.Key.GolfTeamId,
                        //    GolfTeamName = y.Key.GolfTeam,
                        //    OnTheClock = y.Key.OnTheClock
                        //}).ToList<GolfTeam>();

                        E.GolfTeams = AllEvent.GroupBy(s => new { s.GolfTeamId, s.GolfTeam, s.OnTheClock }).Select(y => new GolfTeam()
                        {
                            GolfTeamId = y.Key.GolfTeamId,
                            GolfTeamName = y.Key.GolfTeam,
                            OnTheClock = y.Key.OnTheClock
                        }).ToList<GolfTeam>();

                        E.DraftPicks = AllEvent.GroupBy(s => new { s.GolfTeam, s.Golfer, s.PickNumber, s.GolferImgUrl, s.DraftPickCreatedOn, s.DraftPickCompletedOn }).Select(y => new EventDraft()
                        {
                            EventDraftGolfer = y.Key.Golfer,
                            EventDraftGolfTeam = y.Key.GolfTeam,
                            EventDraftPickNumber = y.Key.PickNumber,
                            GolferImgUrl = y.Key.GolferImgUrl,
                            EventDraftPickCompletedOn = y.Key.DraftPickCompletedOn,
                            EventDraftPickDuration = FormatDuration(y.Key.DraftPickCompletedOn, y.Key.DraftPickCreatedOn)
                        }).ToList<EventDraft>();

                        int lastPick = 0;
                        Golfer ldp = new Golfer();
                        foreach (GolfTeam team in E.GolfTeams)
                        {
                            team.Golfers = AllEvent.Where(s => s.GolfTeamId == team.GolfTeamId && s.Golfer != null).GroupBy(s => new
                            {
                                s.GolferId,
                                s.Golfer,
                                s.GolferImgUrl,
                                s.GolfTeamId,
                                s.GolfTeam,
                                s.PickNumber,
                            }).Select(y => new Golfer()
                            {
                                GolferId = y.Key.GolferId,
                                GolferName = y.Key.Golfer,
                                GolferImgUrl = y.Key.GolferImgUrl,
                                GolfTeamId = y.Key.GolfTeamId,
                                GolferGolfTeam = y.Key.GolfTeam,
                                DraftPickNumber = y.Key.PickNumber,
                            }).ToList<Golfer>();

                            NumberRank LastPickNumber = team.Golfers.OrderByDescending(n => n.DraftPickNumber).Select((n, i) => new NumberRank(n.DraftPickNumber, i + 1)).ToList().Where(f => f.Rank == 1).FirstOrDefault();
                            if (LastPickNumber != null)
                            {
                                if (LastPickNumber.Number > lastPick)
                                {
                                    ldp = team.Golfers.Where(s => s.DraftPickNumber == LastPickNumber.Number).FirstOrDefault();
                                    lastPick = LastPickNumber.Number;
                                }
                            }
                        }

                        if (ldp != null)
                            E.LastDraftPick = "Pick#" + ldp.DraftPickNumber + ": " + ldp.GolferGolfTeam + " selects " + ldp.GolferName;

                        if (E.EventStatusName != "Draft Completed")
                        {
                            GolfTeam otc = E.GolfTeams.Where(s => s.OnTheClock == true).FirstOrDefault();
                            if (otc != null)
                                E.GolfTeamOnTheClock = E.GolfTeams.Where(s => s.OnTheClock == true).FirstOrDefault().GolfTeamName;
                            else
                                E.GolfTeamOnTheClock = (from gt in datacontext.zz_golfteamSet where gt.statuscode.Value == 419260000 select gt).FirstOrDefault().zz_name;
                        }
                        else
                            E.GolfTeamOnTheClock = E.EventStatusName;


                    }
                }
                return E;
            }
            catch (System.Exception ex)
            {
                return E;

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "Has Inner Fault" : "No Inner Fault");
                    }
                }
            }
        }

        public string FormatDuration(DateTime start, DateTime end)
        {
            string rtn = "";
            TimeSpan ts = (start).Subtract(end);
            rtn = ts.ToString(@"hh\:mm");
            return rtn;
        }
        [WebMethod]
        public String AllDataRefresh()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            Shared shared = new Shared();

            try
            {
                Event E = AllData();
                EventXml xml = new EventXml();
                using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                {
                    service.EnableProxyTypes();
                    OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                    ServiceContext datacontext = new ServiceContext(service);

                    List<GolfTeam> uoGolfTeams = (from gt in datacontext.zz_golfteamSet
                                                  where gt.statecode == zz_golfteamState.Active
                                                  select new GolfTeam
                                                  {
                                                      GolfTeamName = gt.zz_name,
                                                      GolfTeamId = gt.zz_golfteamId.Value,
                                                      LeagueId = gt.zz_LeagueId.Id,
                                                  }).ToList<GolfTeam>();

                    xml.GolfTeams = uoGolfTeams.OrderBy(g => g.GolfTeamName).ToList();
                    foreach (GolfTeam team in xml.GolfTeams)
                    {
                        team.Delegates = (from gtd in datacontext.zz_golfteamdelegateSet
                                          join gt in datacontext.zz_golfteamSet on gtd.zz_PrimaryGolfTeamId.Id equals gt.zz_golfteamId
                                          where gtd.zz_DelegateGolfTeamId.Id == team.GolfTeamId
                                          where gt.statecode == zz_golfteamState.Active
                                          select new GolfTeam
                                          {
                                              GolfTeamName = gt.zz_name,
                                              GolfTeamId = gt.zz_golfteamId.Value
                                          }).ToList<GolfTeam>();
                    }
                }

                for (int x = 1; x < 5; x++)
                {
                    string Day = "";
                    String DayName = "";
                    switch (x)
                    {
                        case 1:
                            Day = "419260000";
                            DayName = "Thursday";
                            break;
                        case 2:
                            Day = "419260001";
                            DayName = "Friday";
                            break;
                        case 3:
                            DayName = "Saturday";
                            Day = "419260002";
                            break;
                        case 4:
                            DayName = "Sunday";
                            Day = "419260003";
                            break;
                    }
                    string path = "GolferScorecards_" + E.GolfEventId + "_" + Day + ".htm";
                    string url = "../puScorecards.htm?EventId=" + E.GolfEventId + "&Day=" + Day;
                    string fileName = Server.MapPath("~/scorecards/" + path);
                    if (System.IO.File.Exists(fileName))
                    {
                        Scorecard s = new Scorecard();
                        s.Day = int.Parse(Day);
                        s.DayName = DayName;
                        s.url = url;
                        xml.Scorecards.Add(s);
                    }
                }

                string Path = "xml/" + E.GolfEventId + "_" + DateTime.Now.ToFileTimeUtc() + ".xml";
                xml.CurrentXML = Path;
                xml.CreatedOn = DateTime.Now.ToLongTimeString();
                xml.EventId = E.GolfEventId;
                xml.SkinsCount_Sat = E.Skins_Sat.Count();
                xml.SkinsCount_Sun = E.Skins_Sun.Count();
                xml.TeamTop2Leader = E.GolfTeams.Where(gt => gt.TeamTop2Score == E.GolfTeams.Min(g => g.TeamTop2Score)).FirstOrDefault().GolfTeamName + " @ " + E.GolfTeams.Min(g => g.TeamTop2Score).ToString();
                xml.LastDraftPick = E.LastDraftPick;
                xml.GolfTeamOnTheClock = E.GolfTeamOnTheClock;
                xml.EventName = E.GolfEventName;
                xml.EventStatusName = E.EventStatusName;

                int BBLeaderCount = E.GolfTeams.Where(gt => gt.BestBallScore_Thurs == E.GolfTeams.Min(g => g.BestBallScore_Thurs)).Count();
                if (BBLeaderCount != E.GolfTeams.Count())
                {
                    if (BBLeaderCount > 1)
                        xml.BestBallLeader_Thurs = E.GolfTeams.Where(gt => gt.BestBallScore_Thurs == E.GolfTeams.Min(g => g.BestBallScore_Thurs) && gt.BestBallTieBreaker_Thurs == true).FirstOrDefault().GolfTeamName + " @ " + E.GolfTeams.Min(g => g.BestBallScore_Thurs).ToString() + " *TB";
                    else
                        xml.BestBallLeader_Thurs = E.GolfTeams.Where(gt => gt.BestBallScore_Thurs == E.GolfTeams.Min(g => g.BestBallScore_Thurs)).FirstOrDefault().GolfTeamName + " @ " + E.GolfTeams.Min(g => g.BestBallScore_Thurs).ToString();
                }

                BBLeaderCount = E.GolfTeams.Where(gt => gt.BestBallScore_Fri == E.GolfTeams.Min(g => g.BestBallScore_Fri)).Count();
                if (E.GolfTeams.Count() != BBLeaderCount)
                {
                    if (BBLeaderCount > 1)
                        xml.BestBallLeader_Fri = E.GolfTeams.Where(gt => gt.BestBallScore_Fri == E.GolfTeams.Min(g => g.BestBallScore_Fri) && gt.BestBallTieBreaker_Fri == true).FirstOrDefault().GolfTeamName + " @ " + E.GolfTeams.Min(g => g.BestBallScore_Fri).ToString() + " *TB";
                    else
                        xml.BestBallLeader_Fri = E.GolfTeams.Where(gt => gt.BestBallScore_Fri == E.GolfTeams.Min(g => g.BestBallScore_Fri)).FirstOrDefault().GolfTeamName + " @ " + E.GolfTeams.Min(g => g.BestBallScore_Fri).ToString();
                }

                AllGolfers ag = new AllGolfers();
                AllGolfTeams agt = new AllGolfTeams();
                AllGolfTeams agt_bbThurs = new AllGolfTeams();
                AllGolfTeams agt_bbFri = new AllGolfTeams();
                List<Golfer> unsortedGolfers = new List<Golfer>();

                ag.EventStatusName = E.EventStatusName;

                foreach (GolfTeam gt in E.GolfTeams)
                {
                    foreach (Golfer g in gt.Golfers)
                    {
                        unsortedGolfers.Add(g);
                        BuildXmlFile("xml/golfer/" + E.GolfEventId + "_Golfer_" + g.GolferId + ".xml", g);

                    }

                    //Remove BestBall from other XML files.
                    List<Hole> BBThurs = new List<Hole>();
                    List<Hole> BBFri = new List<Hole>();
                    gt.BestBallHoles_Thurs.ForEach(h => BBThurs.Add(h));
                    gt.BestBallHoles_Fri.ForEach(h => BBFri.Add(h));
                    gt.BestBallHoles_Thurs.Clear();
                    gt.BestBallHoles_Fri.Clear();

                    gt.Golfers.ForEach(glf => glf.Rounds.ForEach(rnd => rnd.Holes.Clear())); //remove Hole Information to improve load time
                    agt.GolfTeams.Add(gt);

                    gt.BestBallHoles_Thurs = BBThurs;
                    agt_bbThurs.GolfTeams.Add(gt);

                    gt.BestBallHoles_Fri = BBFri;
                    agt_bbFri.GolfTeams.Add(gt);

                    gt.ActiveGolfers = gt.Golfers.Where(g => g.GolferStatus == "Active").ToList().Count();

                }
                if (unsortedGolfers.Count() != 0)
                    xml.GolferLeaderboard = unsortedGolfers.Where(g => g.GolferScore == unsortedGolfers.Min(glf => glf.GolferScore)).FirstOrDefault().GolferName + " @ " + unsortedGolfers.Min(g => g.GolferScore).ToString();

                ag.Golfers = unsortedGolfers.OrderBy(g => g.GolferScore).ThenBy(g => g.dtNextTeeTime).ThenBy(g => g.DraftPickNumber).ToList();

                BuildXmlFile("xml/" + E.GolfEventId + "_Golfers.xml", ag);

                Skins EventSkins = new Skins();
                EventSkins.ActiveGolferCount = ag.Golfers.Where(x => x.GolferStatus == "Active").Count();
                EventSkins.Skins_Sun = E.Skins_Sun;
                EventSkins.Skins_Sat = E.Skins_Sat;
                BuildXmlFile("xml/" + E.GolfEventId + "_Skins.xml", EventSkins);

                BuildXmlFile("xml/" + E.GolfEventId + "_GolfTeams.xml", agt);
                BuildXmlFile("xml/" + E.GolfEventId + "_GolfTeams_BestBallThurs.xml", agt_bbThurs);
                BuildXmlFile("xml/" + E.GolfEventId + "_GolfTeams_BestBallFri.xml", agt_bbFri);
                BuildXmlFile("xmlConfig.xml", xml);
                BuildXmlFile(Path, E);
                return Path;
            }
            catch
            {
                return "";
                throw;
            }
        }
        private void BuildXmlFile(String fileName, Event xEvent)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(xEvent.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, xEvent);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, EventXml EventXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(EventXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, EventXml);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, Golfer GolferXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(GolferXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, GolferXml);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, AllGolfers GolfersXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(GolfersXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, GolfersXml);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, AllGolfTeams GolfTeamsXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(GolfTeamsXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, GolfTeamsXml);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, AllLeagues LeaguesXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(LeaguesXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, LeaguesXml);
                }
            }
            catch
            {
                throw;
            }
        }
        private void BuildXmlFile(String fileName, Skins SkinsXml)
        {
            try
            {
                fileName = Server.MapPath("~/" + fileName);
                string[] directories = fileName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string Path = "";
                for (int x = 0; x < directories.Count() - 1; x++)
                {
                    if (Path != "")
                        Path = Path + @"\" + directories[x];
                    else
                        Path = directories[x] + @"\";

                    bool isExists = System.IO.Directory.Exists(Path);
                    if (!isExists)
                        System.IO.Directory.CreateDirectory(Path);
                }
                //string[] lines = { XML };

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(SkinsXml.GetType());
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    serializer.Serialize(writer, SkinsXml);
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
