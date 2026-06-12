
namespace emedl_chase.Model
{
    public class PatientClinicalSummary
    {
        //public PatientDTO Patient { get; set; }
        //public PatientDTO.fhirid Patient { get; set; }
        public PatientHeaderDTO Patient { get; set; }

        public List<EncounterDTO> Encounters { get; set; } = new();

        public List<ConditionDTO> Conditions { get; set; } = new();

        public List<MedicationDTO> Medications { get; set; } = new();

        public List<Alergy> Allergies { get; set; } = new();

        public List<ObservationDTO> Observations { get; set; } = new();

        public List<ProcedureDTO> Procedures { get; set; } = new();

        public List<DiagnosticReportDTO> DiagnosticReports { get; set; } = new();

        public List<ImmunizationDTO> Immunizations { get; set; } = new();
        public List<DocumentReferenceDTO> Documents { get; set; }
    }
    public class ConditionDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ClinicalStatus { get; set; }

        public string VerificationStatus { get; set; }

        public string RecordedDate { get; set; }
    }


    //public class ObservationDTO
    //{
    //    public string id { get; set; }
    //    public string status { get; set; }

    //    public obsCodeableConcept code { get; set; }

    //    public Quantity valueQuantity { get; set; }

    //    public string effectiveDateTime { get; set; }
    //}
    public class ObservationDTO
    {
        public string ObservationId { get; set; }
        public string ObservationName { get; set; }
        public obsQuantity valueQuantity { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

        public string Status { get; set; }
        public string EffectiveDate { get; set; }
    }
    public class ObservationEntry
    {
        public string fullUrl { get; set; }
        public ObservationResource resource { get; set; }
    }
    //public class obsCodeableConcept
    //{
    //    public string text { get; set; }
    //}

    public class obsQuantity
    {
        public decimal? value { get; set; }
        public string unit { get; set; }
    }

    public class ObservationResource
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
    public class Alergy
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
    //public class ProcedureDTO
    //{
    //    public string ProcedureName { get; set; }

    //    public string Status { get; set; }

    //    public string PerformedDate { get; set; }
    //}

    public class ProcedureDTO
    {
        public string id { get; set; }
        public string ProcedureName { get; set; }
        public CodeableConcept code { get; set; }
        public string Status { get; set; }
        public string PerformedDate { get; set; }
    }

   
    public class DiagnosticReportDTO
    {
        public string ReportName { get; set; }

        public string Status { get; set; }

        public string Conclusion { get; set; }
    }
    public class ImmunizationDTO
    {
        public string Vaccine { get; set; }

        public string Status { get; set; }

        public string Date { get; set; }
    }

    public class DocumentReferenceDTO
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public string Date { get; set; }

        public string AttachmentUrl { get; set; }
        public string RawContent { get; set; }
    }




    public class FhirBundle<T>
    {
        public string resourceType { get; set; }
        public List<FhirEntry<T>> entry { get; set; }
    }

    public class FhirEntry<T>
    {
        public T resource { get; set; }
    }


}
