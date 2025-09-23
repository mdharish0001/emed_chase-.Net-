using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class hccinclusion
    {
        public long Id { get; set; }
        public long CategoryID { get; set; }
        public string HccCode { get; set; }
        public string InclusionCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public categorymaster Categorymaster { get; set; }
    }

}
