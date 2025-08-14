namespace emedl_chase.Model
{
    public class Encounter
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
            public Text text { get; set; }
            public Identifier[] identifier { get; set; }
            public string status { get; set; }
            public Class1 _class { get; set; }
            public Type[] type { get; set; }
            public Subject subject { get; set; }
            public Participant[] participant { get; set; }
            public Period period { get; set; }
            public Hospitalization hospitalization { get; set; }
            public Location[] location { get; set; }
            public Reasoncode[] reasonCode { get; set; }
        }

        public class Meta1
        {
            public DateTime lastUpdated { get; set; }
            public string[] profile { get; set; }
        }

        public class Text
        {
            public string status { get; set; }
            public string div { get; set; }
        }

        public class Class1
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

        public class Period
        {
            public DateTime start { get; set; }
            public DateTime end { get; set; }
        }

        public class Hospitalization
        {
            public Dischargedisposition dischargeDisposition { get; set; }
        }

        public class Dischargedisposition
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

        public class Identifier
        {
            public string system { get; set; }
            public string value { get; set; }
        }

        public class Type
        {
            public string text { get; set; }
            public Coding1[] coding { get; set; }
        }

        public class Coding1
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }

        public class Participant
        {
            public Type1[] type { get; set; }
            public Period1 period { get; set; }
            public Individual individual { get; set; }
        }

        public class Period1
        {
            public DateTime start { get; set; }
            public DateTime end { get; set; }
        }

        public class Individual
        {
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Type1
        {
            public Coding2[] coding { get; set; }
            public string text { get; set; }
        }

        public class Coding2
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }

        public class Location
        {
            public Location1 location { get; set; }
        }

        public class Location1
        {
            public string reference { get; set; }
            public string type { get; set; }
        }

        public class Reasoncode
        {
            public string text { get; set; }
        }

        public class Search
        {
            public string mode { get; set; }
        }

        public class finalresponse
        {
            public string? encounternote { get; set; }

            public string ? dos  { get; set; }

        }
    }
}
