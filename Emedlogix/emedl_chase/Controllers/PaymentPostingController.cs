using System.Globalization;
using emedl_chase.DbModel;
using emedl_chase.Option;
using emedl_chase.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using static emedl_chase.Model.ChargeViewModel;

namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentPostingController : ControllerBase
    {
        private readonly charge_captureService _charge_captureService;
        private readonly ApplicationConfig _applicationConfig;
        public readonly IWebHostEnvironment _hostingEnvironment;

        private readonly OrganizationService _organizationService;
        private readonly payment_postService _payment_postService;

        public PaymentPostingController(charge_captureService charge_CaptureService, OrganizationService organization, IOptions<ApplicationConfig> applicationConfig, IWebHostEnvironment hostingEnvironment, payment_postService payment_PostService)
        {
            _charge_captureService = charge_CaptureService;
            _organizationService = organization;
            _applicationConfig = applicationConfig.Value;
            _hostingEnvironment = hostingEnvironment;
            _payment_postService = payment_PostService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpPost("payment-file")]
        public async Task<IActionResult> Index([FromForm] ClientUpload oViewmodel)
        {

            //var call_funct=excel_read();]

            if (oViewmodel == null)
            {
                return Ok("invalid");
            }

            var get_filename = oViewmodel.file.FileName;
            var get_file = oViewmodel.file;
            var get_type = oViewmodel.type;
            var get_source = oViewmodel.source;


            //var get_org_id = _organizationService.GetAll(name: oViewmodel.type).Select(x => x.Id).FirstOrDefault();

            var get_org_id = _organizationService.GetAll(name: get_type.Trim()).Select(x => x.Id).FirstOrDefault();
            var fileName = Path.Combine(WBCGlobal.PaymnetFiles, System.DateTime.Now.ToString("dd_MMM_yyyy"), get_type, get_filename);
            string entryDestination = Path.Combine(_hostingEnvironment.WebRootPath, fileName);

            // Create directories if they don't exist
            Directory.CreateDirectory(Path.GetDirectoryName(entryDestination));
            using (var entryStream = oViewmodel.file.OpenReadStream())
            using (var destination = System.IO.File.Create(entryDestination))
            {
                await entryStream.CopyToAsync(destination);
            }

            List<string> headersToFind = new List<string>
                {
                    "Provider",
                    "Patient Name",
                    "Patient Id",
                    "Dos",
                    "Cpt",
                    "Claim Id",
                    "Claim date",
                    "Encounter Id",
                    "Payment Method",
                    "Payment Date",
                    "Location/Facility",
                    "Paid Amount",
                    "Insurance",
                    "Username",
                    "Practice",
                    "SEPS care ID",
                    "Check#"

                };

          
            string[] formats = { "MM-dd-yyyy", "M-d-yyyy", "MM/dd/yyyy", "M/d/yyyy", "MMM dd, yyyy", "dd-MM-yyyy" };

            DateTime dob;
            var list = new List<payment_posting>();
            var extlist = new List<payment_posting>();
            var invalidlist = new List<payment_posting>();

            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set EPPlus license context

            using (var package = new ExcelPackage(new FileInfo(entryDestination)))
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
                var payment_post_list = _payment_postService.GetAllTable(org_id: get_org_id).ToList();
                for (int row = 2; row <= totalRows; row++)
                {
                    var model = new payment_posting
                    {
                        practice = get_type,
                        created_on = DateTime.UtcNow,
                        org_id = get_org_id,
                        source = get_source,
                        file_name = get_filename

                    };

                    //Provider

                    if (headerColumns.TryGetValue("Provider", out int providerCol))
                        model.provider = worksheet.Cells[row, providerCol].Text;
                    model.provider = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, providerCol].Text) && worksheet.Cells[row, providerCol].Text != "NULL") ? worksheet.Cells[row, providerCol].Text : null;

                    //Patient Name
                    if (headerColumns.TryGetValue("Patient Name", out int nameCol))
                        model.patient_name = worksheet.Cells[row, nameCol].Text;
                        model.patient_name = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, nameCol].Text) && worksheet.Cells[row, nameCol].Text != "NULL") ? worksheet.Cells[row, nameCol].Text : null;

                    //Patient Id
                    if (headerColumns.TryGetValue("Patient Id", out int patientIdCol))
                    {
                        //string idText = worksheet.Cells[row, patientIdCol].Text;
                        //model.patient_id = worksheet.Cells[row, patientIdCol].Text;
                        model.patient_id = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, patientIdCol].Text) && worksheet.Cells[row, patientIdCol].Text != "NULL") ? worksheet.Cells[row, patientIdCol].Text : null;
                    }

                    // Dos 

                    if (headerColumns.TryGetValue("Dos", out int dosCol))
                        //model.dos = ParseDate(worksheet.Cells[row, dosCol].Text);
                        if (DateTime.TryParseExact(worksheet.Cells[row, dosCol].Text, formats,
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None,
                                                  out dob))
                        {
                            model.dos = dob;
                        }
                    //cpt

                    if (headerColumns.TryGetValue("Cpt", out int cptCol))
                        model.cpt = worksheet.Cells[row, cptCol].Text;
                        model.cpt = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, cptCol].Text) && worksheet.Cells[row, cptCol].Text != "NULL") ? worksheet.Cells[row, cptCol].Text : null;

                    //claim id
                    if (headerColumns.TryGetValue("Claim Id", out int claimIdCol))
                    {
                        //string claimIdText = worksheet.Cells[row, claimIdCol].Text;
                        //model.claim_id = claimIdText;
                        model.claim_id = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, claimIdCol].Text) && worksheet.Cells[row, claimIdCol].Text != "NULL") ? worksheet.Cells[row, claimIdCol].Text : null;

                        if (model.claim_id == "N/A")
                        {
                            model.claim_id = null;
                        }
                    }

                    //claim date
                    if (headerColumns.TryGetValue("Claim date", out int claimDateCol))
                        if (DateTime.TryParseExact(worksheet.Cells[row, claimDateCol].Text, formats,
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dob))
                        {
                            model.claim_date = dob;
                        }

                    //encounter

                    if (headerColumns.TryGetValue("Encounter Id", out int encounterIdCol))
                    {
                        //string encIdText = worksheet.Cells[row, encounterIdCol].Text;
                        //model.encounter_id = encIdText;
                        model.encounter_id = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, encounterIdCol].Text) && worksheet.Cells[row, encounterIdCol].Text != "NULL") ? worksheet.Cells[row, encounterIdCol].Text : null;
                         if (model.encounter_id == "N/A")
                        {
                            model.encounter_id = null;
                        }
                    }
                

                    //insurance

                    if (headerColumns.TryGetValue("Insurance", out int insCol))
                    {
                        //string encIdText = worksheet.Cells[row, insCol].Text;
                        //model.insurance = encIdText;
                        model.insurance = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, insCol].Text) && worksheet.Cells[row, insCol].Text != "NULL") ? worksheet.Cells[row, insCol].Text : null;
                    }
                    //location

                    if (headerColumns.TryGetValue("Location/Facility", out int locCol))
                    {
                        //string encIdText = worksheet.Cells[row, locCol].Text;
                        //model.facility = encIdText;
                        model.facility = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, locCol].Text) && worksheet.Cells[row, locCol].Text != "NULL") ? worksheet.Cells[row, locCol].Text : null;
                    }

                    //username
                    if (headerColumns.TryGetValue("Username", out int userCol))
                    {
                        //string encIdText = worksheet.Cells[row, userCol].Text;
                        //model.username = encIdText;
                        model.username = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, userCol].Text) && worksheet.Cells[row, userCol].Text != "NULL") ? worksheet.Cells[row, userCol].Text : null;
                    }

                    // payment amount
                    if (headerColumns.TryGetValue("Paid Amount",out int payCol))
                    {
                        string amountText = worksheet.Cells[row, payCol].Text?.Trim().Replace("$", "");

                        if (double.TryParse(amountText, out double billedAmount))
                        {
                            model.paid_amount = billedAmount;
                        }
                        else
                        {
                            model.paid_amount = 0.0;
                        }
                    }

                    //payment date
                    if (headerColumns.TryGetValue("Payment Date", out int paydateCol))
                    {
                        if (DateTime.TryParseExact(worksheet.Cells[row, paydateCol].Text, formats,
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out dob))
                        {
                            model.payment_date = dob;
                        }
                    }

                    //Payment method

                    if (headerColumns.TryGetValue("Payment Method", out int paymethodCol))
                    {
                        //string encIdText = worksheet.Cells[row, paymethodCol].Text;
                        //model.payment_method = encIdText;
                        model.payment_method = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, paymethodCol].Text) && worksheet.Cells[row, paymethodCol].Text != "NULL") ? worksheet.Cells[row, paymethodCol].Text : null;
                    }

                    //vacre_user_id
                    if (headerColumns.TryGetValue("SEPS care ID", out int careidCol))
                    {
                        //string encIdText = worksheet.Cells[row, paymethodCol].Text;
                        //model.payment_method = encIdText;
                        model.sepscare_id = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, careidCol].Text) && worksheet.Cells[row, careidCol].Text != "NULL") ? worksheet.Cells[row, careidCol].Text : null;
                    }


                    //vacre_user_id
                    if (headerColumns.TryGetValue("Check#", out int checkCol))
                    {
                        //string encIdText = worksheet.Cells[row, paymethodCol].Text;
                        //model.payment_method = encIdText;
                        model.check_number = (!string.IsNullOrWhiteSpace(worksheet.Cells[row, checkCol].Text) && worksheet.Cells[row, checkCol].Text != "NULL") ? worksheet.Cells[row, checkCol].Text : null;
                    }


                    //get_source.Trim().Equals("PMSlogix", StringComparison.OrdinalIgnoreCase)
                    if (get_source.Trim().Equals("ECW", StringComparison.OrdinalIgnoreCase) ||
                                                 get_source.Trim().Equals("Officially", StringComparison.OrdinalIgnoreCase)
                                                 )
                    {
                        //if (!string.IsNullOrWhiteSpace(model.claim_id)&& !string.IsNullOrEmpty(model.claim_id))
                        //{

                        //    model.claim_id = model.encounter_id;
                        //}
                        model.claim_id = model.encounter_id;
                    }

                    if (get_source.Trim().Equals("PMSlogix", StringComparison.OrdinalIgnoreCase))
                    {
                        //if (!string.IsNullOrWhiteSpace(model.claim_id)&& !string.IsNullOrEmpty(model.claim_id))
                        //{

                        //    model.claim_id = model.encounter_id;
                        //}
                         model.encounter_id = model.claim_id;
                    }



                    if (model.dos == null || model.patient_id == null || model.cpt == null || model.claim_id == null || model.encounter_id == null || model.username==null)
                    {
                        // Missing critical data → mark invalid
                        invalidlist.Add(model);
                    }
                    else
                    {
                        // Try to find an existing record in chart_cparture_list
                        var oModel = payment_post_list.FirstOrDefault(a =>
                            a.patient_id == model.patient_id &&
                            a.dos == model.dos &&
                            a.encounter_id == model.encounter_id &&
                            a.cpt == model.cpt &&
                            a.claim_id == model.claim_id &&
                            a.patient_name == model.patient_name
                        );

                        if (oModel == null)
                        {
                            // Not found → new entry
                            list.Add(model);
                        }
                        else
                        {
                            // Found duplicate → existing entry
                            extlist.Add(model);
                        }
                    }




                }

                await _payment_postService.Create(list);
                var response = new
                {
                    TotalRecords = totalRows - 1,
                    Filename = get_filename,
                    Provider = get_type,
                    VerifiedData = list.Count,
                    ExistingData = extlist.Count,
                    InvalidData = invalidlist.Count


                };
                return Ok(response);

            }
           
        }

    }
}
