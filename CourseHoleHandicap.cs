namespace Golf.Web
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

    using Microsoft.Xrm.Sdk;
    public class CourseHoleHandicap
    {
        public Guid CourseHoleHandicapId { get; set; }
        public Guid CourseHoleId { get; set; }
        public int CourseHoleHandicapHoleNumber { get; set; }
        public OptionSetValue Day { get; set; }
        public decimal Handicap { get; set; }
        public int HandicapRank { get; set; }

    }
}