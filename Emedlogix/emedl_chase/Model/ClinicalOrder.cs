using System.Text;
using System.Xml.Linq;

namespace emedl_chase.Model
{
    public class ClinicalSectionOrder
    {
        public static readonly List<string> Order = new()
    {
        "Chief complaint",
        "Reason for visit",
        "HPI",
        "Review of systems",
        "ROS",
        "Past medical history",
        "PMHFH",
        "Allergies",
        "Physical exam",
        "Assessment and plan",
        "MDM",
        "Orders",
        "Treatment plan",
        "Referrals",
        "Subjective",
        "Objective"
    };
    }

    public class ClinicalNote
    {
        public string ChiefComplaint { get; set; }
        public string ReasonForVisit { get; set; }
        public string HPI { get; set; }
        public string ROS { get; set; }
        public string PMH { get; set; }
        public string PMSFH { get; set; }
        public string Allergy { get; set; }
        public string PhysicalExam { get; set; }
        public string AssessmentPlan { get; set; }
        public string Orders { get; set; }
        public string TreatmentPlan { get; set; }
        public string Referrals { get; set; }
        public string ElectronicallySignedBy { get; set; }

        
    }
}
