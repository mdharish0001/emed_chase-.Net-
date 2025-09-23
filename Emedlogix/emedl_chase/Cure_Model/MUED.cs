using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace emedl_chase.Cure_Model
{
    public class MUED
    {
        [Key]
        public long Id { get; set; }
        public string cpt_code { get; set; }
        public int practitioner_service_mue { get; set; }
        public string mue_indicator { get; set; }
        public string mue_rationale { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } // User ID or Name

        public DateTime? ModifiedOn { get; set; }  // Nullable
        public string ModifiedBy { get; set; } // User ID or Name
    }
}
