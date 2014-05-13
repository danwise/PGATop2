using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class EventDraft
    {
        public string EventDraftGolfer { get; set; }
        public string EventDraftEvent { get; set; }
        public int GolferCBSSportsId { get; set; }
        public string GolferImgUrl { get; set; }
        public int EventDraftPickNumber { get; set; }
        public string EventDraftGolfTeam { get; set; }
        public DateTime EventDraftPickCompletedOn { get; set; }
        public string EventDraftPickDuration { get; set; }
        public string EventDraftPickName { get; set; }
        public int EventDraftStatus { get; set; }
        public int Odds { get; set; }
    }
}