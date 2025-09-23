using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class LeftCptWithOutWord
    {
        public long Id { get; set; }

        public string Word { get; set; }
        public string RuleId { get; set; }
        public string cpt_code { get; set; }

        public long CptRuleId { get; set; }

        public CptClassifyRules CptClassifyRules { get;set; }
    }
}
