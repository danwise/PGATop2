using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;

namespace Golf.Web
{
    public class EventAndLeague
    {
        public Guid EventId { get; set; }
        public Guid LeagueId { get; set; }
        public string EventName { get; set; }
       public  OptionSetValue EventStatusCode { get; set; }
       public OptionSetValue LeagueStatusCode { get; set; }
       public OptionSetValue LeagueType { get; set; }
       public string PlaySuspendedReason { get; set; }
       public DateTime StartTime { get; set; }
       public int CutLine { get; set; }
    }
}