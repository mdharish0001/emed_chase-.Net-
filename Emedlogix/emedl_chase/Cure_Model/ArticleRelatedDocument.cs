using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace emedl_chase.Cure_Model
{
    [Table("article_related_documents", Schema = "public")]  // Ensure it maps to correct table and schema
    public class ArticleRelatedDocument
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }  // This is the PK (integer)

        [Column("article_id")]
        public string ArticleId { get; set; }  // This is the article_id (text)

        [Column("article_version")]
        public string ArticleVersion { get; set; }

        [Column("related_num")]
        public string RelatedNum { get; set; }

        [Column("r_article_id")]
        public string RArticleId { get; set; }

        [Column("r_article_version")]
        public string RArticleVersion { get; set; }

        [Column("r_lcd_id")]
        public string RLcdId { get; set; }

        [Column("r_lcd_version")]
        public string RLcdVersion { get; set; }

        [Column("r_contractor_id")]
        public string RContractorId { get; set; }

        [Column("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public string CreatedBy { get; set; }

        [Column("modified_by")]
        public string ModifiedBy { get; set; }

        [Column("modified_on")]
        public DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;

    }
}
