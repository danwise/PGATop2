using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Golf.Web
{
    public class LeagueProduct
    {
        public Guid LeagueId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNumber { get; set; }
        public decimal Amount { get; set; }
        public string Leader { get; set; }
        public int Count { get; set; }
        public string url { get; set; }
        public int ProductDisplayOrder { get; set; }
        public string Day { get; set; }
    }
}