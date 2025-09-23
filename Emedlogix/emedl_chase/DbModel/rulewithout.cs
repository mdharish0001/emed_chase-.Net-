using System;
using System.Collections.Generic;
using System.Text;
namespace emedl_chase.DbModel
{
    public partial class rulewithoutwords
    {
        public long Id { get; set; }
        public long? RuleId { get; set; }
        public string Word { get; set; }
        public string Code { get; set; }
        public bool? IsCodeLevel { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public classificationrules Classificationrule { get; set; }
        public bool IsDelete { get; set; }
        

    }
}
