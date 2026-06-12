namespace emedl_chase.DbModel
{
    public class ClinicalSummary
    {
        public PatientInfo Patient { get; set; } = new();

        public List<string> Allergies { get; set; } = new();

        public List<ProblemInfo> Problems { get; set; } = new();

        public List<MedicationInfo> Medications { get; set; } = new();

        public List<LabResultInfo> LabResults { get; set; } = new();

        public List<EncounterInfo> Encounters { get; set; } = new();
    }
    public class PatientInfo
    {
        public string Name { get; set; }

        public string DOB { get; set; }

        public string Gender { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string AccountNumber { get; set; }

        public int Age { get; set; }
        public string DateOfService { get; set; }
        public string ProviderName { get; set; }
        public string EncounterReason { get; set; }
    }

    public class ProblemInfo
    {
        public string Diagnosis { get; set; }
    }

    public class MedicationInfo
    {
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
    }

    public class LabResultInfo
    {
        public string TestName { get; set; }
        public string Value { get; set; }
        public string ReferenceRange { get; set; }
    }

    public class EncounterInfo
    {
        public string Date { get; set; }
        public string Provider { get; set; }
    }
}
