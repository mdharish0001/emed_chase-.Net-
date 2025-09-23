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

        [HttpPost]
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
                    "Patient Name",
                    "RenderingProviderName",

                };

            List<string> headersForCPT = new List<string>
                {
                    "CPT Code",
                    "Procedure code",
                    "CPT-CODE",

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
                        model.practice = "RB HEART AND VASCULAR CARE PLLC DBA - EPIC HEART AND VASCULAR CARE";

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

                        model.created_on = DateTime.Now;

                        list.Add(model);

                   

                }
                await _charge_captureService.Create(list);
            }
                return Ok("Testing");
        }

        //[NonAction]

        //public static async string excel_read()
        //{

        //    string filePath = @"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\DR RISHI CHARGE REPORT FROM 910 TO 0918.xlsx";

        //    List<charge_capture> headersToFind = new List<charge_capture>();

        //    var list = new List<charge_capture>();

        //    using (var package = new ExcelPackage(new FileInfo(filePath)))
        //    {
        //        var worksheets = package.Workbook.Worksheets;
        //        if (worksheets.Count == 0)
        //        {
        //            throw new Exception("Excel file has no worksheets.");
        //        }

        //        // Access the first worksheet
        //        var worksheet = worksheets[1];
        //        int totalColumns = worksheet.Dimension.End.Column;
        //        int totalRows = worksheet.Dimension.End.Row;

        //        // Dictionary to hold header and corresponding column index
        //        Dictionary<string, int> headerColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        //        // Find the columns for each header
        //        for (int col = 1; col <= totalColumns; col++)
        //        {
        //            string ? header = worksheet.Cells[1, col].Text.Trim();

        //            if (headersToFind.Contains(header))
        //            {
        //                headerColumns[header] = col;
        //            }
        //        }

        //        // Check if any headers were not found
        //        foreach (var header in headersToFind)
        //        {
        //            if (!headerColumns.ContainsKey(header))
        //            {
        //                Console.WriteLine($"Warning: Header '{header}' not found in Excel.");
        //            }
        //        }

        //        // Read data rows
        //        for (int row = 2; row <= totalRows; row++)
        //        {
        //            Console.WriteLine($"Row {row}:");

        //            foreach (var header in headersToFind)
        //            {
        //                if (headerColumns.TryGetValue(header, out int colIndex))
        //                {
        //                    string cellValue = worksheet.Cells[row, colIndex].Text;
        //                    Console.WriteLine($"  {header}: {cellValue}");

                            
        //                }
        //                else
        //                {
        //                    Console.WriteLine($"  {header}: Not Found");
        //                }
                       
        //            }
        //            var model = new charge_capture();

        //            if (headerColumns.TryGetValue("Patient Name", out int nameCol))
        //            {
        //                model.patinet_name = worksheet.Cells[row, nameCol].Text; // or .Value?.ToString()
        //            }

        //            if (headerColumns.TryGetValue("Patient Acct No", out int idCol))
        //            {
        //                string idText = worksheet.Cells[row, idCol].Text;
        //                if (int.TryParse(idText, out int id))
        //                {
        //                    model.patient_id = id;
        //                }
        //                else
        //                {
        //                    // Handle parsing error
        //                    model.patient_id = 0; // or throw, or log error
        //                }
        //            }
        //            if (headerColumns.TryGetValue("Service Date", out int snameCol))
        //            {
        //                string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy" };


        //                DateTime dob;

        //                if (DateTime.TryParseExact(worksheet.Cells[row, snameCol].Text, formats,
        //                                              CultureInfo.InvariantCulture,
        //                                              DateTimeStyles.None,
        //                                              out dob))
        //                {
        //                    model.dos = dob;
        //                }
                       
        //            }
        //            model.created_on=DateTime.Now;
                    

        //            list.Add(model);
                    

        //            Console.WriteLine();
        //        }
        //    }

        //    return "success";
        //}


        //public class ExcelReader
        //{
        //    private static object _charge_CaptureService;

        //    public static List<charge_capture> ReadChargesFromExcel(string filePath)
        //    {
        //        // Define the headers you expect in your Excel file
        //        List<string> headersToFind = new List<string>
        //{
        //    "Resource Provider Name",
        //    "Patient Name",
        //    "Patient Acct No",
        //    "Service Date",
        //    "CPT Code",
        //    "Claim No",
        //    "Claim Date"
           
        //};

        //        string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy" };


        //        DateTime dob;
        //        var list = new List<charge_capture>();

        //        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set EPPlus license context

        //        using (var package = new ExcelPackage(new FileInfo(filePath)))
        //        {
        //            var worksheets = package.Workbook.Worksheets;
        //            if (worksheets.Count == 0)
        //            {
        //                throw new Exception("Excel file has no worksheets.");
        //            }

        //            var worksheet = worksheets[1];  // 1-based index

        //            int totalColumns = worksheet.Dimension.End.Column;
        //            int totalRows = worksheet.Dimension.End.Row;

        //            // Map header to column index
        //            Dictionary<string, int> headerColumns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        //            for (int col = 1; col <= totalColumns; col++)
        //            {
        //                string header = worksheet.Cells[1, col].Text.Trim();
        //                if (headersToFind.Contains(header))
        //                {
        //                    headerColumns[header] = col;
        //                }
        //            }

        //            // Warn if any header not found
        //            foreach (var header in headersToFind)
        //            {
        //                if (!headerColumns.ContainsKey(header))
        //                {
        //                    Console.WriteLine($"Warning: Header '{header}' not found in Excel.");
        //                }
        //            }

        //            // Read data rows
        //            for (int row = 2; row <= totalRows; row++)
        //            {
        //                var model = new charge_capture();

        //                //if (headerColumns.TryGetValue("Practice", out int practiceCol))
        //                //    model.practice = worksheet.Cells[row, practiceCol].Text;
        //                model.practice = "RB HEART AND VASCULAR CARE PLLC DBA - EPIC HEART AND VASCULAR CARE";

        //                if (headerColumns.TryGetValue("Resource Provider Name", out int providerCol))
        //                    model.provider = worksheet.Cells[row, providerCol].Text;

        //                if (headerColumns.TryGetValue("Patient Name", out int nameCol))
        //                    model.patinet_name = worksheet.Cells[row, nameCol].Text;

        //                if (headerColumns.TryGetValue("Patient Acct No", out int patientIdCol))
        //                {
        //                    string idText = worksheet.Cells[row, patientIdCol].Text;
        //                    model.patient_id = int.TryParse(idText, out int id) ? id : 0;
        //                }

        //                if (headerColumns.TryGetValue("Service Date", out int dosCol))
        //                    //model.dos = ParseDate(worksheet.Cells[row, dosCol].Text);
        //                    if (DateTime.TryParseExact(worksheet.Cells[row, dosCol].Text, formats,
        //                                              CultureInfo.InvariantCulture,
        //                                              DateTimeStyles.None,
        //                                              out dob))
        //                    {
        //                        model.dos = dob;
        //                    }

        //                if (headerColumns.TryGetValue("CPT Code", out int cptCol))
        //                    model.cpt = worksheet.Cells[row, cptCol].Text;

        //                if (headerColumns.TryGetValue("Claim No", out int claimIdCol))
        //                {
        //                    string claimIdText = worksheet.Cells[row, claimIdCol].Text;
        //                    model.claim_id = int.TryParse(claimIdText, out int claimId) ? claimId : 0;
        //                    model.encounter_id = int.TryParse(claimIdText, out int EntId) ? EntId : 0;
        //                }

        //                if (headerColumns.TryGetValue("Claim Date", out int claimDateCol))
        //                    if (DateTime.TryParseExact(worksheet.Cells[row, claimDateCol].Text, formats,
        //                                               CultureInfo.InvariantCulture,
        //                                               DateTimeStyles.None,
        //                                               out dob))
        //                    {
        //                        model.claim_date = dob;
        //                    }
                      

        //                //if (headerColumns.TryGetValue("Encounter ID", out int encounterIdCol))
        //                //{
        //                //    string encIdText = worksheet.Cells[row, encounterIdCol].Text;
        //                //    model.encounter_id = int.TryParse(encIdText, out int encId) ? encId : 0;
        //                //}

        //                //if (headerColumns.TryGetValue("IsDelete", out int isDeleteCol))
        //                //{
        //                //    string isDelText = worksheet.Cells[row, isDeleteCol].Text;
        //                //    model.isdelete = bool.TryParse(isDelText, out bool isDel) ? isDel : false;
        //                //}

        //                model.created_on = DateTime.Now;

        //                list.Add(model);

                        
        //            }
        //            await _charge_CaptureService.Create(list);
        //        }

        //        return list;
        //    }

        //    private static DateTime? ParseDate(string text)
        //    {
        //        if (string.IsNullOrWhiteSpace(text))
        //            return null;

        //        string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };

        //        if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
        //            return dt;

        //        if (DateTime.TryParse(text, out dt))
        //            return dt;

        //        return null; // Could not parse date
        //    }
        //}
    }
}
