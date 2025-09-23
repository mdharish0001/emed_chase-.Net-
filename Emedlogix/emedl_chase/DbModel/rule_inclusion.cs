using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class rule_inclusion
    {
        public long Id { get; set; }
        public long RuleId { get; set; }
        public long[] IncludeId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
