using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public class fl_auditlog
    {
        public int id { get; set; }
        public int document_id { get; set; }
        public int ra_taskid { get; set; }
        public string code { get; set; }
        public string reason { get; set; }
        public DateTime? createdon { get; set; }
        public string sentence { get; set; }
    }
}
