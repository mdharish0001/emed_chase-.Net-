namespace emedl_chase.Model
{
    public class DocRefreshEncounterWithPatient
    {
        public class finalresonse
        {
            public string encounterxmldata { get; set; }  // decoded XML string
            public string encounterdate { get; set; }
        }

        // FHIR Bundle
        public class Rootobject
        {
            public string resourceType { get; set; }
            public int total { get; set; }
            public Entry[] entry { get; set; }
        }

        public class Entry
        {
            public Resource resource { get; set; }
        }

        public class Resource
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public string status { get; set; }
            public Content[] content { get; set; }  // ← data lives here
            public Context context { get; set; }
            public string data { get; set; }
        }

        // content[].attachment.data  ← this is where the base64 XML lives
        public class Content
        {
            public Attachment attachment { get; set; }
        }

        public class Attachment
        {
            public string contentType { get; set; }  // "application/xml" or "text/plain"
            public string data { get; set; }  // base64 encoded content
            public string url { get; set; }  // OR a URL to fetch separately
        }

        public class Context
        {
            public Period period { get; set; }
            public Encounter[] encounter { get; set; }
        }

        public class Encounter
        {
            public string reference { get; set; }  // "Encounter/123"
        }

        public class Period
        {
            public DateTime? start { get; set; }
            public DateTime? end { get; set; }
        }
    }
}
