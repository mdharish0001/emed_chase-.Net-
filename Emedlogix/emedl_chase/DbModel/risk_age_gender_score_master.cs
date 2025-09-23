using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class risk_age_gender_score_master
    {

        public int id { get; set; }
        public int year { get; set; }
        public int min_age { get; set; }
        public int max_age { get; set; } = 1000;
        public string gender { get; set; }
        public string segment { get; set; }
        public string duality { get; set; }
        public bool disabled { get; set; }
        public bool originally_disabled { get; set; }
        public double score { get; set; }
        public string createdby { get; set; }
        public DateTime? createdon { get; set; }
    }
}
