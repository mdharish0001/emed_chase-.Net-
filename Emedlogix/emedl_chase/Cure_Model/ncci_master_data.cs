using System;

namespace emedl_chase.Cure_Model
{
    public class ncci_master_data
    {
        public long id { get; set; }
        public string? cpt_code { get; set; }
        public string? cpt_code2 { get; set; }
        public DateTime? effective_date { get; set; }
        public DateTime? term_date { get; set; }
        public string modifier { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
    }
}
