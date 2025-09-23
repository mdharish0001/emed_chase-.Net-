using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace emedl_chase.Cure_Model
{
    [Table("lcdnotcoveredsource")] // ✅ Ensure correct table mapping
    public class LcdNotCoveredSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }  // Primary Key (bigint, auto-increment)

        public long? LcdId { get; set; }  // bigint

        public string? HcpcCodeId { get; set; }  // text

        public int? HcpcCodeGroup { get; set; }  // integer

        [MaxLength(100)]
        public string? ArticleId { get; set; }  // character varying(100)

        public string? Icd10CodeId { get; set; }  // text

        public int? Icd10NoncoveredGroup { get; set; }  // integer

        public string? Paragraph { get; set; }  // text

        [MaxLength(50)]
        public string? Status { get; set; }  // character varying(50)

        public DateTime? StatusUpdateOn { get; set; }  // timestamp without time zone

        public string? HcpcParagraph { get; set; }  // text

        public DateTime? CompletedOn { get; set; }  // timestamp with time zone

        public int? CompletedBy { get; set; }  // integer

        public DateTime? AssignedOn { get; set; }  // timestamp with time zone

        public int? UserId { get; set; }  // integer

        public long? AssignId { get; set; }  // bigint
        public DateTime? effective_date { get; set; }
        public DateTime? term_date { get; set; }
        public DateTime? created_on { get; set; }
    }
}
