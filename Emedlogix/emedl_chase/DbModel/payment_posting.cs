using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public class payment_posting
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
        public string? facility{ get; set; }
        public string? insurance { get; set; }
        public int org_id { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? created_on { get; set; }
        public double? paid_amount { get; set; }
        public Boolean isdelete { get; set; }
    
        

        public int user_id { get; set; }
        public string? username { get; set; }
        public string? post_type { get; set; }

    }
}
