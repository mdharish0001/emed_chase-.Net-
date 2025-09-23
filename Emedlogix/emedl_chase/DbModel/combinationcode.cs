using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public partial class combinationcode
    {
        public long Id { get; set; }
        public string Ex_Code1 { get; set; }
        public string Ex_Code2 { get; set; }
        public string Ex_Code3 { get; set; }
        public string Com_Code1 { get; set; }
        public string Com_Code2 { get; set; }
        public string Com_Code3 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? effectivedate { get; set; }
        public DateTime? termdate { get; set; }

        public bool IsDelete { get; set; }
    }
}
