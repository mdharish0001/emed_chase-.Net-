using emedl_chase.Helper;
using emedl_chase.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using static emedl_chase.Model.Patient;

namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class chaseController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public chaseController (IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet("ecw-bearer")]
        public async  Task<IActionResult> Index()
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);

            return Ok(bearer);
        }
        [HttpGet("ecw-patient")]
        public async Task<IActionResult> GetFile(string name = null , DateTime? dos=null )
        {
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "files", "ecw_credentials.json");

            if (name == null)
            {
                return Ok("Enter the name");
            }

            
            var convert_dos=dos?.ToString("yyyy-MM-dd");

            if(!System.IO.File.Exists(filepath))
            {
                return Ok("file not found");
            }

            var json_data = System.IO.File.ReadAllText(filepath);

            var jsonser = JsonSerializer.Deserialize<ECWConfig>(json_data);

            var bearer = await ECWTokenHelper.GetECWTokenAsync(jsonser);

           
            var get_patient_json = await FhirApiCaller.CallFhirApiAsync(bearer, name);

            var fhir_id = "";

            foreach (var item in get_patient_json)
            {
                fhir_id = item.fhir_id;
            }
            var get_encounter_json = await FhirApiCaller.CallApiForEncounter(bearer, fhir_id, convert_dos);

            var encounter_id = "";

            foreach (var item in get_encounter_json)
            {
                encounter_id = item.encounterid;
            }

            var get_docencounterwithpatient_json = await FhirApiCaller.CallApiForDocrefreshEncounterwithPatient(bearer, fhir_id, encounter_id);

            var type = "encounter";

            var call_xml_reader_file = XmlConvertor.XmlConvertorUpdated(get_docencounterwithpatient_json.encounterxmldata, name, convert_dos, type);

            var get_binary_data = await FhirApiCaller.CallApiForDocrefresh(bearer, fhir_id);


            var get_binary = get_binary_data.binaryid;

            var get_binary_xmldate = await FhirApiCaller.CallApiForBinary(bearer, get_binary);

            var type1 = "full";

            var call_xml_reader_file_1 = XmlConvertor.XmlConvertorUpdated(get_binary_xmldate, name, convert_dos,type1);

            //return Ok(get_patient_json);
            return Ok(get_patient_json);
            
        }

        [HttpGet("CCDAFileRead")]

        public IActionResult CcdaFileReadProcess(string filepath= null)
        {
            //var file_path = "D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\Diaz Maria_2025-07-18_encounter.xml";

            if (filepath == null) {

                return Ok("No data fiund");
            
            }

             var content = XmlConvertor.CCDAFilread(filepath);
           var result=  XmlConvertor.ReadCCDAFile(filepath);
            return Ok(result);
        }

        [HttpPost("ChasefileUpload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Folder path: wwwroot/uploads/15_Sep_2025
                string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

                // Create folder if it doesn’t exist
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Full file path
                string filePath = Path.Combine(uploadPath, Path.GetFileName(file.FileName));

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { message = "File uploaded successfully!", path = filePath });
            }

            return BadRequest("No file uploaded.");
        }
        //    [NonAction]
        //    public static string GenerateEcwJwt(ECWConfig cred)
        //    {
        //        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //        var exp = now + 300;

        //        // Load private key
        //        var rsa = RSA.Create();
        //        string privateKeyPem = System.IO.File.ReadAllText(cred.private_key_path);
        //        rsa.ImportFromPem(privateKeyPem.ToCharArray());

        //        // Create signing credentials
        //        var signingCredentials = new SigningCredentials(
        //            new RsaSecurityKey(rsa)
        //            {
        //                KeyId = cred.kid
        //            },
        //            SecurityAlgorithms.RsaSha384
        //        );

        //        // Build claims manually (or use JwtPayload directly)
        //        var payload = new JwtPayload
        //{
        //    { "iss", cred.client_id },
        //    { "sub", cred.client_id },
        //    { "aud", cred.token_url },
        //    { "exp", exp },
        //    { "iat", now },
        //    { "jti", now.ToString() }
        //};

        //        // Manually create header to add kid and jku
        //        var header = new JwtHeader(signingCredentials);
        //        header["kid"] = cred.kid;
        //        header["jku"] = cred.jku;

        //        // Create the token
        //        var token = new JwtSecurityToken(header, payload);

        //        // Write token to string
        //        var handler = new JwtSecurityTokenHandler();
        //        return handler.WriteToken(token);
        //    }

        //[NonAction]

        //public static string GenerateAccessToekn(string jwttoken=null, string scope)
        //{
        //    var accesscode = string.Empty;
        //    var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //    var exp = now + 300;

        //    var jwt_token = GenerateEcwJwt(jwttoken);

        //     var headers = new headers {
        // "Content-Type": "application/x-www-form-urlencoded"
        //        }




        //    return accesscode;
        //}

    }
}
