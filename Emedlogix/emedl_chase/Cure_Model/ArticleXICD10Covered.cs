using System;

namespace emedl_chase.Cure_Model
{
    public class ArticleXICD10Covered
    {

        public int Id { get; set; }  // Auto-increment primary key
        public int ArticleId { get; set; }  // Integer format
        public int ArticleVersion { get; set; }  // Integer format
        public string Icd10CodeId { get; set; }  // Remains string
        public int Icd10CodeVersion { get; set; }  // Integer format
        public int Icd10CoveredGroup { get; set; }  // Integer format
        public string Range { get; set; }  // Remains string
        public DateTime? LastUpdated { get; set; }  // Nullable DateTime
        public int? SortOrder { get; set; }  // Nullable integer
        public string Description { get; set; }  // Remains string

        // Asterisk field will be stored as boolean (true/false)
        public bool? Asterisk { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current timestamp
        public string CreatedBy { get; set; }  // User who created the record
        public string ModifiedBy { get; set; }  // User who last modified the record
        public DateTime? ModifiedOn { get; set; }  // Timestamp of last modification
    }


}