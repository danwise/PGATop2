using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class CourseHole
    {
        public Guid CourseHoleId { get; set; }
        public int CourseHoleNumber { get; set; }
        public int Par { get; set; }
        public int CourseHandicap { get; set; }

        public List<CourseHoleHandicap> HoleHandicaps { get; set; }
        public CourseHole()
        {

            HoleHandicaps = new List<CourseHoleHandicap>();
        }

    }
}