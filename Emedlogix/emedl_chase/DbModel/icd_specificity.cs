using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
   // [NotMapped]

    public class icd_specificity
    {
        public long id { get; set; }
        public string? icd_code { get; set; }
        public string? specificity { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool isdeleted { get; set; }
    }
}
