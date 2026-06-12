using emedl_chase.Service;
using System.Xml.Linq;
using QuestPDF.Fluent;

namespace emedl_chase.DbModel
{
    public class ClinicalPdfService : IClinicalPdfService
    {
        public async Task<byte[]> GenerateClinicalPdfAsync( string xmlContent, string outputPath, string dos, string encounterReason)
        {
            var summary = ParseCcda(xmlContent);

            summary.Patient.DateOfService = dos;

            summary.Patient.EncounterReason = encounterReason;

            //Console.WriteLine($"Problems: {summary.Problems.Count}");
            //Console.WriteLine($"Medications: {summary.Medications.Count}");
            //Console.WriteLine($"Labs: {summary.LabResults.Count}");
            //Console.WriteLine($"Provider: {summary.Patient.ProviderName}");

            var pdfBytes = new ClinicalPdfDocument(summary).GeneratePdf();

            await File.WriteAllBytesAsync( outputPath, pdfBytes);

            return pdfBytes;
        }

        private ClinicalSummary ParseCcda(string xml)
        {
            XDocument doc = XDocument.Parse(xml);

            XNamespace ns = "urn:hl7-org:v3";

            var summary = new ClinicalSummary();

            ParsePatient(doc, ns, summary);
            
            ParseProvider(doc, ns, summary);
            
            //ParseEncounters(doc, ns, summary);

            ParseAllergies(doc, ns, summary);

            ParseProblems(doc, ns, summary);

            ParseMedications(doc, ns, summary);

            ParseLabResults(doc, ns, summary);
            
            
            
           

            return summary;
        }

        private static void ParsePatient( XDocument doc, XNamespace ns, ClinicalSummary summary)
        {
            var patientNode = doc.Descendants(ns + "patient").FirstOrDefault();

            var patientRole =doc.Descendants(ns + "patientRole").FirstOrDefault();

            if (patientNode == null)
                return;

            var givenName = patientNode.Element(ns + "name") ?.Element(ns + "given")?.Value;

            var familyName =patientNode.Element(ns + "name") ?.Element(ns + "family") ?.Value;

            var dobRaw =  patientNode.Element(ns + "birthTime")?.Attribute("value")  ?.Value;

            DateTime dob = DateTime.MinValue;

            int age = 0;

            if (!string.IsNullOrEmpty(dobRaw))
            {
                DateTime.TryParseExact(
                    dobRaw,
                    "yyyyMMdd",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out dob);

                age = DateTime.Today.Year - dob.Year;

                if (dob > DateTime.Today.AddYears(-age))
                    age--;
            }

            var address =
                patientRole?.Element(ns + "addr");

            summary.Patient = new PatientInfo
            {
                Name = $"{givenName} {familyName}",

                DOB = dob != DateTime.MinValue
                    ? dob.ToString("MM/dd/yyyy")
                    : "",

                Age = age,

                Gender =
                    patientNode.Element(ns + "administrativeGenderCode")
                               ?.Attribute("displayName")
                               ?.Value,

                Phone =
                    patientRole?.Elements(ns + "telecom")
                                .FirstOrDefault()
                                ?.Attribute("value")
                                ?.Value,

                Address =
                    $"{address?.Element(ns + "streetAddressLine")?.Value}, " +
                    $"{address?.Element(ns + "city")?.Value}, " +
                    $"{address?.Element(ns + "state")?.Value}"
            };
        }
        
        private static void ParseLabResults(  XDocument doc,   XNamespace ns, ClinicalSummary summary)
        {
            var observations =
                doc.Descendants(ns + "observation");

            foreach (var obs in observations)
            {
                var testName =
                    obs.Element(ns + "code")
                       ?.Attribute("displayName")
                       ?.Value;

                if (string.IsNullOrWhiteSpace(testName))
                    continue;

                summary.LabResults.Add(
                    new LabResultInfo
                    {
                        TestName = testName,

                        Value =
                            obs.Element(ns + "value")
                               ?.Attribute("value")
                               ?.Value,

                        ReferenceRange =
                            obs.Descendants(ns + "observationRange")
                               .FirstOrDefault()
                               ?.Element(ns + "text")
                               ?.Value
                    });
            }
        }

        private static void ParseAllergies( XDocument doc,  XNamespace ns, ClinicalSummary summary)
        {
            var allergySection =
                doc.Descendants(ns + "section")
                   .FirstOrDefault(x =>
                       (string)x.Element(ns + "title")
                       == "Allergies");

            if (allergySection == null)
                return;

            var allergyText =
                allergySection.Element(ns + "text")
                              ?.Value;

            if (!string.IsNullOrWhiteSpace(allergyText))
            {
                summary.Allergies.Add(allergyText);
            }
        }

        private static void ParseProvider( XDocument doc, XNamespace ns,  ClinicalSummary summary)
        {
            var author =
                doc.Descendants(ns + "assignedAuthor")
                   .FirstOrDefault();

            if (author == null)
                return;

            var name =
                author.Descendants(ns + "name")
                      .FirstOrDefault();

            if (name == null)
                return;

            var given =
                name.Element(ns + "given")?.Value;

            var family =
                name.Element(ns + "family")?.Value;

            summary.Patient.ProviderName =
                $"{given} {family}";
        }

        private static void ParseProblems( XDocument doc,  XNamespace ns,  ClinicalSummary summary)
        {
            var problemSection = doc.Descendants(ns + "section").FirstOrDefault(x =>(x.Element(ns + "title")?.Value ?? "").Contains("Problem"));

            if (problemSection == null)
                return;

            var diagnoses = problemSection.Descendants(ns + "value");

            foreach (var diagnosis in diagnoses)
            {
                var displayName =
                    diagnosis.Attribute("displayName")
                             ?.Value;

                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    summary.Problems.Add(
                        new ProblemInfo
                        {
                            Diagnosis = displayName
                        });
                }
            }
        }

        private static void ParseMedications(  XDocument doc,  XNamespace ns, ClinicalSummary summary)
        {
            var medSection = doc.Descendants(ns + "section").FirstOrDefault(x => (x.Element(ns + "title")?.Value ?? "").Contains("Medication"));

            if (medSection == null)
                return;

            var meds = medSection.Descendants(ns + "manufacturedProduct");

            foreach (var med in meds)
            {
                var medName = med.Descendants(ns + "name").FirstOrDefault() ?.Value;

                if (!string.IsNullOrWhiteSpace(medName))
                {
                    summary.Medications.Add(
                        new MedicationInfo
                        {
                            MedicationName = medName
                        });
                }
            }
        }


    }


}
