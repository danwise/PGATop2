using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class Event
    {
        public string GolfEventName { get; set; }
        public Guid GolfEventId { get; set; }
        public DateTime StartDate { get; set; }
        public string LastRefreshTime { get; set; }
        public string GolfTeamOnTheClock { get; set; }
        public string LastDraftPick { get; set; }
        public DateTime LastDraftPickTime { get; set; }
        public List<GolfTeam> GolfTeams { get; set; }
        public Course EventCourse { get; set; }
        public List<Hole> Skins_Sat { get; set; }
        public List<Hole> Skins_Sun { get; set; }
        public List<EventDraft> DraftPicks { get; set; }
        public string EventStatusName { get; set; }
        public bool isDay1 { get; set; }



        public void CalculateHoles()
        {
            Shared shared = new Shared();
            foreach(GolfTeam gt in GolfTeams)
            {
                foreach (Golfer golfer in gt.Golfers)
                {
                    foreach (Round round in golfer.Rounds)
                    {
                        round.Holes.ToList().ForEach(u =>
                            {
                                u.CourseHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().CourseHandicap;
                                u.Par = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().Par;
                                u.CalculatedHandicapRank = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y=>y.Day.Value == round.Day.Value).FirstOrDefault().HandicapRank;
                                u.ScoreName = shared.ScoreName(u.Par, u.Shots);
                                u.CalculatedHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == round.Day.Value).FirstOrDefault().Handicap;
                            });
                    }
                }
            }
        }

        public void CalculateHolesBestBall()
        {
            Shared shared = new Shared();
            foreach (GolfTeam gt in GolfTeams)
            {
                gt.BestBallHoles_Thurs.ToList().ForEach(u =>
                {
                    u.CourseHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().CourseHandicap;
                    u.Par = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().Par;
                    u.CalculatedHandicapRank = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(1).Value).FirstOrDefault().HandicapRank;
                    u.ScoreName = shared.ScoreName(u.Par, u.Shots);
                    u.CalculatedHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(1).Value).FirstOrDefault().Handicap;
                });

                gt.BestBallHoles_Fri.ToList().ForEach(u =>
                {
                    u.CourseHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().CourseHandicap;
                    u.Par = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().Par;
                    u.CalculatedHandicapRank = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(2).Value).FirstOrDefault().HandicapRank;
                    u.ScoreName = shared.ScoreName(u.Par, u.Shots);
                    u.CalculatedHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(2).Value).FirstOrDefault().Handicap;
                });
            }
        }

        public void CalculateHolesSkins()
        {
            Shared shared = new Shared();
            Skins_Sat.ToList().ForEach(u =>
                {
                    u.CourseHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().CourseHandicap;
                    u.Par = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().Par;
                    u.CalculatedHandicapRank = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(1).Value).FirstOrDefault().HandicapRank;
                    u.ScoreName = shared.ScoreName(u.Par, u.Shots);
                    u.CalculatedHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(1).Value).FirstOrDefault().Handicap;
                });

            Skins_Sun.ToList().ForEach(u =>
                {
                    u.CourseHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().CourseHandicap;
                    u.Par = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().Par;
                    u.CalculatedHandicapRank = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(2).Value).FirstOrDefault().HandicapRank;
                    u.ScoreName = shared.ScoreName(u.Par, u.Shots);
                    u.CalculatedHandicap = EventCourse.CourseHoles.Where(s => s.CourseHoleNumber == u.HoleNumber).FirstOrDefault().HoleHandicaps.Where(y => y.Day.Value == shared.osvGetDay(2).Value).FirstOrDefault().Handicap;
                });
        }

        public Event()
        {
            GolfTeams = new List<GolfTeam>();
            Skins_Sat = new List<Hole>();
            Skins_Sun = new List<Hole>();
            LastRefreshTime = DateTime.Now.ToShortTimeString();
            isDay1 = true;
        }
    }
}