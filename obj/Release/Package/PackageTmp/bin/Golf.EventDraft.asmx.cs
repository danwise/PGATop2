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

        [WebMethod]
        public List<EventOdd> UndraftedGolfers()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            List<EventOdd> rtn = new List<EventOdd>();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();

                zz_golfteam otc = (from gt in datacontext.zz_golfteamSet
                                        where
                                         gt.statuscode == new OptionSetValue(419260000)
                                        select gt
                                    ).FirstOrDefault();
                GolfTeam ontheclock = new GolfTeam();
                if (otc != null)
                {
                    ontheclock.GolfTeamName = otc.zz_name;
                    ontheclock.GolfTeamId = otc.zz_golfteamId.ToString();
                }

                //zz_event Event = (from e in datacontext.zz_eventSet where e.zz_eventId == Guid.Parse("9D4B5EF9-DED1-E211-BDB6-3EBB89D5579F") select e).FirstOrDefault();
                List<zz_eventodds> Odds = (
                                        from r in datacontext.zz_eventoddsSet
                                        where
                                        r.zz_EventId.Id == Event.zz_eventId
                                        && r.statuscode != new OptionSetValue(419260000)
                                        select r
                                    ).ToList<zz_eventodds>();
                foreach (zz_eventodds odd in Odds.OrderBy(s => s.zz_Odds).ThenBy(n => n.zz_name))
                {
                    zz_golfer golfer = (from g in datacontext.zz_golferSet
                                        where g.zz_golferId == odd.zz_GolferId.Id
                                        select g).FirstOrDefault();

                    EventOdd o = new EventOdd()
                    {
                        name = odd.zz_name,
                        Golfer = golfer.zz_name,
                        Event = Event.zz_name,
                        odds = int.Parse(odd.zz_Odds.ToString()),
                        EventOddsId = odd.zz_eventoddsId.ToString(),
                        OnTheClock = ontheclock,
                    };

                    if (golfer.zz_CBSSportsGolferId != null)
                    {
                        o.GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + golfer.zz_CBSSportsGolferId.ToString() + @".jpg";
                        o.GolferCBSSportsId = int.Parse(golfer.zz_CBSSportsGolferId.ToString());
                    }
                    rtn.Add(o);
                }
            }
            return rtn;
        }

        [WebMethod]
        public List<EventDraft> DraftResults()
        {
            ServerConnection serverConnect = new ServerConnection();
            ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
            List<EventDraft> rtn = new List<EventDraft>();
            using (service = ServerConnection.GetOrganizationProxy(serverConfig))
            {
                service.EnableProxyTypes();
                OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                ServiceContext datacontext = new ServiceContext(service);

                zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                
                List<zz_eventdraft> draftpicks = (
                                        from r in datacontext.zz_eventdraftSet
                                        where
                                        r.zz_EventId.Id == Event.zz_eventId
                                        && r.statuscode != new OptionSetValue(419260000)
                                        select r
                                    ).ToList<zz_eventdraft>();
                foreach (zz_eventdraft draftpick in draftpicks.OrderByDescending(s => s.zz_PickNumber))
                {
                    zz_golfer golfer = (from g in datacontext.zz_golferSet
                                        where g.zz_golferId == draftpick.zz_GolferId.Id
                                        select g).FirstOrDefault();
                    zz_golfteam golfTeam = (from g in datacontext.zz_golfteamSet
                                        where g.zz_golfteamId == draftpick.zz_GolfTeamId.Id
                                        select g).FirstOrDefault();

                    EventDraft d = new EventDraft()
                    {
                        Golfer = golfer.zz_name,
                        Event = Event.zz_name,
                        PickNumber = (int)draftpick.zz_PickNumber,
                        GolfTeam = golfTeam.zz_name,
                        
                    };

                    if (golfer.zz_CBSSportsGolferId != null)
                    {
                        d.GolferImgUrl = @"http://sports.cbsimg.net/images/golf/players/60x80/" + golfer.zz_CBSSportsGolferId.ToString() + @".jpg";
                    }
                    rtn.Add(d);
                }
            }
            return rtn;
        }
        [WebMethod]
        public GolfTeam GolfTeam_OnTheClock()
        {
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
                        GolfTeamId = OnTheClock.zz_golfteamId.ToString()
                    };

                    rtn = gt;
                }
              
            }
            return rtn;
        }

        [WebMethod]
        public List<Event> HelloWorld()
        {
            ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration serverConfig = serverConnect.GetServerConfiguration("cseadw.calcsea.org", "cseadw", "CSEAHQ", "dwise", true, AuthenticationProviderType.Federation);
                List<Event> rtn = new List<Event>();
                using (service = ServerConnection.GetOrganizationProxy(serverConfig))
                {
                    service.EnableProxyTypes();
                    OrganizationServiceContext _orgContext = new OrganizationServiceContext(service);
                    ServiceContext datacontext = new ServiceContext(service);

                    Guid EventId = Guid.Parse("9D4B5EF9-DED1-E211-BDB6-3EBB89D5579F");
                    List<zz_event> AllEvents = (from e in datacontext.zz_eventSet select e).ToList<zz_event>();
                    foreach (zz_event E in AllEvents)
                    {
                        Event evnt = new Event();
                        evnt.Name = E.zz_name;
                        evnt.StartDate = (DateTime)E.zz_StartDate;
                        rtn.Add(evnt);
                    }
                }
                return rtn;
        }


        [WebMethod]
        public Boolean DraftGolfer(String EventOddsId)
        {
            Boolean rtn = false;
            Guid gEventOddsId = new Guid();
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
                        zz_event Event = (from e in datacontext.zz_eventSet where e.statecode == zz_eventState.Active select e).FirstOrDefault();
                        //get selected event odds record
                        zz_eventodds eventOdd = (from o in datacontext.zz_eventoddsSet where o.zz_eventoddsId == gEventOddsId select o).FirstOrDefault();
                        //Get GolfTeam that is on the clock
                        zz_golfteam golfTeam = (from gt in datacontext.zz_golfteamSet where gt.statuscode == new OptionSetValue(419260000) select gt).FirstOrDefault();
                        //Get Draft Pick that is on the clock
                        zz_eventdraft draftPick = (from ed in datacontext.zz_eventdraftSet where ed.statuscode == new OptionSetValue(419260000) select ed).FirstOrDefault();

                        zz_golfer golfer = (from g in datacontext.zz_golferSet where g.zz_golferId == eventOdd.zz_GolferId.Id select g).FirstOrDefault();

                        //Update Event Draft with the selected golfer and new name
                        Entity uEventDraft = service.Retrieve("zz_eventdraft", (Guid)draftPick.zz_eventdraftId, new ColumnSet(true));
                        uEventDraft.Attributes["zz_golferid"] = new EntityReference(zz_golfer.EntityLogicalName, eventOdd.zz_GolferId.Id);
                        uEventDraft.Attributes["zz_name"] = Event.zz_name + ": Pick# " + draftPick.zz_PickNumber + " " + golfTeam.zz_name + " selects " + golfer.zz_name;
                        service.Update(uEventDraft);

                        //Update Golfer
                        Entity uGolfer = service.Retrieve("zz_golfer", (Guid)golfer.zz_golferId, new ColumnSet(true));
                        uGolfer.Attributes["zz_golfteamid"] = new EntityReference(zz_golfteam.EntityLogicalName, (Guid)golfTeam.zz_golfteamId);
                        service.Update(uGolfer);

                        //Event Odds
                        SetStateRequest request = new SetStateRequest();
                        request.EntityMoniker = new EntityReference(zz_eventodds.EntityLogicalName, (Guid)eventOdd.zz_eventoddsId);
                        request.State = new OptionSetValue((int)zz_eventoddsState.Active);
                        request.Status = new OptionSetValue(419260000); //Inactive
                        SetStateResponse response = (SetStateResponse)service.Execute(request);

                        //Draft Pick
                        SetStateRequest requestdp = new SetStateRequest();
                        requestdp.EntityMoniker = new EntityReference(zz_eventdraft.EntityLogicalName, (Guid)draftPick.zz_eventdraftId);
                        requestdp.State = new OptionSetValue((int)zz_eventdraftState.Active);
                        requestdp.Status = new OptionSetValue(419260001); 
                        SetStateResponse responsedp = (SetStateResponse)service.Execute(requestdp);

                        //Golf Team - Take off clock
                        SetStateRequest requestgt = new SetStateRequest();
                        requestgt.EntityMoniker = new EntityReference(zz_golfteam.EntityLogicalName, (Guid)golfTeam.zz_golfteamId);
                        requestgt.State = new OptionSetValue((int)zz_golfteamState.Active);
                        requestgt.Status = new OptionSetValue(1); //Active
                        SetStateResponse responsegt = (SetStateResponse)service.Execute(requestgt);


                        //Golfer - Set to Drafted
                        SetStateRequest requestg = new SetStateRequest();
                        requestg.EntityMoniker = new EntityReference(zz_golfer.EntityLogicalName, (Guid)golfer.zz_golferId);
                        requestg.State = new OptionSetValue((int)zz_golferState.Active);
                        requestg.Status = new OptionSetValue(1); //Active/Drafted
                        SetStateResponse responseg = (SetStateResponse)service.Execute(requestg);

                        List<zz_golfteam> GolfTeams = (from gt in datacontext.zz_golfteamSet where gt.statecode == zz_golfteamState.Active select gt).ToList<zz_golfteam>();
                        Int32 numberOfTeams = GolfTeams.Count();
                        Int32 NextDraftPosition = GetNextPickDraftPosition((int)draftPick.zz_PickNumber, numberOfTeams);
                        zz_golfteam OnTheClock = GolfTeams.Where(s => s.zz_DraftPosition == NextDraftPosition).FirstOrDefault();

                       Guid pk = CreateDraftPick(service, Event, OnTheClock, (int)draftPick.zz_PickNumber + 1);

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

        private Guid CreateDraftPick(OrganizationServiceProxy service, zz_event Event, zz_golfteam golfTeam, int PickNumber)
        {
            Guid pk = new Guid();
            try
            {
                zz_eventdraft ed = new zz_eventdraft()
                {
                    zz_EventId = Event.ToEntityReference(),
                    zz_GolfTeamId =  golfTeam.ToEntityReference(),
                    zz_PickNumber = PickNumber,
                    zz_name = Event.zz_name + ": Pick# " + PickNumber.ToString(),
                    zz_Skins = true,
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

       
    }
}
