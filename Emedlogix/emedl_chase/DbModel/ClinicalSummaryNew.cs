namespace emedl_chase.DbModel
{
    public class ClinicalSummaryNew
    {
        // Patient
        public string PatientName { get; set; }
        public string MRN { get; set; }
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }

        // Encounter
        public DateTime? DateOfService { get; set; }
        public string EncounterReason { get; set; }
        public string ProviderName { get; set; }

        // SOAP
        public string Subjective { get; set; }
        public string Objective { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }

        // CCD Sections
        public List<ProblemItem> Problems { get; set; } = new();
        public List<MedicationItem> Medications { get; set; } = new();
        public List<AllergyItem> Allergies { get; set; } = new();
        public List<LabResultItem> Labs { get; set; } = new();
    }

    public class ProblemItem
    {
        public string Diagnosis { get; set; }
        public string Description { get; set; }
    }
    public class MedicationItem
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
    }
    public class AllergyItem
    {
        public string Substance { get; set; }
        public string Reaction { get; set; }
    }
    public class LabResultItem
    {
        public string TestName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
