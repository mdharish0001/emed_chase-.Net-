using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class risk_county_rate_master
    {

        public int id { get; set; }
        public int year { get; set; }
        public string code { get; set; }
        public string countyname { get; set; }
        public string state { get; set; }
        public double? parts_aandb_5_bonus { get; set; }
        public double? parts_aandb_3_5_bonus { get; set; }
        public double? parts_aandb_0_bonus { get; set; }
        public double? parts_aandb_esrd { get; set; }
        public int? createdby { get; set; }
        public DateTime? createdon { get; set; }
    }
}
