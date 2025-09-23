using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class ruleTaskLineItem
    {
        public ruleTaskLineItem()
        {
        }
        public long Id { get; set; }
        public long RuleTaskId { get; set; }
        public string Sentense { get; set; }
        public string Code { get; set; }
        public long? RuleID { get; set; }
        public int? Doc_MasterID { get; set; }
        public string MeatSentense { get; set; }
        public string Item { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public ruleTask RuleTask { get; set; }
        public int? SentenseID { get; set; }
        public int? MeatSentenseID { get; set; }
        public string CodeType { get; set; }
        public string Notes { get; set; }
        //public string Status { get; set; }
    }
}
