using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class risk_hcc_score_list
    {

        public int id { get; set; }
        public int year { get; set; }
        public string hcc_code { get; set; }
        public string segment { get; set; }
        public string duality { get; set; }
        public bool disabled { get; set; }
        public bool aged { get; set; }
        public double score { get; set; }
        public string createdby { get; set; }
        public DateTime? createdon { get; set; }
    }
}
