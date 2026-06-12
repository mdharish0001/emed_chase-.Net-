using System.Text.Json.Serialization;

namespace emedl_chase.Model
{
    public class DocumentReferenceBundle
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("entry")]
        public List<DocumentReferenceEntry> entry { get; set; }
    }

    public class DocumentReferenceEntry
    {
        [JsonPropertyName("resource")]
        public DocumentReferenceResource resource { get; set; }
    }

    public class DocumentReferenceResource
    {
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("status")]
        public string status { get; set; }

        [JsonPropertyName("type")]
        public DocumentReferenceType type { get; set; }

        [JsonPropertyName("date")]
        public string date { get; set; }

        [JsonPropertyName("content")]
        public List<DocumentReferenceContent> content { get; set; }

        [JsonPropertyName("context")]
        public DocumentReferenceContext context { get; set; }
    }

    public class DocumentReferenceType
    {
        [JsonPropertyName("coding")]
        public List<DocumentReferenceCoding> coding { get; set; }

        [JsonPropertyName("text")]
        public string text { get; set; }
    }

    public class DocumentReferenceCoding
    {
        [JsonPropertyName("system")]
        public string system { get; set; }

        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("display")]
        public string display { get; set; }
    }

    public class DocumentReferenceContent
    {
        [JsonPropertyName("attachment")]
        public DocumentReferenceAttachment attachment { get; set; }
    }

    public class DocumentReferenceAttachment
    {
        // This is the key field — contains the Binary URL like:
        // "https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Binary/12345"
        [JsonPropertyName("url")]
        public string url { get; set; }

        [JsonPropertyName("contentType")]
        public string contentType { get; set; }

        [JsonPropertyName("title")]
        public string title { get; set; }
    }

    public class DocumentReferenceContext
    {
        // The encounter this document belongs to
        [JsonPropertyName("encounter")]
        public List<DocumentReferenceReference> encounter { get; set; }

        [JsonPropertyName("period")]
        public DocumentReferencePeriod period { get; set; }
    }

    public class DocumentReferenceReference
    {
        [JsonPropertyName("reference")]
        public string reference { get; set; }  // e.g. "Encounter/98765"
    }

    public class DocumentReferencePeriod
    {
        [JsonPropertyName("start")]
        public string start { get; set; }

        [JsonPropertyName("end")]
        public string end { get; set; }
    }
}
