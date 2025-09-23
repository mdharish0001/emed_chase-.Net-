using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace emedl_chase.DbModel
{
    //[NotMapped]
    public class icd_condtions_mapping
    {
        public long Id { get; set; }
        public string? code { get; set; }
        public string? backend_desc { get; set; }
        public string? frontend_descs { get; set; }
        public string created_by { get; set; }
        public DateTime? created_date { get; set; }
        public string? type { get; set; }
        public int? character_cnt  { get; set; }
        public int? source_id { get; set; }
        public string? source_from { get; set; }
        public string? ssh { get; set; }
        public string? modified_by { get; set; }
        public DateTime? modified_date { get; set; }
        public bool isdeleted { get; set; }
        public string? backend_condtion { get; set; }
        public int? score { get; set; }
        public long? groupid { get; set; }
        public DateTime? effective_date { get; set; }
        public DateTime? term_date { get; set; }
    }
}
