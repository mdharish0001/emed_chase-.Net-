using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class finalhcc
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Hcc { get; set; }
        public string Description { get; set; }
        public long VersionCategoryID { get; set; }
        public int DocmentMasterID { get; set; }
        public string Type { get; set; }
        public int? CreatedBy { get; set; }
        public int  HccValue { get; set; }
        public DateTime? DOS { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? VersionId { get; set; }
        public categorymaster Categorymaster { get; set; }
        public documentMaster DocumentMaster { get; set; }



    }
}
