using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class DraftPick
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public Guid GolferId { get; set; }
        public string GolferName { get; set; }
        public Guid GolfTeamId { get; set; }
        public string GolfTeamName { get; set; }
        public Guid LeagueId { get; set; }
        public int PickNumber { get; set; }
        public Guid EventDraftId { get; set; }
        public int RoundsInDraft { get; set; }
        public Guid EventOddsId { get; set; }
        public int EventDraftStatus { get; set; }
    }
}