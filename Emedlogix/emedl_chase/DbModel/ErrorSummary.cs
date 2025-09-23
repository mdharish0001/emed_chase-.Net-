using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace emedl_chase.DbModel
{
    public partial class ErrorSummary
    {
        public int id { get; set; }
        public string cpt { get; set; }
        public string? mod1 { get; set; }
        public string? mod2 { get; set; }
        public string? mod3 { get; set; }
        public string? mod4 { get; set; }
        public int? units { get; set; }
        public long chart_master_id { get; set; }
        public string? Service { get; set; }
        public string? PolicyReference { get; set; }        
        public string? Policy_Reference_Details { get; set; }        
        public string? ApprovedStatus  { get; set; }        
        public string? Recommendation { get; set; }
        public DateTime? created_on { get; set; }
        public string? IsEdit { get; set; }
        public string? Icd1 { get; set; }
        public string? Icd2 { get; set; }
        public string? Icd3 { get; set; }
        public string? Icd4 { get; set; }
        public int? ErrorId { get; set; }
        public bool IsAlready { get; set; }
        public int? roleId { get; set; }
        public int? UserId { get; set; }

        public string? Gender { get; set; }
        public chart_master chart_Master { get; set; }
     
    }
}
