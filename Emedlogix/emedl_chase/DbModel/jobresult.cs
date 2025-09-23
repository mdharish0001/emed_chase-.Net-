using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class jobresult
    {
        public int Id { get; set; }
        public int JobDetailId { get; set; }
        public int SentenceId { get; set; }
        public string RuleNo { get; set; }
        public int Score { get; set; }
        public string Code { get; set; }
        public string Section { get; set; }
        public string Description { get; set; }
        public string CodeType { get; set; }
        public bool? IsHcc { get; set; }
        public int? PageNo { get; set; }
    }
}
