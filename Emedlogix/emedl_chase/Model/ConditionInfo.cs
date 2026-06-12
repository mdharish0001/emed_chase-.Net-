using Hl7.Fhir.Model;

namespace emedl_chase.Model
{
    public class ConditionInfo
    {
        public string ConditionId { get; set; }

        public string Diagnosis { get; set; }

        public string ClinicalStatus { get; set; }

        public string VerificationStatus { get; set; }

        public string Category { get; set; }

        public string Severity { get; set; }

        public DateTime? OnsetDate { get; set; }

        public DateTime? RecordedDate { get; set; }

        public string Recorder { get; set; }

        public string PatientReference { get; set; }

        public List<ConditionEntry> entry { get; set; }
    }

    public class ConditionEntry
    {
        public string fullUrl { get; set; }
        public ConditionResource resource { get; set; }
    }

    public class CodeableConcept
    {
        public string text { get; set; }

        public List<Coding> coding { get; set; }
    }

   
    public class Reference
    {
        public string reference { get; set; }

        public string display { get; set; }
    }
    public class ConditionResource
    {
        public string id { get; set; }

        public CodeableConcept code { get; set; }

        public CodeableConcept clinicalStatus { get; set; }

        public CodeableConcept verificationStatus { get; set; }

        public List<CodeableConcept> category { get; set; }

        public CodeableConcept severity { get; set; }

        public DateTime? onsetDateTime { get; set; }

        public DateTime? recordedDate { get; set; }

        public Reference recorder { get; set; }

        public Reference subject { get; set; }
    }
}
