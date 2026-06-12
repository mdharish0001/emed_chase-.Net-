using emedl_chase.Helper;
using emedl_chase.Model;
using emedl_chase.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
//using YourApp.Clinical;


namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClinicalPdfService _clinicalPdfService;


        public PatientController(IWebHostEnvironment webHostEnvironment, IClinicalPdfService clinicalPdfService)
        {
            _webHostEnvironment = webHostEnvironment;
            _clinicalPdfService = clinicalPdfService;
        }

        [HttpGet("Get-Token")]
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

        [HttpGet("GetRecordNew")]
        public async Task<IActionResult> GetRecordNew(string name = null, DateTime? dos = null)
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
            
            var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name);

            var fhir_id = "";
            foreach (var item in get_patient)
            {
                fhir_id = item.fhir_id;
            }

            var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);

            var dosEncounter = get_encounter_json?
                .FirstOrDefault(e => e.dos == convert_dos)   // exact DOS match
                ?? get_encounter_json?
                .OrderByDescending(e => e.dos)               // fallback: most recent
                .FirstOrDefault();

            var encounter_id = dosEncounter?.encounterid ?? "";

            Console.WriteLine($"[Controller] encounter_id='{encounter_id}' date='{dosEncounter?.dos}'");
            
            var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);

            var type = "encounter";
            if (get_docencounterwithpatient_json != null)
            {
                var call_xml_reader_file = XmlConvertor.XmlConvertorUpdated(get_docencounterwithpatient_json.encounterxmldata, name, convert_dos, type);
            }


            Console.WriteLine($"[CC/ROS] DocRef result: {(get_docencounterwithpatient_json == null ? "NULL" : "0")}");
            
            #region strategy to get cc and ros
            //strategy 1
            //string ccText = "";
            //string rosText = "";
            
            //// Strategy 1: CC from Encounter.reasonCode (most reliable, structured)
            //if (!string.IsNullOrEmpty(encounter_id))
            //{
            //    ccText = await FhirApiCaller.GetCcFromEncounterResource(bearer, encounter_id);
            //    Console.WriteLine($"[CC] From Encounter.reasonCode: '{ccText}'");
            //}

            ////strategy 2
            //if (get_docencounterwithpatient_json?.encounterxmldata != null)
            //{
            //    var (ccFromXml, rosFromXml) = FhirCcRosHelper.ExtractCcRosFromEncounterXml(get_docencounterwithpatient_json.encounterxmldata);

            //    if (string.IsNullOrEmpty(ccText) && !string.IsNullOrEmpty(ccFromXml))
            //        ccText = ccFromXml;

            //    if (!string.IsNullOrEmpty(rosFromXml))
            //        rosText = rosFromXml;

            //    Console.WriteLine($"[CC] From DocRef XML: '{ccFromXml}'");
            //    Console.WriteLine($"[ROS] From DocRef XML: '{rosFromXml?.Length} chars'");
            //}

            //// Strategy 3: ROS broader search if still empty
            //if (string.IsNullOrEmpty(rosText))
            //{
            //    rosText = await FhirApiCaller.GetRosFromDocumentReference(bearer, fhir_id, encounter_id, convert_dos);
            //    Console.WriteLine($"[ROS] From broader DocRef search: '{rosText?.Length} chars'");
            //}
            #endregion

            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
            var binaryId = get_binary_data?.binaryid ?? "";

            if (string.IsNullOrEmpty(binaryId))
                return NotFound("Full CCDA DocumentReference not found for this patient");


            var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);

            string ccText = "";
            string rosText = "";

            if (!string.IsNullOrWhiteSpace(get_CCDA_binary))
            {                              

                var note = CcdaParserNew.Parse(get_CCDA_binary, convert_dos);

                if (!string.IsNullOrEmpty(convert_dos))
                    note.DateOfService = convert_dos;
                
                // Check CC & ROS from XML 
                if (get_docencounterwithpatient_json?.encounterxmldata != null)
                {
                    var (ccFromXml, rosFromXml) = FhirCcRosHelper.ExtractCcRosFromEncounterXml(
                        get_docencounterwithpatient_json.encounterxmldata);

                    ccText = ccFromXml ?? "";
                    rosText = rosFromXml ?? "";
                    Console.WriteLine($"[CC/ROS] From encounter XML: cc='{ccText}' ros={rosText.Length} chars");
                }

                // CC from Encounter.reasonCode if encounter XML gave nothing
                if (string.IsNullOrEmpty(ccText) && !string.IsNullOrEmpty(encounter_id))
                {
                    ccText = await FhirApiCaller.GetCcFromEncounterResource(bearer, encounter_id);
                    Console.WriteLine($"[CC/ROS] Fallback from reasonCode: cc='{ccText}'");
                }

                // Inject into note (only if not already populated by CCDA)
                if (!string.IsNullOrEmpty(ccText)
                    && !note.Sections.ContainsKey(CcdaParserNew.SEC_CC))
                    note.Sections[CcdaParserNew.SEC_CC] = new List<string> { ccText };

                if (!string.IsNullOrEmpty(rosText)
                    && !note.Sections.ContainsKey(CcdaParserNew.SEC_ROS))
                    note.Sections[CcdaParserNew.SEC_ROS] = new List<string> { rosText };

                // ── Step 7: Log and generate PDF ────────────────────────────────
                foreach (var kvp in note.Sections)
                    Console.WriteLine($"SECTION: {kvp.Key} = {kvp.Value.Count} rows");

                byte[] fileBytes = ClinicalPdfGeneratorNew.Generate(note);

               
                var savedPath = SavePdfToFolder(fileBytes, note);
                Console.WriteLine($"[PDF] Saved to: {savedPath}");
                return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");

                
            }




            return Ok(get_patient);

        }


        private string SavePdfToFolder(byte[] pdfBytes, Model.ClinicalNoteNew note)
        {
            // ── 1. Build today's dated folder ────────────────────────────
            // Format: 12_Jun_2026
            string dateFolder = DateTime.Now.ToString("dd_MMM_yyyy");  // e.g. "12_Jun_2026"

            string baseDir = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "ECW_EHR_PatientRecords",
                dateFolder);

            Directory.CreateDirectory(baseDir);
            string patientNameSafe = "Unknown";
            if (!string.IsNullOrWhiteSpace(note.PatientName))
            {
                var nameParts = note.PatientName.Trim().Split(
                    ' ', StringSplitOptions.RemoveEmptyEntries);
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string lastName = nameParts.Length > 1 ? nameParts[^1] : firstName;
                string combined = $"{lastName}_{firstName}";
                // Remove any invalid file name characters
                patientNameSafe = string.Concat(
                    combined.Split(Path.GetInvalidFileNameChars()));
            }

            // Date format: DOS as yyyyMMdd  e.g. "2026-06-07" → "20260607"
            // Use DOS (convert_dos) not DOB — matches your example HernandezJennifer_20260607
            // Use DateOfBirth formatted as yyyyMMdd  e.g. "08/09/1974" → "19740809"
            string dosFormatted = "NA";
            if (!string.IsNullOrWhiteSpace(note.DateOfBirth))
            {
                if (DateTime.TryParse(note.DateOfBirth, out var dobDate))
                    dosFormatted = dobDate.ToString("yyyyMMdd");
                else
                    dosFormatted = note.DateOfBirth.Replace("-", "").Replace("/", "");
            }

            // Final: HernandezJennifer_20260607.pdf
            string fileName = $"{patientNameSafe}_{dosFormatted}_NA.pdf";

            string fullPath = Path.Combine(baseDir, fileName);

            // ── 3. If file already exists add a counter ───────────────────
            // Prevents overwrite if same patient processed twice in one day
            if (System.IO.File.Exists(fullPath))
            {
                int counter = 1;
                string nameWithoutExt = $"{patientNameSafe}_{dosFormatted}_NA";
                do
                {
                    fileName = $"{nameWithoutExt}_{counter}.pdf";
                    fullPath = Path.Combine(baseDir, fileName);
                    counter++;
                }
                while (System.IO.File.Exists(fullPath));
            }

            System.IO.File.WriteAllBytes(fullPath, pdfBytes);
            return fullPath;
        }


        //Added on 12-06-26
        [HttpGet("GetECWPatientRecord")]
        public async Task<IActionResult> GetECWPatientRecord(string name = null, DateTime? dos = null)
        {
            if (name == null) return Ok("Enter the name");

            var convert_dos = dos?.ToString("yyyy-MM-dd");
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (!System.IO.File.Exists(filepath))
                return Ok("file not found");

            // ── TOKEN: one line replaces the old per-request token call ──
            var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(System.IO.File.ReadAllText(filepath));
            ECWTokenManager.Instance.SetConfig(jsonser);
            var bearer = await ECWTokenManager.Instance.GetTokenAsync();


            var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name);
            var fhir_id = get_patient?.FirstOrDefault()?.fhir_id ?? "";

            var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);
            var dosEncounter = get_encounter_json?.FirstOrDefault(e => e.dos == convert_dos) ?? get_encounter_json?.OrderByDescending(e => e.dos).FirstOrDefault();
            var encounter_id = dosEncounter?.encounterid ?? "";

            Console.WriteLine($"[Controller] encounter_id='{encounter_id}' date='{dosEncounter?.dos}'");

            var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);

            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
            var binaryId = get_binary_data?.binaryid ?? "";

            if (string.IsNullOrEmpty(binaryId))
                return NotFound("Full CCDA DocumentReference not found for this patient");

            var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(
                bearer, binaryId, name, convert_dos);

            if (string.IsNullOrWhiteSpace(get_CCDA_binary))
                return NotFound("CCDA XML is empty");

            var note = CcdaParserNew.Parse(get_CCDA_binary, convert_dos);
            if (!string.IsNullOrEmpty(convert_dos))
                note.DateOfService = convert_dos;

            // CC injection
            string ccText = "", rosText = "";
            if (get_docencounterwithpatient_json?.encounterxmldata != null)
            {
                var (ccFromXml, rosFromXml) = FhirCcRosHelper.ExtractCcRosFromEncounterXml(
                    get_docencounterwithpatient_json.encounterxmldata);
                ccText = ccFromXml ?? "";
                rosText = rosFromXml ?? "";
            }
            if (string.IsNullOrEmpty(ccText) && !string.IsNullOrEmpty(encounter_id))
            {
                // Token may have been renewed by now — always get fresh reference
                bearer = await ECWTokenManager.Instance.GetTokenAsync();
                ccText = await FhirApiCaller.GetCcFromEncounterResource(bearer, encounter_id);
            }
            if (!string.IsNullOrEmpty(ccText) &&
                !note.Sections.ContainsKey(CcdaParserNew.SEC_CC))
                note.Sections[CcdaParserNew.SEC_CC] = new List<string> { ccText };
            if (!string.IsNullOrEmpty(rosText) &&
                !note.Sections.ContainsKey(CcdaParserNew.SEC_ROS))
                note.Sections[CcdaParserNew.SEC_ROS] = new List<string> { rosText };

            foreach (var kvp in note.Sections)
                Console.WriteLine($"SECTION: {kvp.Key} = {kvp.Value.Count} rows");

            byte[] fileBytes = ClinicalPdfGeneratorNew.Generate(note);
            return File(fileBytes, "application/pdf", $"{name}_CCDA.pdf");
        }


        [HttpPost("ProcessBatch")]
        public async Task<IActionResult> ProcessBatch([FromBody] BatchRequest req)
        {
            if (string.IsNullOrEmpty(req?.FileName))
                return BadRequest("fileName is required");

            var excelPath = Path.Combine(_webHostEnvironment.WebRootPath, "files", req.FileName);

            if (!System.IO.File.Exists(excelPath))
                return NotFound($"Excel file not found: {req.FileName}");

            // ── Step 1: Init token ONCE before batch starts ────────
            var credPath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");
            var config = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(System.IO.File.ReadAllText(credPath));

            ECWTokenManager.Instance.SetConfig(config);
            var initialToken = await ECWTokenManager.Instance.GetTokenAsync();
            Console.WriteLine(
                $"[Batch] Token ready. Expires at " +
                $"{ECWTokenManager.Instance.ExpiresAt.ToLocalTime():HH:mm:ss}. " +
                $"Valid for {ECWTokenManager.Instance.TimeUntilExpiry.TotalMinutes:F0} min");

            // ── Step 2: Read Excel ─────────────────────────────────
            var patients = ReadExcel(excelPath);
            Console.WriteLine($"[Batch] {patients.Count} patients to process");

            // ── Step 3: Process with controlled concurrency ────────
            // 5 at a time — ECW rate limit safe
            var semaphore = new SemaphoreSlim(5);
            var results = new List<BatchResult>();
            var resultLock = new object();

            var tasks = patients.Select(async patient =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var result = await ProcessOnePatient(patient);
                    lock (resultLock) results.Add(result);
                    Console.WriteLine(
                        $"[Batch] {(result.Success ? "✓" : "✗")} " +
                        $"{patient.Name} {patient.DOS} " +
                        $"[Token expires in " +
                        $"{ECWTokenManager.Instance.TimeUntilExpiry.TotalMinutes:F0}min]");
                }
                finally { semaphore.Release(); }
            });

            await Task.WhenAll(tasks);

            var summary = new
            {
                Total = results.Count,
                Succeeded = results.Count(r => r.Success),
                Failed = results.Count(r => !r.Success),
                Results = results.OrderBy(r => r.PatientName).ToList()
            };

            Console.WriteLine(
                $"[Batch] Done — {summary.Succeeded}/{summary.Total} succeeded");

            return Ok(summary);
        }

        // ── Process a single patient (used by both batch and can be reused) ──
        private async Task<BatchResult> ProcessOnePatient(PatientRow patient)
        {
            try
            {
                // Always call GetTokenAsync() — it returns cached token
                // or silently renews if near expiry. Zero overhead if valid.
                var bearer = await ECWTokenManager.Instance.GetTokenAsync();

                // Patient lookup
                var patients = await FhirApiCaller.CallApiforPatientDemo(
                    bearer, patient.Name);
                var fhir_id = patients?.FirstOrDefault()?.fhir_id ?? "";
                if (string.IsNullOrEmpty(fhir_id))
                    return Fail(patient, "Patient not found in ECW");

                // Encounter
                bearer = await ECWTokenManager.Instance.GetTokenAsync();
                var encounters = await FhirApiCaller.EncounterAPI(
                    bearer, fhir_id, patient.DOS);
                var enc = encounters?
                    .FirstOrDefault(e => e.dos == patient.DOS)
                    ?? encounters?.OrderByDescending(e => e.dos).FirstOrDefault();
                var encounter_id = enc?.encounterid ?? "";

                // Encounter note XML
                bearer = await ECWTokenManager.Instance.GetTokenAsync();
                var docRef = await FhirApiCaller
                    .CallApiForDocrefreshEncounterwithPatient(
                        bearer, fhir_id, encounter_id);

                // CCDA binary
                bearer = await ECWTokenManager.Instance.GetTokenAsync();
                var binary = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);
                var binaryId = binary?.binaryid ?? "";
                if (string.IsNullOrEmpty(binaryId))
                    return Fail(patient, "CCDA DocumentReference not found");

                bearer = await ECWTokenManager.Instance.GetTokenAsync();
                var ccdaXml = await FhirApiCaller.GetCcdaXml(
                    bearer, binaryId, patient.Name, patient.DOS);
                if (string.IsNullOrWhiteSpace(ccdaXml))
                    return Fail(patient, "CCDA XML is empty");

                // Parse
                var note = CcdaParserNew.Parse(ccdaXml, patient.DOS);
                note.DateOfService = patient.DOS;

                // CC
                string ccText = "";
                if (docRef?.encounterxmldata != null)
                {
                    var (ccFromXml, _) = FhirCcRosHelper.ExtractCcRosFromEncounterXml(
                        docRef.encounterxmldata);
                    ccText = ccFromXml ?? "";
                }
                if (string.IsNullOrEmpty(ccText) && !string.IsNullOrEmpty(encounter_id))
                {
                    bearer = await ECWTokenManager.Instance.GetTokenAsync();
                    ccText = await FhirApiCaller.GetCcFromEncounterResource(
                        bearer, encounter_id);
                }
                if (!string.IsNullOrEmpty(ccText) &&
                    !note.Sections.ContainsKey(CcdaParserNew.SEC_CC))
                    note.Sections[CcdaParserNew.SEC_CC] = new List<string> { ccText };

                // Generate PDF
                var pdfBytes = ClinicalPdfGeneratorNew.Generate(note);

                // Save to output folder
                var outDir = Path.Combine(
                    _webHostEnvironment.WebRootPath, "output");
                Directory.CreateDirectory(outDir);

                var safeName = string.Concat(
                    patient.Name.Split(Path.GetInvalidFileNameChars()));
                var outPath = Path.Combine(outDir,
                    $"{safeName}_{patient.DOS.Replace("-", "")}.pdf");

                await System.IO.File.WriteAllBytesAsync(outPath, pdfBytes);

                return new BatchResult
                {
                    PatientName = patient.Name,
                    DOS = patient.DOS,
                    Success = true,
                    PdfPath = $"/output/{Path.GetFileName(outPath)}"
                };
            }
            catch (Exception ex)
            {
                return Fail(patient, ex.Message);
            }
        }

        // ── Excel reader ────────────────────────────────────────────
        // Expected columns:  A = Patient Name   B = Date (any format)
        // Row 1 = header (skipped automatically)
        private List<PatientRow> ReadExcel(string path)
        {
            var rows = new List<PatientRow>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);

            foreach (var row in ws.RowsUsed().Skip(1))  // skip header
            {
                string name = row.Cell(1).GetString().Trim();
                string dos = row.Cell(2).GetString().Trim();
                if (string.IsNullOrEmpty(name)) continue;

                // Normalise date to yyyy-MM-dd regardless of Excel format
                if (DateTime.TryParse(dos, out var d))
                    dos = d.ToString("yyyy-MM-dd");

                rows.Add(new PatientRow { Name = name, DOS = dos });
            }

            return rows;
        }

        private static BatchResult Fail(PatientRow p, string error) =>
            new() { PatientName = p.Name, DOS = p.DOS, Success = false, Error = error };


        //[HttpGet("GetRecordNewFinal")]
        //public async Task<IActionResult> GetRecordNewFinal(string name, string convert_dos)
        //{
        //    var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

        //    if (name == null)
        //    {
        //        return Ok("Enter the name");
        //    }

        //    //var convert_dos = dos?.ToString("yyyy-MM-dd");

        //    if (!System.IO.File.Exists(filepath))
        //    {
        //        return Ok("file not found");
        //    }

        //    var json_data = System.IO.File.ReadAllText(filepath);
        //    var jsonser = System.Text.Json.JsonSerializer.Deserialize<ECWConfig>(json_data);
        //    var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);

        //    var get_patient = await FhirApiCaller.CallApiforPatientDemo(bearer, name);
        //    //var fhir_id = get_patient?.id ?? "";

        //    //if (string.IsNullOrEmpty(fhir_id))
        //    //    return NotFound("Patient not found");

        //    var fhir_id = "";
        //    foreach (var item in get_patient)
        //    {
        //        fhir_id = item.fhir_id;
        //    }

        //    var get_encounter_json = await FhirApiCaller.EncounterAPI(bearer, fhir_id, convert_dos);


        //    var dosEncounter = get_encounter_json?
        //        .FirstOrDefault(e => e.dos == convert_dos)   // exact DOS match
        //        ?? get_encounter_json?
        //        .OrderByDescending(e => e.dos)               // fallback: most recent
        //        .FirstOrDefault();

        //    var encounter_id = dosEncounter?.encounterid ?? "";


        //    var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);

        //    string ccFromEncounter = "";
        //    string rosFromEncounter = "";

        //    if (get_docencounterwithpatient_json != null)
        //    {
        //        // Extract CC and ROS from the encounter note XML
        //        var (cc, ros) = FhirCcRosHelper.ExtractCcRosFromEncounterXml(
        //            get_docencounterwithpatient_json.encounterxmldata);
        //        ccFromEncounter = cc;
        //        rosFromEncounter = ros;
        //    }


        //    var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);


        //    var binaryId = get_binary_data?.binaryid ?? "";

        //    if (string.IsNullOrEmpty(binaryId))
        //        return NotFound("Full CCDA DocumentReference not found for this patient");


        //    var get_CCDA_binary = await FhirApiCaller.GetCcdaXml(bearer, binaryId, name, convert_dos);
        //    // URL: /Binary/{binaryId}   ← CORRECT

        //    if (string.IsNullOrEmpty(get_CCDA_binary))
        //        return NotFound("CCDA binary content is empty");

        //    //Parse CCDA and inject CC + ROS ───────────────────────
        //    var note = CcdaParserNew.Parse(get_CCDA_binary, convert_dos);

        //    // Override DateOfService with what was passed in
        //    if (!string.IsNullOrEmpty(convert_dos))
        //        note.DateOfService = convert_dos;

        //    // Inject CC and ROS from encounter note (not in CCDA)
        //    if (!string.IsNullOrEmpty(ccFromEncounter))
        //        note.Sections[CcdaParserNew.SEC_CC] = new List<string> { ccFromEncounter };

        //    if (!string.IsNullOrEmpty(rosFromEncounter))
        //        note.Sections[CcdaParserNew.SEC_ROS] = new List<string> { rosFromEncounter };

        //    // Generate PDF
        //    byte[] fileBytes = ClinicalPdfGeneratorNew.Generate(note);

        //    var fileName = $"{name?.Replace(" ", "_")}_{convert_dos}_CCDA.pdf";
        //    return File(fileBytes, "application/pdf", fileName);
        //}

    }
}
