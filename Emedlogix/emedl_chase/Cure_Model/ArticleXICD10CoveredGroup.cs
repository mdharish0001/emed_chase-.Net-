using System;

namespace emedl_chase.Cure_Model
{
    public class ArticleXICD10CoveredGroup
    {
        public int Id { get; set; }  // Auto-increment primary key
        public string ArticleId { get; set; }  // Article ID
        public string ArticleVersion { get; set; }  // Article version
        public string Icd10CoveredGroup { get; set; }  // ICD-10 covered group
        public string Paragraph { get; set; }  // Paragraph content
        public DateTime? LastUpdated { get; set; }  // Last updated date (nullable)
        public bool? Icd10CoveredAst { get; set; }  // Covered asterisk flag (nullable)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Auto-set creation timestamp

        public string CreatedBy { get; set; }  // User who created the record
        public string ModifiedBy { get; set; }  // User who last modified the record
        public DateTime? ModifiedOn { get; set; }  // Timestamp of last modification


    }
}
