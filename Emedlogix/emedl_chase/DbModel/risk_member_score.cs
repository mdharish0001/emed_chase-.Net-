using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class risk_member_score
    {
        public int id { get; set; }
        public int member_id { get; set; }
        public string age_gender_score { get; set; }
        public string disability_score { get; set; }
        public bool hcc_score { get; set; }
        public bool hcc_count_score { get; set; }
        public double diseases_interaction_score { get; set; }
        public double risk_score { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
        //public virtual MemberMaster MemberMaster { get; set; }
    }
}
