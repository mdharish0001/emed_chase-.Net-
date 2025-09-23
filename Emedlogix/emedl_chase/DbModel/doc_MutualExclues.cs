using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class doc_MutualExclues
    {
        public long Id { get; set; }
        public int DocId { get; set; }
        public int VersionId { get; set; }
        public string Code { get; set; }
        public string MutualExcludeCode { get; set; }
        public DateTime? Dos { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public documentMaster DocumentMaster { get; set; }
    }
}
