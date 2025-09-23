using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
    public class icd
    {
        public int Id { get; set; }
        public string Icdcode { get; set; }
        public string Hcc { get; set; }
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TermDate { get; set; }
        [NotMapped]
        public string? Description_ignorecase { get; set; }
        //public string local_Desc { get; set; }

    }
}
