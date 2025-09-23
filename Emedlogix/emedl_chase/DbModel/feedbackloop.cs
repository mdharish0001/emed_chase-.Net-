using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class feedbackloop
    {
        public int id { get; set; }
        public string sentencetxt { get; set; }
        public string code { get; set; }
        public string codetype { get; set; }
        public string actionby { get; set; }
        public string rule { get; set; }
        public int? createdby { get; set; }
        public DateTime? createddate { get; set; }
        public int? modifiedby { get; set; }
        public DateTime? modifieddate { get; set; }

        public int sentenceid { get; set; }
        public int jobid { get; set; }
        public string instancetype { get; set; }
        public bool issend { get; set; }
        public bool isracompleted { get; set; }
        public string docname { get; set; }
        public int version { get; set; }
    }
}
