using emedl_chase.DbModel;
using emedl_chase.Helper;
using emedl_chase.Model;
using emedl_chase.Service;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
//using YourApp.Clinical;
using static emedl_chase.Model.EncounterDTO;

namespace emedl_chase.Controllers
{
    public class ECWController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClinicalPdfService _clinicalPdfService;
        public ECWController(IWebHostEnvironment webHostEnvironment, IClinicalPdfService clinicalPdfService)
        {
            _webHostEnvironment = webHostEnvironment;
            _clinicalPdfService = clinicalPdfService;
        }

        [HttpGet("Get-ECW-BearerToken")]
        public async Task<IActionResult> Index()
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);

            return Ok(bearer);
        }

        [HttpGet("GetPatientRecord")]
        public async Task<IActionResult> GetPatientRecord(string name = null, DateTime? dos = null)
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (name == null)
            {
                return Ok("Enter the name");
            }

            var convert_dos = dos?.ToString("yyyy-MM-dd");

            if (!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);


            var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name); //Patient

            var fhir_id = "";

            foreach (var item in get_patient)
            {
                fhir_id = item.fhir_id;
            }
                        
            //var get_Condition = await FhirApiCaller.ConditionAPI(bearer, fhir_id); //Conditions
            //var get_Allergy = await FhirApiCaller.AllergyIntoleranceAPI(bearer, fhir_id);//Allergy
            //var get_Medication_Request = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationRequest
            ////var get_Medication_Statement = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationStatement
            //var get_immunizations = await FhirApiCaller.ImmunizationAPI(bearer, fhir_id);//Immunisation
            ////var get_Observations = await FhirApiCaller.ObservationAPI(bearer, fhir_id);//Observations
            ////var get_procedures = await FhirApiCaller.ProcedureAPI(bearer, fhir_id);//Procedures
            //var get_DiagnosisReports = await FhirApiCaller.DiagnosticReportAPI(bearer, fhir_id);//Diagnosis Report

            var get_DocumentReference = await FhirApiCaller.DocumentReferenceAPI(bearer, fhir_id);//Document Reference

            var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);//Encounter
            var encounter = get_encounter_json.FirstOrDefault();

            var encounter_id = "";

            foreach (var item in get_encounter_json)
            {
                encounter_id = item.encounterid;
            }

            var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);


            var type = "encounter";

            if (get_docencounterwithpatient_json != null)
            {
                var call_xml_reader_file = XmlConvertor.XmlConvertorUpdated(get_docencounterwithpatient_json.encounterxmldata, name, convert_dos, type);
            }


            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
            var binaryId = get_binary_data.binaryid;            

            //var encounter = encounters.FirstOrDefault();

            //string get_CCDA_binary;

            //if (encounter != null)
            //{

            //    get_CCDA_binary = await ECWFHIRApiCaller.GetCcdaXmlByEncounter(bearer, fhir_id, encounter.encounterid, name, convert_dos);                
            //    if (string.IsNullOrWhiteSpace(get_CCDA_binary))
            //        get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);
            //}
            //else
            //{
            //    get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);
            //}

            var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);

            //if(get_CCDA_binary != null)
            if (!string.IsNullOrWhiteSpace(get_CCDA_binary))
            {

                var debugDoc = new XmlDocument();
                debugDoc.LoadXml(get_CCDA_binary);
                var debugNs = new XmlNamespaceManager(debugDoc.NameTable);
                debugNs.AddNamespace("cda", "urn:hl7-org:v3");
                foreach (XmlNode sec in debugDoc.SelectNodes("//cda:section", debugNs))
                {
                    string t = sec.SelectSingleNode("cda:title", debugNs)?.InnerText;
                    string c = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["code"]?.Value;
                    string d = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["displayName"]?.Value;
                    Console.WriteLine($"SECTION → title='{t}'  code='{c}'  displayName='{d}'");
                }


                ////var sections = XmlConvertor.ParseCCDASections(get_CCDA_binary);
                ////string pdfPath = XmlConvertor.GenerateClinicalPdf(patient, sections, "JohnDoe_CCDA");

                var sections = PDFConvertor.ParseCCDASectionsFromString(get_CCDA_binary, convert_dos);
                var patient = PDFConvertor.ExtractPatientFromCCDA(get_CCDA_binary);

                patient.DateOfService = convert_dos;


                //new method GenerateSoapNote               
                //string pdfPath = XmlConvertor.GenerateClinicalPdf_SoapNote(patient, sections, $"{name}_CCDA");

                //working code without generatesoapnote
                // 4. Generate PDF
                string pdfPath = XmlConvertor.GenerateClinicalPdfNew(patient, sections, $"{name}_CCDA");

                byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath);
                return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");


                //// Generate PDF
                ////GeneratePdf(reportText,   @"D:\Temp\ClinicalNote.pdf");


                ////WORKING CODE
                //var content = XmlConvertor.CCDAFilread(get_CCDA_binary);

                //var result = XmlConvertor.ReadCCDAFileOld(get_CCDA_binary);
            }
            
           


            return Ok(get_patient);

        }

        //[HttpGet("GetPatientRecordNew")]
        //public async Task<IActionResult> GetPatientRecordNew(string name = null, DateTime? dos = null)
        //{
        //    var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

        //    if (name == null)
        //    {
        //        return Ok("Enter the name");
        //    }

        //    var convert_dos = dos?.ToString("yyyy-MM-dd");

        //    if (!System.IO.File.Exists(filepath))
        //    {
        //        return Ok("file not found");
        //    }

        //    var json_data = System.IO.File.ReadAllText(filepath);

        //    var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);

        //    var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);


        //    var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name); //Patient

        //    var fhir_id = "";

        //    foreach (var item in get_patient)
        //    {
        //        fhir_id = item.fhir_id;
        //    }

        //    //var get_Condition = await FhirApiCaller.ConditionAPI(bearer, fhir_id); //Conditions
        //    //var get_Allergy = await FhirApiCaller.AllergyIntoleranceAPI(bearer, fhir_id);//Allergy
        //    //var get_Medication_Request = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationRequest
        //    ////var get_Medication_Statement = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationStatement
        //    //var get_immunizations = await FhirApiCaller.ImmunizationAPI(bearer, fhir_id);//Immunisation
        //    ////var get_Observations = await FhirApiCaller.ObservationAPI(bearer, fhir_id);//Observations
        //    ////var get_procedures = await FhirApiCaller.ProcedureAPI(bearer, fhir_id);//Procedures
        //    //var get_DiagnosisReports = await FhirApiCaller.DiagnosticReportAPI(bearer, fhir_id);//Diagnosis Report

        //    var get_DocumentReference = await FhirApiCaller.DocumentReferenceAPI(bearer, fhir_id);//Document Reference

        //    var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);//Encounter
        //    var encounter = get_encounter_json.FirstOrDefault();

        //    var encounter_id = "";

        //    foreach (var item in get_encounter_json)
        //    {
        //        encounter_id = item.encounterid;
        //    }

        //    var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);


        //    var type = "encounter";

        //    if (get_docencounterwithpatient_json != null)
        //    {
        //        var call_xml_reader_file = XmlConvertor.XmlConvertorUpdated(get_docencounterwithpatient_json.encounterxmldata, name, convert_dos, type);
        //    }


        //    var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
        //    var binaryId = get_binary_data.binaryid;

        //    if(binaryId != null )
        //    {
        //        var get_binary_xmldate = await FhirApiCaller.CallApiForBinary(bearer, binaryId);
        //        var call_xml_reader_file_1 = XmlConvertor.XmlConvertorUpdated(get_binary_xmldate, name, convert_dos, "Full");
        //    }

        //    var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);

        //    //if(get_CCDA_binary != null)
        //    if (!string.IsNullOrWhiteSpace(get_CCDA_binary))
        //    {

        //        var debugDoc = new XmlDocument();
        //        debugDoc.LoadXml(get_CCDA_binary);
        //        var debugNs = new XmlNamespaceManager(debugDoc.NameTable);
        //        debugNs.AddNamespace("cda", "urn:hl7-org:v3");
        //        foreach (XmlNode sec in debugDoc.SelectNodes("//cda:section", debugNs))
        //        {
        //            string t = sec.SelectSingleNode("cda:title", debugNs)?.InnerText;
        //            string c = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["code"]?.Value;
        //            string d = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["displayName"]?.Value;
        //            Console.WriteLine($"SECTION → title='{t}'  code='{c}'  displayName='{d}'");
        //        }


        //        ////var sections = XmlConvertor.ParseCCDASections(get_CCDA_binary);
        //        ////string pdfPath = XmlConvertor.GenerateClinicalPdf(patient, sections, "JohnDoe_CCDA");

        //        var sections = PDFConvertor.ParseCCDASectionsFromString(get_CCDA_binary, convert_dos);
        //        var patient = PDFConvertor.ExtractPatientFromCCDA(get_CCDA_binary);

        //        patient.DateOfService = convert_dos;




        //        //Map your existing patient +sections into ClinicalNote
        //        var note = new ClinicalNoteNew
        //        {
        //            PatientName = $"{patient.Name}".Trim(),
        //            DateOfService = patient.DateOfService,
        //            Provider = patient.Provider,
        //            DateOfBirth = patient.DOB,          
        //            Age = patient.Age?.ToString(),
        //            //Sex = patient.Sex,
        //            AccountNumber = patient.AccountNumber,
        //            Phone = patient.Phone,
        //            Address = patient.Address
        //        };

        //        //foreach (var section in sections)
        //        //{
        //        //    if (string.IsNullOrEmpty(section.Section)) continue;

        //        //    if (!note.Sections.ContainsKey(section.Section))
        //        //        note.Sections[section.Section] = new List<string>();

        //        //    note.Sections[section.Section].Add(section.Value);
        //        //}

        //        foreach (var section in sections)
        //        {
        //            if (string.IsNullOrEmpty(section.Section)) continue;
        //            if (!note.Sections.ContainsKey(section.Section))
        //                note.Sections[section.Section] = new List<string>();
        //            note.Sections[section.Section].Add(section.Value);
        //        }

        //        // ── Override Assessment with direct XML parse (accurate + DOS-filtered) ──
        //        var assessmentRows = ClinicalPdfGenerator.ParseAssessmentFromXml(get_CCDA_binary, convert_dos);
        //        Console.WriteLine($"[ASSESS DEBUG] dos='{convert_dos}' rows={assessmentRows.Count}");

        //        if (assessmentRows.Any())
        //        {
        //            // Replace the concatenated text version with clean XML-parsed version
        //            note.Sections["MDM or Assessment and Plan"] = assessmentRows
        //                .Select(r => r.Value)
        //                .ToList();
        //        }

        //        // ADD THIS DEBUG
        //        Console.WriteLine("=== NOTE.SECTIONS CONTENT ===");
        //        foreach (var kvp in note.Sections)
        //        {
        //            Console.WriteLine($"  SECTION: '{kvp.Key}' rows={kvp.Value.Count}");
        //            foreach (var v in kvp.Value)
        //                Console.WriteLine($"    VALUE: '{v?.Substring(0, Math.Min(120, v?.Length ?? 0))}'");

        //        }
        //        //byte[] fileBytes = ClinicalPdfGenerator.Generate(note);
        //        //return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");

        //        Console.WriteLine($"[DOS] convert_dos = '{convert_dos}'");


        //        byte[] fileBytes = ClinicalPdfGenerator.Generate(note, dosFilter: convert_dos);
        //        return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");


        //        ////WORKING CODE
        //        //var content = XmlConvertor.CCDAFilread(get_CCDA_binary);

        //        //var result = XmlConvertor.ReadCCDAFileOld(get_CCDA_binary);
        //    }




        //    return Ok(get_patient);

        //}




        [HttpGet("GetPatientRecordNew")]
        public async Task<IActionResult> GetPatientRecordNew(string name = null, DateTime? dos = null)
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (name == null)
            {
                return Ok("Enter the name");
            }

            var convert_dos = dos?.ToString("yyyy-MM-dd");

            if (!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);


            var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name); //Patient

            var fhir_id = "";

            foreach (var item in get_patient)
            {
                fhir_id = item.fhir_id;
            }

            //var get_Condition = await FhirApiCaller.ConditionAPI(bearer, fhir_id); //Conditions
            //var get_Allergy = await FhirApiCaller.AllergyIntoleranceAPI(bearer, fhir_id);//Allergy
            //var get_Medication_Request = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationRequest
            ////var get_Medication_Statement = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationStatement
            //var get_immunizations = await FhirApiCaller.ImmunizationAPI(bearer, fhir_id);//Immunisation
            ////var get_Observations = await FhirApiCaller.ObservationAPI(bearer, fhir_id);//Observations
            ////var get_procedures = await FhirApiCaller.ProcedureAPI(bearer, fhir_id);//Procedures
            //var get_DiagnosisReports = await FhirApiCaller.DiagnosticReportAPI(bearer, fhir_id);//Diagnosis Report

            var get_DocumentReference = await FhirApiCaller.DocumentReferenceAPI(bearer, fhir_id);//Document Reference

            var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);//Encounter
            var encounter = get_encounter_json.FirstOrDefault();

            var encounter_id = "";

            foreach (var item in get_encounter_json)
            {
                encounter_id = item.encounterid;
            }

            var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);


            var type = "encounter";

            if (get_docencounterwithpatient_json != null)
            {
                var call_xml_reader_file = XmlConvertor.XmlConvertorUpdated(get_docencounterwithpatient_json.encounterxmldata, name, convert_dos, type);
            }


            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
            var binaryId = get_binary_data.binaryid;

            if (binaryId != null)
            {
                var get_binary_xmldate = await FhirApiCaller.CallApiForBinary(bearer, binaryId);
                var call_xml_reader_file_1 = XmlConvertor.XmlConvertorUpdated(get_binary_xmldate, name, convert_dos, "Full");
            }

            var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);


            

            if (!string.IsNullOrWhiteSpace(get_CCDA_binary))
            {
                
                var note = CcdaParserNew.Parse(get_CCDA_binary, convert_dos);

                // ── STEP 2: (Optional) Override DateOfService if your caller provides it ──
                if (!string.IsNullOrEmpty(convert_dos))
                    note.DateOfService = convert_dos;

                
                foreach (var kvp in note.Sections)
                    Console.WriteLine($"SECTION: {kvp.Key} = {kvp.Value.Count} rows");

                byte[] fileBytes = ClinicalPdfGeneratorNew.Generate(note);

                return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");
            }




            return Ok(get_patient);

        }


        [HttpGet("GetECWPatientRecord")]
        public async Task<IActionResult> GetECWPatientRecord(string name = null, DateTime? dos = null)
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (name == null)
            {
                return Ok("Enter the name");
            }

            var convert_dos = dos?.ToString("yyyy-MM-dd");

            if (!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);


            var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name); //Patient

            var fhir_id = "";

            foreach (var item in get_patient)
            {
                fhir_id = item.fhir_id;
            }



            //var get_Encounter = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);//Encounter
            var get_Condition = await FhirApiCaller.ConditionAPI(bearer, fhir_id); //Conditions
            var get_Allergy = await FhirApiCaller.AllergyIntoleranceAPI(bearer, fhir_id);//Allergy
            var get_Medication_Request = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationRequest
            //var get_Medication_Statement = await FhirApiCaller.MedicationAPI(bearer, fhir_id);//MedicationStatement
            var get_immunizations = await FhirApiCaller.ImmunizationAPI(bearer, fhir_id);//Immunisation
            //var get_Observations = await FhirApiCaller.ObservationAPI(bearer, fhir_id);//Observations
            //var get_procedures = await FhirApiCaller.ProcedureAPI(bearer, fhir_id);//Procedures
            var get_DiagnosisReports = await FhirApiCaller.DiagnosticReportAPI(bearer, fhir_id);//Diagnosis Report

            //CarePlan
            //CareTeam
            var get_DocumentReference = await FhirApiCaller.DocumentReferenceAPI(bearer, fhir_id);//Document Reference
            //Composition
            //Specimen
            //Device
            //Practitioner
            //Organization
            //Coverage
            //Goal



            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
            var binaryId = get_binary_data.binaryid;

            var encounters = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);
            var encounter = encounters.FirstOrDefault();
            
            var encounter_id = "";

            foreach (var item in encounters)
            {
                encounter_id = item.encounterid;
            }
            string get_CCDA_binary;

            if (encounter != null)
            {

                get_CCDA_binary = await ECWFHIRApiCaller.GetCcdaXmlByEncounter(bearer, fhir_id, encounter.encounterid, name, convert_dos);
                if (string.IsNullOrWhiteSpace(get_CCDA_binary))
                    get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);
            }
            else
            {
                get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);
            }




            //var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);

            //if(get_CCDA_binary != null)
            if (!string.IsNullOrWhiteSpace(get_CCDA_binary))
            {

                var debugDoc = new XmlDocument();
                debugDoc.LoadXml(get_CCDA_binary);
                var debugNs = new XmlNamespaceManager(debugDoc.NameTable);
                debugNs.AddNamespace("cda", "urn:hl7-org:v3");
                foreach (XmlNode sec in debugDoc.SelectNodes("//cda:section", debugNs))
                {
                    string t = sec.SelectSingleNode("cda:title", debugNs)?.InnerText;
                    string c = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["code"]?.Value;
                    string d = sec.SelectSingleNode("cda:code", debugNs)?.Attributes?["displayName"]?.Value;
                    Console.WriteLine($"SECTION → title='{t}'  code='{c}'  displayName='{d}'");
                }


                //var sections = XmlConvertor.ParseCCDASections(get_CCDA_binary);
                //string pdfPath = XmlConvertor.GenerateClinicalPdf(patient, sections, "JohnDoe_CCDA");

                var sections = PDFConvertor.ParseCCDASectionsFromString(get_CCDA_binary, convert_dos);

                // 2. Extract patient (THIS FIXES YOUR ERROR)
                var patient = PDFConvertor.ExtractPatientFromCCDA(get_CCDA_binary);

                // 3. Add fallback values if needed
                patient.DateOfService = convert_dos;

                // 4. Generate PDF
                string pdfPath = XmlConvertor.GenerateClinicalPdfNew(patient, sections, $"{name}_CCDA");
                byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath);
                return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");


            }




            return Ok(get_patient);

        }

        public static ClinicalNote ParseCcda(string xml)
        {
            var doc = XDocument.Parse(xml);

            XNamespace ns = "urn:hl7-org:v3";

            var note = new ClinicalNote();

            foreach (var section in doc.Descendants(ns + "section"))
            {
                var title = section.Element(ns + "title")?.Value?.Trim();

                var text = section.Element(ns + "text")?.Value?.Trim();

                if (string.IsNullOrEmpty(title))
                    continue;

                switch (title.ToUpper())
                {
                    case "CHIEF COMPLAINT":
                        note.ChiefComplaint = text;
                        break;

                    case "HISTORY OF PRESENT ILLNESS":
                        note.HPI = text;
                        break;

                    case "REVIEW OF SYSTEMS":
                        note.ROS = text;
                        break;

                    case "PAST MEDICAL HISTORY":
                        note.PMH = text;
                        break;

                    case "FAMILY HISTORY":
                    case "SOCIAL HISTORY":
                        note.PMSFH += Environment.NewLine + text;
                        break;

                    case "ALLERGIES":
                        note.Allergy = text;
                        break;

                    case "PHYSICAL EXAM":
                        note.PhysicalExam = text;
                        break;

                    case "ASSESSMENT":
                    case "PLAN OF CARE":
                        note.AssessmentPlan += Environment.NewLine + text;
                        break;

                    case "ORDERS":
                        note.Orders = text;
                        break;

                    case "REFERRALS":
                        note.Referrals = text;
                        break;
                }
            }

            return note;
        }

        public static string BuildHistoryAndPhysical(ClinicalNote note)
        {
            var sb = new StringBuilder();

            sb.AppendLine("CHIEF COMPLAINT");
            sb.AppendLine(note.ChiefComplaint);

            sb.AppendLine();
            sb.AppendLine("HPI");
            sb.AppendLine(note.HPI);

            sb.AppendLine();
            sb.AppendLine("ROS");
            sb.AppendLine(note.ROS);

            sb.AppendLine();
            sb.AppendLine("PMH");
            sb.AppendLine(note.PMH);

            sb.AppendLine();
            sb.AppendLine("PMSFH");
            sb.AppendLine(note.PMSFH);

            sb.AppendLine();
            sb.AppendLine("ALLERGIES");
            sb.AppendLine(note.Allergy);

            sb.AppendLine();
            sb.AppendLine("PHYSICAL EXAM");
            sb.AppendLine(note.PhysicalExam);

            sb.AppendLine();
            sb.AppendLine("ASSESSMENT AND PLAN");
            sb.AppendLine(note.AssessmentPlan);

            sb.AppendLine();
            sb.AppendLine("ORDERS");
            sb.AppendLine(note.Orders);

            sb.AppendLine();
            sb.AppendLine("TREATMENT PLAN");
            sb.AppendLine(note.TreatmentPlan);

            sb.AppendLine();
            sb.AppendLine("REFERRALS");
            sb.AppendLine(note.Referrals);

            sb.AppendLine();
            sb.AppendLine("Electronically signed by");
            sb.AppendLine(note.ElectronicallySignedBy);

            return sb.ToString();
        }

        public static string BuildSoapNote(ClinicalNote note)
        {
            var sb = new StringBuilder();

            sb.AppendLine("SUBJECTIVE");

            sb.AppendLine("Chief Complaint");
            sb.AppendLine(note.ChiefComplaint);

            sb.AppendLine();
            sb.AppendLine("HPI");
            sb.AppendLine(note.HPI);

            sb.AppendLine();
            sb.AppendLine("ROS");
            sb.AppendLine(note.ROS);

            sb.AppendLine();
            sb.AppendLine("PMH");
            sb.AppendLine(note.PMH);

            sb.AppendLine();
            sb.AppendLine("PMSFH");
            sb.AppendLine(note.PMSFH);

            sb.AppendLine();
            sb.AppendLine("Allergies");
            sb.AppendLine(note.Allergy);

            sb.AppendLine();
            sb.AppendLine("OBJECTIVE");

            sb.AppendLine("Physical Exam");
            sb.AppendLine(note.PhysicalExam);

            sb.AppendLine();
            sb.AppendLine("ASSESSMENT AND PLAN");
            sb.AppendLine(note.AssessmentPlan);

            sb.AppendLine();
            sb.AppendLine("Orders");
            sb.AppendLine(note.Orders);

            sb.AppendLine();
            sb.AppendLine("Referrals");
            sb.AppendLine(note.Referrals);

            sb.AppendLine();
            sb.AppendLine("Electronically signed by");
            sb.AppendLine(note.ElectronicallySignedBy);

            return sb.ToString();
        }

    }
}
