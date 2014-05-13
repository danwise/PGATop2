using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class GolfTeamDelegate
    {
        public Guid GolfTeamId { get; set; }
        public string GolfTeamName { get; set; }
        public Guid DelegateGolfTeamId { get; set; }
        public string DelegateGolfTeamName { get; set; }
    }
}