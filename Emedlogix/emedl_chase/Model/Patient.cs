namespace emedl_chase.Model
{
    public class PatientDTO
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
            public bool active { get; set; }
            public Name[] name { get; set; }
            public Telecom[] telecom { get; set; }
            public string gender { get; set; }
            public string birthDate { get; set; }
            public Address[] address { get; set; }
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

        public class Identifier
        {
            public string use { get; set; }
            public string system { get; set; }
            public string value { get; set; }
        }

        public class Name
        {
            public string text { get; set; }
            public string family { get; set; }
            public string[] given { get; set; }
        }

        public class Telecom
        {
            public string system { get; set; }
            public string value { get; set; }
            public string use { get; set; }
        }

        public class Address
        {
            public string use { get; set; }
            public string type { get; set; }
            public string text { get; set; }
            public string[] line { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postalCode { get; set; }
            public string country { get; set; }
        }

        public class Search
        {
            public string mode { get; set; }
        }

        public class fhirid
        {
            public string name { get; set; }
            public string fhir_id { get; set; }

            public string gender { get; set; }

            public string birthDate { get; set; }

            public DateTime lastUpdated { get; set; }
            //public DateTime lastUpdated { get; set; }

            public string fullurl { get; set; }

            public bool active { get; set; }

            public string bundleid { get; set; }

        }

        

    }


    public class PatientHeaderDTO
    {
        public string FhirId { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public string BirthDate { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
    public class PatientInfoforPDF
    {
        public string Name { get; set; }
        public string DOB { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string AccountNumber { get; set; }
        public string DateOfService { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Provider { get; set; }
    }
}