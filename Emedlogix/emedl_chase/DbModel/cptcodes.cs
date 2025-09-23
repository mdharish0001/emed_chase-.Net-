using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel{
    //[NotMapped]

    public class cptcodes
    {
        public long id { get; set; }
        public string cpt_code { get; set; }
        public string backend_desc { get; set; }
        public string front_desc { get; set; }
        public DateTime? term_date { get; set; }
        public DateTime? eff_date { get; set; }
        public int created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string medium_desc { get; set; }
        public string short_desc { get; set; }
        [NotMapped]
        public string medium_desc_lower { get; set; }
        [NotMapped]
        public string short_desc_lower { get; set; }
        [NotMapped]
        public string front_desc_lower { get; set; }



    }
}
