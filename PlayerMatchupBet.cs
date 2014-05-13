namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class PlayerMatchupBet
    {
        public GolfTeam Favorite { get; set; }
        public GolfTeam Underdog { get; set; }
        public decimal BetAmount { get; set; }
        public bool FavoriteWon { get; set; }
        public bool UnderdogWon { get; set; }
        public decimal BetResult { get; set; }
        public bool isAuto { get; set; }
    }
}