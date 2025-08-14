namespace emedl_chase.Model
{
    public class DocRefresh
    {
        public class Rootobject
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public string type { get; set; }
            public int total { get; set; }
            public Link[] link { get; set; }
            public Entry[] entry { get; set; }
        }

        public class Meta
        {
            public DateTime lastUpdated { get; set; }
        }

        public class Link
        {
            public string relation { get; set; }
            public string url { get; set; }
        }

        public class Entry
        {
            public string fullUrl { get; set; }
            public Resource resource { get; set; }
            public Search search { get; set; }
        }

        public class Resource
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta1 meta { get; set; }
            public string contentType { get; set; }
            public string data { get; set; }
            public Text text { get; set; }
            public Identifier[] identifier { get; set; }
            public string status { get; set; }
            public Type type { get; set; }
            public Category[] category { get; set; }
            public Subject subject { get; set; }
            public DateTime date { get; set; }
            public Author[] author { get; set; }
            public Custodian custodian { get; set; }
            public Content[] content { get; set; }
            public Context context { get; set; }
        }

        public class Meta1
        {
            public string[] profile { get; set; }
            public DateTime lastUpdated { get; set; }
        }

        public class Text
        {
            public string status { get; set; }
            public string div { get; set; }
        }

        public class Type
        {
            public Coding[] coding { get; set; }
            public string text { get; set; }
        }

        public class Coding
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }

        public class Subject
        {
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Custodian
        {
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Context
        {
            public Encounter[] encounter { get; set; }
            public Period period { get; set; }
        }

        public class Period
        {
            public DateTime end { get; set; }
            public Extension[] extension { get; set; }
            public DateTime start { get; set; }
        }

        public class Extension
        {
            public string url { get; set; }
            public string valueCode { get; set; }
        }

        public class Encounter
        {
            public Extension1[] extension { get; set; }
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Extension1
        {
            public string url { get; set; }
            public string valueCode { get; set; }
        }

        public class Identifier
        {
            public string system { get; set; }
            public string value { get; set; }
        }

        public class Category
        {
            public Coding1[] coding { get; set; }
            public string text { get; set; }
        }

        public class Coding1
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }

        public class Author
        {
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Content
        {
            public Attachment attachment { get; set; }
            public Format format { get; set; }
        }

        public class Attachment
        {
            public string contentType { get; set; }
            public string url { get; set; }
            public string data { get; set; }
        }

        public class Format
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }

        public class Search
        {
            public string mode { get; set; }
        }

        public class finalresonse
        {
            public string binaryurl { get; set; }

            public string type { get; set; }

            public string binaryid { get; set; }
        }
    }
}