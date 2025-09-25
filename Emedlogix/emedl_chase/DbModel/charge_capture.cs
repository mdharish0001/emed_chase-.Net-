using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace emedl_chase.DbModel
{
    public class charge_capture
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string ? practice { get; set; }
        public string ? provider { get; set; }
        public string? patient_name { get; set; }
        public int patient_id { get; set; }
        public DateTime? dos { get; set; }
        public string ? cpt { get; set; }
        public int? claim_id { get; set; }
        public DateTime? claim_date { get; set; }
        public int?  encounter_id { get; set; }
        public DateTime? created_on { get; set; }
        public Boolean isdelete { get; set; }

        public string? file_name { get; set; }
        public string? state { get; set; }
        public int  org_id { get; set; }

        public string? location { get; set; }

    }
}
