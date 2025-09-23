using System;
using System.ComponentModel.DataAnnotations;

namespace emedl_chase.Cure_Model
{
    public class LcdSourceData
    {
        [Key]
        public long id { get; set; }  // bigint -> long
        public long? LcdId { get; set; }  // bigint -> long
        public string HcpcCodeId { get; set; }  // text -> string
        public string HcpcCodeGroup { get; set; }  // text -> string
        public long ArticleId { get; set; }  // text -> string
        public string Icd10CodeId { get; set; }  // text -> string
        public int Icd10CoveredGroup { get; set; }  // integer -> int
        public string Paragraph { get; set; }  // text -> string
        public string hcpc_paragraph { get; set; }  // text -> string
        public string status { get; set; }  // text -> string
        public DateTime? statusupdatedon { get; set; }  // text -> string
        public DateTime? assigned_on { get; set; }
        public int? user_id { get; set; }
        public int? completed_by { get; set; }
        public DateTime? completed_on { get; set; }       
        public long? assing_id { get; set; }
        public DateTime? effective_date { get; set; }
        public DateTime? term_date { get; set; }
        public DateTime? created_on { get; set; }
    }
}
