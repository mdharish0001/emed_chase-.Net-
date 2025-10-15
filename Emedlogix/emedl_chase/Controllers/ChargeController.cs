using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using emedl_chase.DbModel;
using emedl_chase.Model;
using emedl_chase.Option;
using emedl_chase.Service;
using emedl_chase.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using static emedl_chase.Model.ChargeViewModel;
using static emedl_chase.Model.Patient;

namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChargeController : ControllerBase
    {
        private readonly charge_captureService _charge_captureService;
        private readonly ApplicationConfig _applicationConfig;

        private readonly OrganizationService _organizationService;

        public ChargeController(charge_captureService charge_CaptureService,OrganizationService organization, IOptions<ApplicationConfig> applicationConfig)
        {
            _charge_captureService = charge_CaptureService;
            _organizationService = organization;
            _applicationConfig = applicationConfig.Value;
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
                            model.patient_id = worksheet.Cells[row, patientIdCol].Text;
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

            //string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            //var filePath = Path.Combine(uploadPath, Path.GetFileName(get_filename));


            var fileName = Path.Combine(WBCGlobal.ChargeFiles, System.DateTime.Now.ToString("dd_MMM_yyyy"), get_filename);
            string entryDestination = Path.Combine(_applicationConfig.AppBasePath, fileName);

            // Create directories if they don't exist
            Directory.CreateDirectory(Path.GetDirectoryName(entryDestination));

            //  string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            //var filePath = Path.Combine(uploadPath, Path.GetFileName(get_filename));

            using (var entryStream = oViewmodel.file.OpenReadStream())
            using (var destination = System.IO.File.Create(entryDestination))
            {
                await entryStream.CopyToAsync(destination);
            }



            // Save uploaded file to disk
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await get_file.CopyToAsync(stream);
            //}

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

            string[] formats = { "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss","dd/MM/yyyy","dd/MM/yyyy HH:mm:ss" };

            var list = new List<charge_capture>();
            var extlist = new List<charge_capture>();
            DateTime parsedDate;

            using (var package = new ExcelPackage(new FileInfo(entryDestination)))
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
                        model.patient_id = worksheet.Cells[row, patientIdCol].Text;
                    }

                    // Service Date
                    if (TryGetColumn(headerColumns, headersForServiceDate, out int dosCol))
                    {
                        string dateText = worksheet.Cells[row, dosCol].Value.ToString();
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
                        string dateText2 = worksheet.Cells[row, claimDateCol].Value.ToString();
                        if (DateTime.TryParseExact(dateText2, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                            model.claim_date = parsedDate;
                    }

                    if (TryGetColumn(headerColumns, headersForLocation, out int LocCol))
                        model.location = worksheet.Cells[row, LocCol].Text;

                    if (model.encounter_id == null)
                    {
                        if (new[] { 1, 5, 21,17, 22 }.Contains(model.org_id))
                        {
                            model.encounter_id = model.claim_id;
                        }
                    }
                    var match = _charge_captureService.GetAll(patient_id: model.patient_id, dos: model.dos, encounter_id:model.encounter_id,cpt:model.cpt,org_id:get_org_id,claim_id:model.claim_id,patientname:model.patient_name).ToList() ;

                    if (match.Count==0)
                    {
                        list.Add(model);
                    }
                    
                    else
                    {
                        extlist.Add(model);
                    }
                }

                await _charge_captureService.Create(list);
                var response = new
                {
                    TotalRecords = totalRows,
                    Filename = get_filename,
                    Provider = get_type,
                    TotalData= list.Count,
                    ExistingData=extlist.Count
                };
            return Ok(response);

            }

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

        [HttpGet("getrecords")]

        public IActionResult Get(string? patient = null,int? org_id= null,int page=1,int pageSize=10)
        {
            var data = _charge_captureService.GetAll(patient_id: patient,org_id:org_id).ToList();

            var get_cpt = data.GroupBy(a => a.claim_id).ToList();
            Console.WriteLine(get_cpt.Count);

            var patient_list=new List<ChargeModel>();

            foreach(var c in data)
            {
                var model =new ChargeModel();
                model.Id = c.id;
                model.cpt_code = c.cpt;
                model.claim_id = c.claim_id;
                model.patient_name = c.patient_name;
                model.practice = c.practice;
                model.encounter_id = c.encounter_id!=null ? c.encounter_id : c.claim_id;
                model.dos = c.dos;
                model.claim_date=c.claim_date;

                patient_list.Add(model);

            }
            var totalCount = patient_list.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedData = patient_list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!pagedData.Any())
            {
                return NotFound("No data found for the given filter.");
            }
            var response = new
            {
                TotalRecords = patient_list.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = pagedData
            };
            return Ok (response);
        }


        [HttpGet("getpatientname")]
        public IActionResult GetPateint(string? patient_name= null,int ?org_id = null, int page = 1, int pageSize = 10)
        {
            var data = _charge_captureService.GetAll(patientname: patient_name, org_id: org_id).ToList();

            var get_cpt = data.GroupBy(a => a.claim_id).ToList();
            Console.WriteLine(get_cpt.Count);

            var patient_list = new List<ChargeModel>();

            foreach (var c in data)
            {
                var model = new ChargeModel();
                model.Id = c.id;
                model.patient_id = c.patient_id;
                model.cpt_code = c.cpt;
                model.claim_id = c.claim_id;
                model.patient_name = c.patient_name;
                model.practice = c.practice;
                model.encounter_id = c.encounter_id != null ? c.encounter_id : c.claim_id;
                model.dos = c.dos;
                model.claim_date = c.claim_date;

                patient_list.Add(model);

            }
            var totalCount = patient_list.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedData = patient_list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!pagedData.Any())
            {
                return NotFound("No data found for the given filter.");
            }
            var response = new
            {
                TotalRecords = patient_list.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Data = pagedData
            };
            return Ok(response);

        }
    }
}
