namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class PlayerMatchup
    {
        public string PlayerMatchupTitle { get; set; }
        public Guid PlayerMatchupId { get; set; }
        public bool isTie { get; set; }
        public Guid EventId { get; set; }
        public bool isEventMatchup { get; set; }
        public OptionSetValue Day { get; set; }
        public string DayName { get; set; }
        public int DayNumber { get; set; }
        public decimal BetAmount { get; set; }
        public decimal BetResult { get; set; }
        public string BetStatus { get; set; }
        public string CloseTime { get; set; }
        public bool BetClosed { get; set; }

        public Guid UnderdogGolferId { get; set; }
        public string UnderdogGolfer { get; set; }
        public int UnderdogOdds { get; set; }
        public decimal UnderdogAmountPerDollar { get; set; }
        public decimal UnderdogAmount { get; set; }
        public bool UnderdogWon { get; set; }
        public Guid UnderdogGolfTeamId { get; set; }
        public string UnderdogGolfTeam { get; set; }
        public bool UnderdogAccepted { get; set; }



        public Guid FavoriteGolferId { get; set; }
        public string FavoriteGolfer { get; set; }
        public int FavoriteOdds { get; set; }
        public decimal FavoriteAmountPerDollar { get; set; }
        public decimal FavoriteAmount { get; set; }
        public bool FavoriteWon { get; set; }
        public Guid FavoriteGolfTeamId { get; set; }
        public string FavoriteGolfTeam { get; set; }
        public bool FavoriteAccepted { get; set; }

        //Team specific
        public bool Proposed { get; set; }
        public bool MatchupInProgress_Tied { get; set; }
        public bool MatchupInProgress_Winning { get; set; }
        public int  MatchupInProgress_ScoreDelta { get; set; }
        public bool WonMatchup { get; set; }
        public decimal WinAmount { get; set; }
        public decimal LossAmount { get; set; }
        public decimal ResultAmount { get; set; }
        public bool isFavorite { get; set; }
        

        public Golfer Favorite { get; set; }
        public Golfer Underdog { get; set; }

        public string TeamGolferName { get; set; }
        
        public List<PlayerMatchupBet> PlayerMatchupBets { get; set; }
        public List<PlayerMatchupProposal> BetProposals { get; set; }

        public PlayerMatchup()
        {
            Favorite = new Golfer();
            Underdog = new Golfer();
        }
        
    }
}