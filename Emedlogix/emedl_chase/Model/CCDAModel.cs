using emedl_chase.DbModel;

namespace emedl_chase.Model
{
    public class CCDAModel
    {
        public PatientInfo Patient { get; set; } = new();

        public List<string> Allergies { get; set; } = new();
        public List<string> Medications { get; set; } = new();
        public List<string> Problems { get; set; } = new();
        public List<string> Procedures { get; set; } = new();
        public List<string> Immunizations { get; set; } = new();
        public List<string> Results { get; set; } = new();
    }

    public class PatientInfon
    {
        public string Name { get; set; }
        public string MRN { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
    }
    public class CCDAHL7Model
    {
        public string section { get; set; }
        public string Sentence { get; set; }
        public string DateOfEncounter { get; set; }
        public int PageNo { get; set; }
    }

    public class CCDA_ToFileWriteModel
    { 
    public string Section { get; set; }
    public string Value { get; set; }
    }
}
