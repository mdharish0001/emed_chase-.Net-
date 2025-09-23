using System;
using System.Collections.Generic;
using System.Text;

namespace emedl_chase.DbModel
{
    public partial class cpt_termtoterm
    {
        public int id { get; set; }
        public string term { get; set; }
        public string map_to { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool isdeleted { get; set; }
    }
}

