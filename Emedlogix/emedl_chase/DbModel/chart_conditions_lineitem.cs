using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.DbModel
{
   // [NotMapped]

    public class chart_conditions_lineitem
    {
        public long Id { get; set; }
        public long Chart_Master_Id { get; set; }
        public string? Condition { get; set; }
        public string? Icd_Code { get; set; }
        public string? Source_Type { get; set; }
        public int? Created_By { get; set; }
        public DateTime? Created_On { get; set; }
        public string? Message { get; set; }
        public string? Sentence { get; set; }
        public long? Rule_Id { get; set; }
        public string? Section { get; set; }
        public chart_master chart_Master { get; set; }
        public string desc { get; set; }
        public bool ischosen { get; set; } 
        public bool isreject { get; set; }
        public string? Reasoning { get; set; }
        public string CptCode { get; set; }
        public int? cptDetailsId { get; set; }

        public CptDetails CptDetails { get; set; }


        public bool IsDeleted { get; set; }
        public int? Order { get; set; }

    }
}
