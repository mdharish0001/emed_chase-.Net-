namespace emedl_chase.Model
{
    public class AlergyIntolerance
    {
        public string alergyid { get; set; }
        public string alergyname { get; set; }

        public string clinicalStatus { get; set; }

        public string verificationStatus { get; set; }
        public string category { get; set; }
        public string criticality { get; set; }

        public DateTime recordedDate { get; set; }

        public string reaction { get; set; }
        public string reference { get; set; }

        public string recorder { get; set; }

    }

    public class AllergyBundle
    {
        public List<Entry> entry { get; set; }
    }

    public class Entry
    {
        public AllergyResource resource { get; set; }
    }

    public class AllergyResource
    {
        public string id { get; set; }

        public ClinicalStatus clinicalStatus { get; set; }

        public VerificationStatus verificationStatus { get; set; }

        public List<string> category { get; set; }

        public string criticality { get; set; }

        public Code code { get; set; }

        public DateTime? recordedDate { get; set; }

        public PatientReference patient { get; set; }

        public List<Reaction> reaction { get; set; }

        public Recorder recorder { get; set; }
    }

    public class ClinicalStatus
    {
        public List<Coding> coding { get; set; }
    }

    public class VerificationStatus
    {
        public List<Coding> coding { get; set; }
    }

    public class Coding
    {
        public string code { get; set; }
        public string display { get; set; }
    }

    public class Code
    {
        public string text { get; set; }
    }

    public class PatientReference
    {
        public string reference { get; set; }
    }

    public class Reaction
    {
        public List<Manifestation> manifestation { get; set; }
    }

    public class Manifestation
    {
        public string text { get; set; }
    }

    public class Recorder
    {
        public string display { get; set; }
    }
}
