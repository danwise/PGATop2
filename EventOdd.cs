using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class EventOdd
    {
        public string name { get; set; }
        public int odds { get; set; }
        public string Golfer { get; set; }
        public Guid GolferId { get; set; }
        public string Event { get; set; }
        public Guid EventId { get; set; }
        public int GolferCBSSportsId { get; set; }
        public string GolferImgUrl { get; set; }
        public string GolferRotoWorldUrl { get; set; }
        public Guid EventOddsId { get; set; }
        public Guid OnTheClock_GolfTeamId { get; set; }
    }


}