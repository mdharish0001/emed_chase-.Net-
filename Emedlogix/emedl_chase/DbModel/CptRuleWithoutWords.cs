using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class CptRuleWithoutWords
    {
        public long Id { get; set; }
        public string Word { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string modfiedby { get; set; }

        public string modfied_on { get; set; }
        public string cpt_code { get; set; }

        public bool IsDelete { get; set; } = false;
        public CptClassifyRules CptClassifyRules { get; set; }
    }
}
