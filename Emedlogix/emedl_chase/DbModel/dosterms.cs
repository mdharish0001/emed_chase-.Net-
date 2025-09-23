using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class dosterms
    {
        public int id { get; set; }
        public string value { get; set; }
        public int? createdby { get; set; }
        public DateTime? createddate { get; set; }
        public int? modifiedby { get; set; }
        public DateTime? modifieddate { get; set; }
        public bool isdeleted { get; set; }        
    }
}
