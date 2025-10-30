using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.ViewModel
{
    public class payment_postViewModel
    {
        public int Id { get; set; }
        public string patient_name { get; set; }
        public string patient_id { get; set; }
        public string cpt_code { get; set; }
        public string practice { get; set; }
        public string? claim_id { get; set; }
        public string encounter_id { get; set; }
        public DateTime? dos { get; set; }
        public DateTime? claim_date { get; set; }
      

    }
}
