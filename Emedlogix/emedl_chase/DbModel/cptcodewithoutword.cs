using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class cptcodewithoutword
    {
        public long Id { get; set; }
        public long? RuleId { get; set; }
        public string Word { get; set; }
        public string CptCode { get; set; }
        public bool? IsCptCodeLevel { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public CptClassifyRules CptRules { get; set; }
        public bool IsDelete { get; set; }
    }
}
