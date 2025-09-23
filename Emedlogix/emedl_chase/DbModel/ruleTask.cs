using System;
using System.Collections.Generic;
using System.Text;


namespace emedl_chase.DbModel


{
    public partial class ruleTask
    {
        public ruleTask()
        {
            RuleTaskLineItems = new HashSet<ruleTaskLineItem>();
        }
        public long Id { get; set; }
        public long TaskId { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public int? Count { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public ICollection<ruleTaskLineItem> RuleTaskLineItems { get; set; }
        public virtual task Task { get; set; }

    }
}
