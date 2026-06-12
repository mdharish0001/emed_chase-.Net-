using Hl7.Fhir.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace emedl_chase.Model
{
    public class MedicationDTO
    {
        public string MedicationId { get; set; }
        public string MedicationName { get; set; }
        public string Status { get; set; }
        public string Intent { get; set; }
        public string DosageInstruction { get; set; }
        public string Route { get; set; }
        public string Frequency { get; set; }
        public DateTime? AuthoredOn { get; set; }
        public string Prescriber { get; set; }
    }
    public class MedicationEntry
    {
        public string fullUrl { get; set; }
        public MedicationResource resource { get; set; }
    }

    public class MedicationBundle
    {
        public List<MedicationEntry> entry { get; set; }
    }

    public class MedicationResource
    {
        public string id { get; set; }

        public MedicationCodeableConcept medicationCodeableConcept { get; set; }

        public string status { get; set; }

        public string intent { get; set; }

        public List<DosageInstruction> dosageInstruction { get; set; }

        public DateTime? authoredOn { get; set; }

        public Reference requester { get; set; }
    }

    public class MedicationCodeableConcept
    {
        public string text { get; set; }
    }

    public class DosageInstruction
    {
        public string text { get; set; }

        public Route route { get; set; }

        public Timing timing { get; set; }
    }

    public class Route
    {
        public string text { get; set; }
    }

    public class Timing
    {
        public Repeat repeat { get; set; }
    }

    public class Repeat
    {
        public int? frequency { get; set; }
    }

    public class MedicationReference
    {
        public string display { get; set; }

        public string reference { get; set; }
    }

}
