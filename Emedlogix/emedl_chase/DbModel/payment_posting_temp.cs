using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.DbModel
{
    public class payment_posting_temp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string? patient_id { get; set; }
        public string? patient_name { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? dos { get; set; }
        public string? cpt { get; set; }
        public string? claim_id { get; set; }
        public string? encounter_id { get; set; }
        public string? provider { get; set; }
        public string? practice { get; set; }
        public string? payment_method { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? claim_date { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? payment_date { get; set; }
        public string? facility { get; set; }
        public string? insurance { get; set; }
        public int org_id { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? created_on { get; set; }
        public double? paid_amount { get; set; }
        public Boolean isdelete { get; set; }



        public int user_id { get; set; }
        public string? username { get; set; }
        public string? post_type { get; set; }
        public string? source { get; set; }
        public string? file_name { get; set; }
        public string? sepscare_id { get; set; }
        public string? check_number { get; set; }

        //public string npi { get; set; }

        // public ProviderMaster ProviderMaster { get; set; }

        public int? payment_post_id { get; set; }
    }
}
