namespace emedl_chase.Model
{
    public class UnifiedClinicalRecord
    {
        public string DateOfService { get; set; }
        public List<string> Diagnoses { get; set; } = new();
        public List<string> ProceduresU { get; set; } = new();

        public List<string> EncounterSummaries { get; set; } = new();

        public List<string> OperativeNotes { get; set; } = new();

        public List<string> ClinicalDocuments { get; set; } = new();

        public string FullNarrative { get; set; }

        public List<string> Observations { get; set; } = new();
        public List<string> MedicationsU { get; set; } = new();
        public List<string> Allergiesu { get; set; } = new();
        public List<string> Immunizationsu { get; set; } = new();
        public List<string> Documentsu { get; set; } = new();
        public List<string> CarePlans { get; set; } = new();
        public List<string> DiagnosticReports { get; set; } = new();
        public List<string> Specimens { get; set; } = new();
        public List<string> Devices { get; set; } = new();
        public string PatientInfo { get; set; }




        ///
        public PatientDTO.fhirid Patient { get; set; }

        public List<MedicationDTO> Medications { get; set; }

        public List<AlergyIntolerance> Allergies { get; set; }

        public List<ConditionInfo> Conditions { get; set; }

        public List<EncounterDTO.finalresponse> Encounters { get; set; }

        public List<ProcedureDTO> Procedures { get; set; }

        public List<DiagnosticReportDTO> Reports { get; set; }

        public List<ImmunizationDTO> Immunizations { get; set; }
        public List<DocumentReferenceDTO> Documents { get; set; }

        public SoapNote SoapNote { get; set; }
    }
    public class SoapNote
    {
        public string ReasonForAppointment { get; set; }

        public string ChiefComplaint { get; set; }

        public string HPI { get; set; }

        public string ROS { get; set; }

        public string Assessment { get; set; }

        public string Plan { get; set; }
    }

    public static class EncounterXmlParser
    {
        public static SoapNote Parse(string xml)
        {
            var note = new SoapNote();

            // Parsing logic goes here

            return note;
        }
    }
}
