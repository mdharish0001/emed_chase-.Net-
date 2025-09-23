using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class ra_cpttaskwithrule
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
