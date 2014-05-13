using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class Course
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; }

        public List<CourseHole> CourseHoles { get; set; }
        public Course()
        {
            CourseHoles = new List<CourseHole>();
        }
    }
}