using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class task
    {
        public task()
        {
            this.RuleTask = new ruleTask();   
        }
        public long Id { get; set; }
        public string Task { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Notes { get; set; }
        public string Remarks { get; set; }
        public int? CompletedBy { get; set; }
        //public bool isCurrent { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? UserID { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public virtual ruleTask RuleTask { get; set; }
    }
}
