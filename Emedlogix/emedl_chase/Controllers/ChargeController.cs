using emedl_chase.DbModel;
using emedl_chase.Model;
using emedl_chase.Service;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using static emedl_chase.Model.ChargeViewModel;
using static emedl_chase.Model.Patient;

namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargeController : ControllerBase
    {
        private readonly charge_captureService _charge_captureService;


        private readonly OrganizationService _organizationService;

        public ChargeController(charge_captureService charge_CaptureService,OrganizationService organization)
        {
            _charge_captureService = charge_CaptureService;
            _organizationService = organization;
        }

        [HttpPost("test")]
        public async Task<IActionResult> Index([FromForm] ClientUpload oViewmodel)
        {

            //var call_funct=excel_read();]

            if (oViewmodel==null)
                {
                    return Ok("invalid");
                }

           var  get_filename=oViewmodel.file.FileName;
           var  get_file=oViewmodel.file;
           var get_type=oViewmodel.type;


            var get_org_id=_organizationService.GetAll(name:oViewmodel.type).Select(x => x.Id).FirstOrDefault();

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var filePath = Path.Combine(uploadPath, Path.GetFileName(oViewmodel.file.FileName));

            //string filePath = @"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\DR RISHI CHARGE REPORT.xlsx";
            // Define the headers you expect in your Excel file
            List<string> headersToFind = new List<string>
                {
                    "Resource Provider Name",
                    "Patient Name",
                    "Patient Acct No",
                    "Service Date",
                    "CPT Code",
                    "Claim No",
                    "Claim Date",
                    "PatientName",
                    "PatientID",
                    "EncounterID",
                    "ServiceStartDate",
                    "RenderingProviderName",
                    "ProcedureCode",

                };

            List<string> headersForProviderName = new List<string>
                {
                    "Resource Provider Name",
              
                    "RenderingProviderName",

                };

            List<string> headersForCPT = new List<string>
                {
                    "CPT Code",
                    "Procedure code"
                    
                };

            List<string> headersForPatient = new List<string>
                {
                    "Patient Name",
                     "PatientName"
                };

            List<string> headersForPatientID = new List<string>
                {
                    "PatientID",
                     "Patient Acct No"
                };
             List<string> headersForService = new List<string>
                {
                      "Service Date",
                       "ServiceStartDate"
                };
                List<string> headersForCalim = new List<string>
                {
                      "Claim No",
                       "ID"
                };



            string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy", "MMM dd, yyyy","dd-MM-yyyy" };


                DateTime dob;
                var list = new List<charge_capture>();

                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set EPPlus license context

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheets = package.Workbook.Worksheets;
                    if (worksheets.Count == 0)
                    {
                        throw new Exception("Excel file has no worksheets.");
                    }

                    var worksheet = worksheets[1];  // 1-based index

                    int totalColumns = worksheet.Dimension.End.Column;
                    int totalRows = worksheet.Dimension.End.Row;

                    // Map header to column index
                    Dictionary<string, int> headerColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                    for (int col = 1; col <= totalColumns; col++)
                    {
                        string header = worksheet.Cells[1, col].Text.Trim();
                        if (headersToFind.Contains(header))
                        {
                            headerColumns[header] = col;
                        }
                    }

                    // Warn if any header not found
                    foreach (var header in headersToFind)
                    {
                        if (!headerColumns.ContainsKey(header))
                        {
                            Console.WriteLine($"Warning: Header '{header}' not found in Excel.");
                        }
                    }

                    // Read data rows
                    for (int row = 2; row <= totalRows; row++)
                    {
                        var model = new charge_capture();

                        //if (headerColumns.TryGetValue("Practice", out int practiceCol))
                        //    model.practice = worksheet.Cells[row, practiceCol].Text;
                        model.practice = get_type;
                        model.file_name = get_filename;
                        model.state = "test";
                        model.created_on = DateTime.Now;

                    if (headerColumns.TryGetValue("Resource Provider Name", out int providerCol))
                        model.provider = worksheet.Cells[row, providerCol].Text;

                    if (headerColumns.TryGetValue("Patient Name", out int nameCol))
                            model.patient_name = worksheet.Cells[row, nameCol].Text;

                        if (headerColumns.TryGetValue("Patient Acct No", out int patientIdCol))
                        {
                            string idText = worksheet.Cells[row, patientIdCol].Text;
                            model.patient_id = int.TryParse(idText, out int id) ? id : 0;
                        }

                        if (headerColumns.TryGetValue("Service Date", out int dosCol))
                            //model.dos = ParseDate(worksheet.Cells[row, dosCol].Text);
                            if (DateTime.TryParseExact(worksheet.Cells[row, dosCol].Text, formats,
                                                      CultureInfo.InvariantCulture,
                                                      DateTimeStyles.None,
                                                      out dob))
                            {
                                model.dos = dob;
                            }

                        if (headerColumns.TryGetValue("CPT Code", out int cptCol))
                            model.cpt = worksheet.Cells[row, cptCol].Text;

                        if (headerColumns.TryGetValue("Claim No", out int claimIdCol))
                        {
                            string claimIdText = worksheet.Cells[row, claimIdCol].Text;
                            model.claim_id = int.TryParse(claimIdText, out int claimId) ? claimId : 0;
                            model.encounter_id = int.TryParse(claimIdText, out int EntId) ? EntId : 0;
                        }

                        if (headerColumns.TryGetValue("Claim Date", out int claimDateCol))
                            if (DateTime.TryParseExact(worksheet.Cells[row, claimDateCol].Text, formats,
                                                       CultureInfo.InvariantCulture,
                                                       DateTimeStyles.None,
                                                       out dob))
                            {
                                model.claim_date = dob;
                            }


                        //if (headerColumns.TryGetValue("Encounter ID", out int encounterIdCol))
                        //{
                        //    string encIdText = worksheet.Cells[row, encounterIdCol].Text;
                        //    model.encounter_id = int.TryParse(encIdText, out int encId) ? encId : 0;
                        //}

                        //if (headerColumns.TryGetValue("IsDelete", out int isDeleteCol))
                        //{
                        //    string isDelText = worksheet.Cells[row, isDeleteCol].Text;
                        //    model.isdelete = bool.TryParse(isDelText, out bool isDel) ? isDel : false;
                        //}

                       

                        list.Add(model);

                   

                }
                await _charge_captureService.Create(list);
            }
                return Ok("Testing");
        }



        [HttpPost("ClaimfileUpload")]
        public async Task<IActionResult> ExcelUpload([FromForm] ClientUpload oViewmodel)
        {
            if (oViewmodel == null)
                return Ok("Invalid input.");

            var get_filename = oViewmodel.file.FileName;
            var get_file = oViewmodel.file;
            var get_type = oViewmodel.type;

            var get_org_id = _organizationService.GetAll(name: oViewmodel.type)
                                                 .Select(x => x.Id)
                                                 .FirstOrDefault();

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(uploadPath, Path.GetFileName(get_filename));

            // Save uploaded file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await get_file.CopyToAsync(stream);
            }

            // Header matching lists
            List<string> headersToFind = new List<string>
    {
        "Rendering Provider Name", "Patient Name", "Patient Acct No", "Service Date",
        "CPT Code", "Claim No", "Claim Date", "PatientName", "PatientID", "EncounterID",
        "ServiceStartDate", "RenderingProviderName", "ProcedureCode", "Procedure code", "CPT-CODE", "ID","CreatedDate","Facility Name","ServiceLocationName"
    };

            List<string> headersForProviderName = new List<string> { "Rendering Provider Name", "RenderingProviderName" };
            List<string> headersForCPT = new List<string> { "CPT Code", "Procedure code", "ProcedureCode", "CPT-CODE" };
            List<string> headersForPatient = new List<string> { "Patient Name", "PatientName" };
            List<string> headersForPatientID = new List<string> { "PatientID", "Patient Acct No" };
            List<string> headersForServiceDate = new List<string> { "Service Date", "ServiceStartDate" };
            List<string> headersForClaimNo = new List<string> { "Claim No", "ID" };
            List<string> headersForClaimDate = new List<string> { "Claim Date" , "CreatedDate" };
            List<string> headersForEncounter = new List<string> { "EncounterID" };
            List<string> headersForLocation = new List<string> { "Facility Name", "ServiceLocationName" };

            string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy", "MMM dd, yyyy", "dd-MM-yyyy" };

            var list = new List<charge_capture>();
            DateTime parsedDate;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheets = package.Workbook.Worksheets;
                if (worksheets.Count == 0)
                    return BadRequest("Excel file has no worksheets.");

                var worksheet = worksheets[1]; // 1-based index
                int totalColumns = worksheet.Dimension.End.Column;
                int totalRows = worksheet.Dimension.End.Row;

                // Map headers
                Dictionary<string, int> headerColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 1; col <= totalColumns; col++)
                {
                    string header = worksheet.Cells[1, col].Text.Trim();
                    if (headersToFind.Contains(header, StringComparer.OrdinalIgnoreCase))
                    {
                        headerColumns[header] = col;
                    }
                }

                // Warn if expected headers not found
                foreach (var header in headersToFind)
                {
                    if (!headerColumns.ContainsKey(header))
                        Console.WriteLine($"Warning: Header '{header}' not found.");
                }

                // Process data rows
                for (int row = 2; row <= totalRows; row++)
                {
                    var model = new charge_capture
                    {
                        practice = get_type,
                        file_name = get_filename,
                        state = "test",
                        created_on = DateTime.Now,
                        org_id= get_org_id

                    };

                    // Provider Name
                    if (TryGetColumn(headerColumns, headersForProviderName, out int providerCol))
                        model.provider = worksheet.Cells[row, providerCol].Text;

                    // Patient Name
                    if (TryGetColumn(headerColumns, headersForPatient, out int nameCol))
                        model.patient_name = worksheet.Cells[row, nameCol].Text;

                    // Patient ID
                    if (TryGetColumn(headerColumns, headersForPatientID, out int patientIdCol))
                    {
                        string idText = worksheet.Cells[row, patientIdCol].Text;
                        model.patient_id = int.TryParse(idText, out int pid) ? pid : 0;
                    }

                    // Service Date
                    if (TryGetColumn(headerColumns, headersForServiceDate, out int dosCol))
                    {
                        string dateText = worksheet.Cells[row, dosCol].Text;
                        if (DateTime.TryParseExact(dateText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            model.dos = parsedDate;
                    }

                    // CPT Code
                    if (TryGetColumn(headerColumns, headersForCPT, out int cptCol))
                        model.cpt = worksheet.Cells[row, cptCol].Text;

                    // Claim No / Encounter ID
                    if (TryGetColumn(headerColumns, headersForClaimNo, out int claimNoCol))
                    {
                        string claimIdText = worksheet.Cells[row, claimNoCol].Text;
                        model.claim_id = int.TryParse(claimIdText, out int cid) ? cid : 0;

                        //model.encounter_id = int.TryParse(claimIdText, out int eid) ? eid : cid; // Assuming same value
                    }

                    if (TryGetColumn(headerColumns, headersForEncounter, out int EntNoCol))
                    {
                        string EntIdText = worksheet.Cells[row, EntNoCol].Text;
                       
                        model.encounter_id = int.TryParse(EntIdText, out int eid) ? eid : 0; // Assuming same value
                    }

                    // Claim Date
                    if (TryGetColumn(headerColumns, headersForClaimDate, out int claimDateCol))
                    {
                        string dateText = worksheet.Cells[row, claimDateCol].Text;
                        if (DateTime.TryParseExact(dateText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            model.claim_date = parsedDate;
                    }

                    if (TryGetColumn(headerColumns, headersForLocation, out int LocCol))
                        model.location = worksheet.Cells[row, LocCol].Text;

                    list.Add(model);
                }

                await _charge_captureService.Create(list);
            }

            return Ok("Testing");
        }

        [NonAction]
        // 🔧 Helper Method for Matching Headers
        private bool TryGetColumn(Dictionary<string, int> headerColumns, List<string> possibleHeaders, out int colIndex)
        {
            foreach (var header in possibleHeaders)
            {
                if (headerColumns.TryGetValue(header, out colIndex))
                {
                    return true;
                }
            }
            colIndex = -1;
            return false;
        }



    }
}
