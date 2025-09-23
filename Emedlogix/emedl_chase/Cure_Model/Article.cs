
using System;

namespace emedl_chase.Cure_Model
{
    public class Article
    {
        public long ArticleId { get; set; }
        public int? ArticleVersion { get; set; }
        public string ArticleType { get; set; }
        public string Title { get; set; }
        public DateTime? ArticlePubDate { get; set; }
        public DateTime? ArticleEffDate { get; set; }
        public DateTime? ArticleEndDate { get; set; }
        public string Description { get; set; }
        public string OtherComments { get; set; }
        public string SadUrl { get; set; }
        public string Status { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string HistoryExp { get; set; }
        public string? KeyArticle { get; set; }
        public string Icd9CoveredPara { get; set; }
        public string Icd9NoncoveredPara { get; set; }
        public string RevenuePara { get; set; }
        public string ThirtyPercent { get; set; }
        public DateTime? ArticleRevEndDate { get; set; }
        public string SourceArticleId { get; set; }
        public DateTime? DateRetired { get; set; }
        public string Keywords { get; set; }
        public string Icd10Doc { get; set; }
        public string AddIcd10Info { get; set; }
        public string CmsCovPolicy { get; set; }
        public string DisplayId { get; set; }
        public string ReferenceArticle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to UTC
        public string CreatedBy { get; set; }  // User who created the record
        public string ModifiedBy { get; set; }  // User who last modified the record
        public DateTime? ModifiedOn { get; set; }  // Timestamp of last modification
    }
}



