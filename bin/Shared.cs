namespace Golf.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Xrm.Sdk;

    public class Shared
    {

        public Shared(){}

        public OptionSetValue osvGetDay(int d)
        {
            OptionSetValue iReturn = new OptionSetValue(419260000);
            switch (d)
            {
                case 1:
                    iReturn = new OptionSetValue(419260000);
                    break;
                case 2:
                    iReturn = new OptionSetValue(419260001);
                    break;
                case 3:
                    iReturn = new OptionSetValue(419260002);
                    break;
                case 4:
                    iReturn = new OptionSetValue(419260003);
                    break;
            }
            return iReturn;
        }

        public int iGetDay(OptionSetValue d)
        {
            int iReturn = 0;
            if (d != null)
            {
                switch (d.Value)
                {
                    case 419260000:
                        iReturn = 1;
                        break;
                    case 419260001:
                        iReturn = 2;
                        break;
                    case 419260002:
                        iReturn = 3;
                        break;
                    case 419260003:
                        iReturn = 4;
                        break;
                }
            }
            return iReturn;
        }

        public string sGetDay(OptionSetValue d)
        {
            string sReturn = "";
            if (d != null)
            {
                switch (d.Value)
                {
                    case 419260000:
                        sReturn = "Thursday";
                        break;
                    case 419260001:
                        sReturn = "Friday";
                        break;
                    case 419260002:
                        sReturn = "Saturday";
                        break;
                    case 419260003:
                        sReturn = "Sunday";
                        break;
                }
            }
            return sReturn;
        }

        public string sGetEventStatus(OptionSetValue status)
        {
            string sReturn = "";
            switch (status.Value)
            {
                case 419260000:
                    sReturn = "Draft Completed";
                    break;
                case 419260001:
                    sReturn = "In Process";
                    break;
                case 419260002:
                    sReturn = "Play Suspended";
                    break;
                case 1:
                    sReturn = "Active";
                    break;
                case 2:
                    sReturn = "Completed";
                    break;
            }
            return sReturn; ;
        }

        public string sGetLeagueType(OptionSetValue status)
        {
            string sReturn = "";
            switch (status.Value)
            {
                case 419260000:
                    sReturn = "Standard";
                    break;
                case 419260001:
                    sReturn = "Friday Redraft";
                    break;
                case 419260002:
                    sReturn = "Sunday Only";
                    break;
                case 419260003:
                    sReturn = "Undrafted Only League";
                    break;
                case 1:
                    sReturn = "Active";
                    break;
                case 2:
                    sReturn = "Completed";
                    break;
            }
            return sReturn; ;
        }

        public string sGetRoundEventStatus(OptionSetValue status)
        {
            string sReturn = "";
            switch (status.Value)
            {
                case 419260000:
                    sReturn = "Played";
                    break;
                case 419260001:
                    sReturn = "Missed Cut";
                    break;
                case 419260002:
                    sReturn = "Withdraw";
                    break;
            }
            return sReturn; 
        }
        public String ScoreName(int par, int shots)
        {
            String rtn = "";
            switch (shots - par)
            {
                case -3:
                    rtn = "Double Eagle";
                    break;
                case -2:
                    rtn = "Eagle";
                    break;
                case -1:
                    rtn = "Birdie";
                    break;
                case 0:
                    rtn = "Par";
                    break;
                case 1:
                    rtn = "Bogey";
                    break;
                case 2:
                    rtn = "Double Bogey";
                    break;
            }
            if (shots == 1)
                rtn = "Hole in One!!!!!";

            return rtn;
        }

        public OptionSetValue osvEventStatus(String Status)
        {
            OptionSetValue iReturn = new OptionSetValue(419260000);
            switch (Status)
            {
                case "Played":
                    iReturn = new OptionSetValue(419260000);
                    break;
                case "Missed Cut":
                    iReturn = new OptionSetValue(419260001);
                    break;
                case "Withdraw":
                    iReturn = new OptionSetValue(419260002);
                    break;
            }
            return iReturn;
        }

        public string sBetStatus(OptionSetValue Status)
        {
            string sReturn = "";
            switch (Status.Value)
            {
                case 1:
                    sReturn = "Active";
                    break;
                case 2:
                    sReturn = "Completed";
                    break;
                case 419260000:
                    sReturn = "Accepted";
                    break;
                case 419260001:
                    sReturn = "Declined";
                    break;
                case 419260002:
                    sReturn = "Proposed";
                    break;
                case 419260003:
                    sReturn = "Closed";
                    break;
            }
            return sReturn; 
        }

        public string sGolferStatus(OptionSetValue Status)
        {
            string sReturn = "";
            switch (Status.Value)
            {
                case 1:
                    sReturn = "Active";
                    break;
                case 2:
                    sReturn = "Missed Cut";
                    break;
                case 419260000:
                    sReturn = "Undrafted";
                    break;
                case 419260001:
                    sReturn = "Withdraw";
                    break;
            }
            return sReturn; 
        }
    }
}