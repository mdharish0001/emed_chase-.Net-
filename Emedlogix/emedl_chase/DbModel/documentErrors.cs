using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
   public partial class codeChangeLog
    {
        public long Id { get; set; }
        public int Doc_MasterID { get; set; }
        public string Code { get; set; }
        public string Sentence { get; set; }
        public string Type { get; set; }
        public int? RuleID { get; set; }
        public string UserType { get; set; }
        public int VersionID { get; set; }
        public int UserID { get; set; }
        public string PreviousCodetype { get; set; }
        public string CurrentCodeType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }        
        public documentMaster DocumentMaster { get; set; }
    }
}
