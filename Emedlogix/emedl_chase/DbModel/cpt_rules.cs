using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
namespace emedl_chase.DbModel
{
   // [NotMapped]

    public class cpt_rules
    {
        public long id { get; set; }
        public string? code { get; set; }
        public string? backend_desc { get; set; }
        public string? frontend_desc { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string? backend_condtion { get; set; }
        public bool isdeleted { get; set; }
        public int? score { get; set; }
    }
}
