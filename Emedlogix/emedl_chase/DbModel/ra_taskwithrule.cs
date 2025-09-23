using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class ra_taskwithrule
    {
        public int id { get; set; }
        public int? ra_taskid { get; set; }
        public long? sourceid { get; set; }
        public string ruleno { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        public string sourcetype { get; set; }

    }
}
