namespace emedl_chase.DbModel
{
    public partial class CptDetails
    {
        public int id { get; set; }
        public string cpt { get; set; }
        public string? mod1 { get; set; }
        public string? mod2 { get; set; }
        public string? mod3 { get; set; }
        public string? mod4 { get; set; }
        public int? units { get; set; }        
        public int? ErrorId { get; set; }        
        public bool IsClarification { get; set; }        
        public long chart_master_id { get; set; }
        public string? Service { get; set; }        
        public chart_master chart_Master { get; set; }

        public bool IsDeleted { get; set; }
        public string? icd1 { get; set; }
        public string? icd2 { get; set; }
        public string? icd3 { get; set; }
        public string? icd4 { get; set; }

        public int? ChargeAssignUserId { get; set; }
        public string? ChargeStatus { get; set; }
        public DateTime? charge_submitted_on { get; set; }
        public DateTime? charge_completed_on { get; set; }

        public string? ChargeComments { get; set; }

        public string? encounter { get; set; }

        public string? claim_id { get; set; }


        public ICollection<chart_conditions_lineitem> chart_Conditions_Lineitems { get; set; }

       


    }
}
