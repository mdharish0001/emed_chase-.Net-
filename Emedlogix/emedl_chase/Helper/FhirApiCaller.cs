using emedl_chase.DbModel;
using emedl_chase.Model;
using Hl7.Fhir.Model;
using Hl7.Fhir.Model.CdsHooks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;



//using Hl7.Fhir.Model;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using static emedl_chase.Model.PatientDTO;

public class FhirApiCaller
{
    public static async Task<List<PatientDTO.fhirid>>   CallApiforPatientDemo(string accesstoken,string patientname)
    {
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);

        patientname = Uri.EscapeDataString(patientname);

        var baseurl =$"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient?name={patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get,baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",accesstoken);

        List<PatientDTO.fhirid> peoplefhir =new List<PatientDTO.fhirid>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id=System.Text.Json.JsonSerializer.Deserialize<PatientDTO.Rootobject>(responseBody);

            //Console.WriteLine($"Content Type: {get_fhir_id.resourceType}");

            //Console.WriteLine($"Base URL: {baseurl}");
            //Console.WriteLine($"Response Body: {responseBody}");

            if (get_fhir_id.entry != null)
            {
                peoplefhir = get_fhir_id.entry.Where(e => e.resource?.name != null 
                                                    && !string.IsNullOrEmpty(e.resource.id) && e.resource.meta != null)
                      .SelectMany(e => e.resource.name
                          .Where(n => !string.IsNullOrEmpty(n.text))
                          .Select(n => new PatientDTO.fhirid
                          {
                              fhir_id = e.resource.id,
                              name = n.text,
                              gender = e.resource.gender,
                              birthDate = e.resource.birthDate,
                              lastUpdated = e.resource.meta.lastUpdated,
                              fullurl = e.fullUrl,
                              active = e.resource.active,
                              bundleid = e.resource.id

                          }))
                      .ToList();
                Console.WriteLine("Patient Records: " + peoplefhir);
                return peoplefhir;
            }

           else
            {
                return peoplefhir;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<PatientDTO.fhirid>();
        }
    }
    public static async Task<List<AlergyIntolerance>> AllergyIntoleranceAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/AllergyIntolerance?patient={patientId}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var allergyBundle = System.Text.Json.JsonSerializer.Deserialize<AllergyBundle>(responseBody);

            List<AlergyIntolerance> allergyList = new();

            if (allergyBundle?.entry != null)
            {
                allergyList = allergyBundle.entry
                    .Where(x => x.resource != null)
                    .Select(x => new AlergyIntolerance
                    {
                        alergyid = x.resource.id,

                        alergyname = x.resource.code?.text,

                        clinicalStatus =
                            x.resource.clinicalStatus?.coding?
                                .FirstOrDefault()?.code,

                        verificationStatus =
                            x.resource.verificationStatus?.coding?
                                .FirstOrDefault()?.code,

                        category =
                            x.resource.category != null
                                ? string.Join(",", x.resource.category)
                                : null,

                        criticality =
                            x.resource.criticality,

                        recordedDate =
                            x.resource.recordedDate ?? DateTime.MinValue,

                        reaction =
                            x.resource.reaction?
                                .FirstOrDefault()?
                                .manifestation?
                                .FirstOrDefault()?
                                .text,

                        reference =
                            x.resource.patient?.reference,

                        recorder =
                            x.resource.recorder?.display
                    })
                    .ToList();
            }

            return allergyList;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<AlergyIntolerance>();
        }

    }
    public static async Task<List<ConditionInfo>> ConditionAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();
      
        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Condition?patient={patientId}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var conditionBundle = System.Text.Json.JsonSerializer.Deserialize<ConditionInfo>(responseBody);
            
            List<ConditionInfo> conditionList =
                    conditionBundle?.entry?
                    .Select(x => new ConditionInfo
                    {
                        ConditionId = x.resource.id,

                        Diagnosis = x.resource.code?.text,

                        ClinicalStatus = x.resource.clinicalStatus?.coding?
                            .FirstOrDefault()?.code,

                        VerificationStatus = x.resource.verificationStatus?.coding?
                            .FirstOrDefault()?.code,

                        Category = x.resource.category?
                            .FirstOrDefault()?.text,

                        Severity = x.resource.severity?.text,

                        OnsetDate = x.resource.onsetDateTime,

                        RecordedDate = x.resource.recordedDate,

                        Recorder = x.resource.recorder?.display,

                        PatientReference = x.resource.subject?.reference
                    })
                    .ToList() ?? new List<ConditionInfo>();
            return conditionList;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<ConditionInfo>();
        }

    }
    public static async Task<List<ObservationDTO>> ObservationAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?patient={patientId}";
        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?patient=Patient/{patientId}";
        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?subject=Patient/{patientId}";
        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?subject=Patient/{patientId}";
        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?subject=Patient/{Uri.EscapeDataString(patientId)}";
        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation";

        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?patient={patientId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.SendAsync(request);


            var errorBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("STATUS: " + response.StatusCode);
                Console.WriteLine("ERROR BODY: " + errorBody);
            }

            response.EnsureSuccessStatusCode();

            


            var json = await response.Content.ReadAsStringAsync();

            var bundle = System.Text.Json.JsonSerializer.Deserialize<FhirBundle<ObservationDTO>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var result = bundle?.entry?
                .Select(x => new ObservationDTO
                {
                    ObservationId = x.resource.ObservationId,
                    ObservationName = x.resource.ObservationName,
                    Value = x.resource.valueQuantity?.value?.ToString(),
                    Unit = x.resource.valueQuantity?.unit,
                    Status = x.resource.Status,
                    EffectiveDate = x.resource.EffectiveDate
                })
                .ToList() ?? new List<ObservationDTO>();

            return result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Observation API failed: {ex.Message}");
            return new List<ObservationDTO>();
        }
    }
    public static async Task<List<ProcedureDTO>> ProcedureAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        //var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Procedure?patient={patientId}";
        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Procedure?subject=Patient/{patientId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request);


        var errorBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("STATUS: " + response.StatusCode);
            Console.WriteLine("ERROR BODY: " + errorBody);
        }

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine(json);

        var bundle = System.Text.Json.JsonSerializer.Deserialize<FhirBundle<ProcedureDTO>>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


        var result = new List<ProcedureDTO>();
        if (bundle?.entry == null || !bundle.entry.Any())
        {
            return new List<ProcedureDTO>();
        }
        foreach (var entry in bundle.entry)
        {
            result.Add(
                new ProcedureDTO
                {
                    ProcedureName = entry.resource?.code.text,

                    Status = entry.resource.Status,

                    PerformedDate = entry.resource.PerformedDate
                });
        }

        return result;
    }
    public static async Task<List<ImmunizationDTO>> ImmunizationAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Immunization?patient={patientId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();
     
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("STATUS: " + response.StatusCode);
            Console.WriteLine("ERROR BODY: " + json);
        }
       
        var bundle = System.Text.Json.JsonSerializer.Deserialize<FhirBundle<ImmunizationDTO>>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var result = new List<ImmunizationDTO>();

        if (bundle?.entry == null || !bundle.entry.Any())
        {
            return new List<ImmunizationDTO>();
        }

        foreach (var entry in bundle.entry)
        {
            result.Add(
                new ImmunizationDTO
                {
                    Vaccine = entry.resource.Vaccine,

                    Status =  entry.resource.Status,

                    Date =  entry.resource.Date
                });
        }

        return result;
    }
    public static async Task<List<MedicationDTO>> MedicationAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/MedicationRequest?patient={patientId}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            //var medicationBundle = JsonSerializer.Deserialize<MedicationDTO>(responseBody);

            var medicationBundle = System.Text.Json.JsonSerializer.Deserialize<MedicationBundle>(responseBody,
                                            new JsonSerializerOptions
                                            {
                                                PropertyNameCaseInsensitive = true
                                            });

            List<MedicationDTO> medicationList =
                    medicationBundle?.entry?
                    .Select(x => new MedicationDTO
                    {
                        MedicationId = x.resource.id,

                        MedicationName =
                            x.resource.medicationCodeableConcept?.text,

                        Status =
                            x.resource.status,

                        Intent =
                            x.resource.intent,

                        DosageInstruction =
                            x.resource.dosageInstruction?
                                .FirstOrDefault()?.text,

                        Route =
                            x.resource.dosageInstruction?
                                .FirstOrDefault()?.route?.text,

                        Frequency =
                            x.resource.dosageInstruction?
                                .FirstOrDefault()?
                                .timing?.repeat?.frequency?.ToString(),

                        AuthoredOn =
                            x.resource.authoredOn,

                        Prescriber =
                            x.resource.requester?.display
                    })
                    .ToList() ?? new List<MedicationDTO>();


            return medicationList;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<MedicationDTO>();
        }

    }   
    public static async Task<List<EncounterDTO.finalresponse>> EncounterAPIold(string accesstoken, string patientname, string dos)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        List<EncounterDTO.finalresponse> encounterapi = new List<EncounterDTO.finalresponse>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<EncounterDTO.Rootobject>(responseBody);

            encounterapi = get_fhir_id.entry.Where(a => a.resource?.reasonCode != null && a.resource?.period?.start.ToString("yyyy-MM-dd") == dos).SelectMany(e => e.resource.reasonCode
                .Where(n => !string.IsNullOrEmpty(n.text)).Select(n => new EncounterDTO.finalresponse
                {
                    encounternote = n.text,
                    dos = e.resource.period.start.ToString("yyyy-MM-dd"),
                    encounterid=e.resource.id,
                })).ToList();

            return encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<EncounterDTO.finalresponse>();
        }
    }


    public static async Task<List<EncounterDTO.finalresponse>> EncounterAPI(
    string accesstoken, string patientId, string dos)
    {
        using var client = new HttpClient();

        // ── URL is correct, no change needed ────────────────────────────
        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        // DEBUG — add this temporarily to see what ECW actually returns
        Console.WriteLine($"[EncounterAPI] raw JSON (first 500):");
        Console.WriteLine(json.Length > 500 ? json[..500] : json);

        var bundle = System.Text.Json.JsonSerializer.Deserialize<EncounterDTO.Rootobject>(json);

        if (bundle?.entry == null || !bundle.entry.Any())
        {
            Console.WriteLine("[EncounterAPI] bundle.entry is null or empty");
            return new List<EncounterDTO.finalresponse>();
        }

        Console.WriteLine($"[EncounterAPI] total entries: {bundle.entry.Length}");

        // ── Normalize DOS to yyyy-MM-dd regardless of what was passed in ─
        // convert_dos might be "2026-06-02" OR "06/02/2026" OR "06-02-2026"
        // Normalize to yyyy-MM-dd for comparison
        string dosNormalized = "";
        if (!string.IsNullOrEmpty(dos))
        {
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "MM-dd-yyyy",
                              "M/d/yyyy",  "M-d-yyyy",   "yyyyMMdd" };
            if (DateTime.TryParseExact(dos.Trim(), formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var parsedDos))
            {
                dosNormalized = parsedDos.ToString("yyyy-MM-dd");
                Console.WriteLine($"[EncounterAPI] DOS normalized: {dos} → {dosNormalized}");
            }
            else
            {
                Console.WriteLine($"[EncounterAPI] WARNING: could not parse DOS '{dos}'");
                dosNormalized = dos; // use as-is
            }
        }

        var allEntries = bundle.entry.Where(e => e.resource?.period?.start != null).ToList();

        Console.WriteLine($"[EncounterAPI] entries with period.start: {allEntries.Count}");
        foreach (var e in allEntries)
            Console.WriteLine($"  encounter id={e.resource.id} " +
                              $"start={e.resource.period.start:yyyy-MM-dd} " +
                              $"status={e.resource.status}");

        // ── Filter by DOS if provided ────────────────────────────────────
        // Compare date portion only, timezone-safe:
        //   e.resource.period.start is DateTime — use .Date to strip time
        //   then format as yyyy-MM-dd and compare to normalized DOS
        var result = allEntries
            .Where(e =>
            {
                if (string.IsNullOrEmpty(dosNormalized)) return true;

                // .Date strips the time component — timezone already resolved
                // by C# JsonSerializer when deserializing the ISO 8601 string
                string entryDate = ((DateTime)e.resource.period.start)
                                    .Date
                                    .ToString("yyyy-MM-dd");

                bool match = entryDate == dosNormalized;
                Console.WriteLine($"  [filter] {entryDate} == {dosNormalized} → {match}");
                return match;
            })
            .Select(e => new EncounterDTO.finalresponse
            {
                encounterid = e.resource.id,
                dos = ((DateTime)e.resource.period.start)
                                .Date.ToString("yyyy-MM-dd"),
                encounternote = e.resource.reasonCode?.FirstOrDefault()?.text
            })
            .ToList();

        Console.WriteLine($"[EncounterAPI] matched: {result.Count}");

        // ── Fallback: if no DOS match, return most recent encounter ─────
        // This prevents the caller from getting an empty list and failing
        // when the patient has encounters but none exactly on this DOS
        if (!result.Any() && !string.IsNullOrEmpty(dosNormalized))
        {
            Console.WriteLine("[EncounterAPI] No DOS match — returning most recent encounter");
            var fallback = allEntries
                .OrderByDescending(e => e.resource.period.start)
                .FirstOrDefault();

            if (fallback != null)
                result.Add(new EncounterDTO.finalresponse
                {
                    encounterid = fallback.resource.id,
                    dos = ((DateTime)fallback.resource.period.start)
                                    .Date.ToString("yyyy-MM-dd"),
                    encounternote = fallback.resource.reasonCode?.FirstOrDefault()?.text
                });
        }

        return result;
    }

    public static async Task<List<EncounterDTO.finalresponse>> EncounterAPI123(string accesstoken, string patientId, string dos)
    {
        using var client = new HttpClient();

        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var bundle = System.Text.Json.JsonSerializer.Deserialize<EncounterDTO.Rootobject>(json);

        if (bundle?.entry == null)
            return new List<EncounterDTO.finalresponse>();

        var result = bundle.entry.Where(e => e.resource?.period?.start != null)
                    .Where(e =>
                    {
                        if (string.IsNullOrEmpty(dos))
                            return true;

                        DateTime start = e.resource.period.start;

                        return start.ToString("yyyy-MM-dd") == dos;
                    })
                    .Select(e => new EncounterDTO.finalresponse
                    {
                        encounterid = e.resource.id,
                        dos = ((DateTime)e.resource.period.start).ToString("yyyy-MM-dd"),
                        encounternote = e.resource.reasonCode?.FirstOrDefault()?.text
                    })
                    .ToList();

        //var result = bundle.entry
        //    .Where(e => e.resource?.period?.start != null)
        //    .Where(e => string.IsNullOrEmpty(dos) || e.resource.period.start.Date.ToString("yyyy-MM-dd") == dos)
        //    .Select(e => new EncounterDTO.finalresponse
        //    {
        //        encounterid = e.resource.id,
        //        dos = e.resource.period.start.Date.ToString("yyyy-MM-dd"),
        //        encounternote = e.resource.reasonCode?.FirstOrDefault()?.text
        //    })
        //    .ToList();
        /// /// /// 
        //var result = bundle.entry
        //    .Where(e => e.resource?.period?.start != null)
        //    .Where(e => string.IsNullOrEmpty(dos) ||  e.resource.period.start.Date.ToString("yyyy-MM-dd") == dos)
        //    .Select(e => new EncounterDTO.finalresponse
        //    {
        //        encounterid = e.resource.id,
        //        dos = e.resource.period.start.Date.ToString("yyyy-MM-dd"),
        //        encounternote = e.resource.reasonCode?.FirstOrDefault()?.text
        //    })
        //    .ToList();

        return result;
    }

    public static async Task<List<DiagnosticReportDTO>> DiagnosticReportAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        var url =
            $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DiagnosticReport?patient={patientId}";

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        var json =
            await client.GetStringAsync(url);

        dynamic bundle =
            JsonConvert.DeserializeObject(json);

        var result =
            new List<DiagnosticReportDTO>();

        foreach (var entry in bundle.entry)
        {
            result.Add(
                new DiagnosticReportDTO
                {
                    ReportName =
                        entry.resource.code.text,

                    Status =
                        entry.resource.status,

                    Conclusion =
                        entry.resource.conclusion
                });
        }

        return result;
    }
    public static async Task<DocRefresh.finalresonse> CallApiForDocrefresh(string accesstoken, string patientname)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);

            //Console.WriteLine(documentReference.content[0].attachment.contentType);

            var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<DocRefresh.Rootobject>(responseBody);


            var encounterapi = get_fhir_id.entry.Select(n => new DocRefresh.finalresonse
            {
                binaryurl = n.fullUrl,
                type = n.resource.resourceType,
                binaryid = n.resource.id,
            }).FirstOrDefault();

            

            return  encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
    }
    public static async Task<string> CallApiForBinary(string accesstoken, string patientname)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Binary/{patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();
            var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<BinaryDTO.Rootobject>(responseBody);
            var encounterapi = get_fhir_id.data.ToString();       
            return encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
    }

    public static async Task<string> GetCcdaXml(string accessToken, string binaryId, string patientName, string dos)
    {
        using var client = new HttpClient();

        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Binary/{binaryId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
       
        var xml = await response.Content.ReadAsStringAsync();

        string filepathccda = "";
        if (!string.IsNullOrWhiteSpace(xml))
        {
            string folderName = DateTime.Now.ToString("dd_MMM_yyyy");

            string uploadPath = Path.Combine( Directory.GetCurrentDirectory(),"wwwroot", "uploads", folderName);

            // 2. Ensure directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 3. Clean filename (important)
            string safeName = string.Concat(patientName.Split(Path.GetInvalidFileNameChars()));
            string safeDos = string.Concat(dos.Split(Path.GetInvalidFileNameChars()));

            var fileName = $"{safeName}_{safeDos}.xml";

            filepathccda = Path.Combine(uploadPath, fileName);

            // 4. Write file using correct variable
            File.WriteAllText(filepathccda, xml);
        }
        else
        {
            // optional logging
            Console.WriteLine("XML response is empty, file not saved.");
        }

        return xml;
        //return filepathccda;
    }

    
    public static async Task<string> GetCcdaXmlold(string accessToken,string binaryId)
    {
        using var client = new HttpClient();
        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Binary/{binaryId}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);


        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.SendAsync(request);

        Console.WriteLine(response.Content.Headers.ContentType);
        Console.WriteLine($"Status: {response.StatusCode}");

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);
        Console.WriteLine(body.Substring(0, Math.Min(500, body.Length)));      

        File.WriteAllText(@"D:\Temp\Note.html", body);

        return await client.GetStringAsync(baseurl);
    }

    public static async Task<DocRefreshEncounterWithPatient.finalresonse>CallApiForDocrefreshEncounterwithPatient(string accesstoken, string patientid, string encounterid)
    {
        using var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/" + $"DocumentReference?encounter={encounterid}&patient={patientid}";

        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // BUG 1 FIX: was "application/json+fhir" — must be "application/fhir+json"
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
             
            var bundle = System.Text.Json.JsonSerializer.Deserialize<DocRefreshEncounterWithPatient.Rootobject>(responseBody,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // handles ECW JSON case variations
                });

            //if (bundle?.entry == null || !bundle.entry.Any())
            //{
            //    Console.WriteLine("[DocRefEnc] no entries in bundle");
            //    return null;
            //}


            var encounterapi = bundle.entry.Where(a => a.resource?.data != null).Select(n => new DocRefreshEncounterWithPatient.finalresonse
            {
                encounterxmldata = n.resource.data.ToString(),
                encounterdate = n.resource?.context?.period?.start.ToString(),
            }).FirstOrDefault();

            if (bundle?.entry != null || bundle.entry.Any())
            {
                Console.WriteLine($"[DocRefEnc] entries: {bundle.entry.Length}");

                foreach (var entry in bundle.entry)
                {
                    var resource = entry?.resource;
                    if (resource == null) continue;

                    // BUG 2 FIX: data is at content[].attachment.data
                    // NOT at resource.data
                    var attachment = resource.content?.FirstOrDefault(c => c?.attachment?.data != null)?.attachment;

                    if (attachment == null)
                    {
                        // Some ECW DocumentReferences provide a URL instead of inline data
                        var urlAttachment = resource.content?.FirstOrDefault(c => !string.IsNullOrEmpty(c?.attachment?.url))?.attachment;

                        if (urlAttachment != null)
                        {
                            Console.WriteLine($"[DocRefEnc] data is at URL: {urlAttachment.url}");
                            // Would need a separate Binary fetch here — handled below
                        }

                        Console.WriteLine("[DocRefEnc] no inline attachment.data found");
                        continue;
                    }

                    // BUG 3 FIX: data is base64 encoded — decode to UTF-8 string
                    string xmlContent;
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(attachment.data);
                        xmlContent = System.Text.Encoding.UTF8.GetString(bytes);
                    }
                    catch (FormatException)
                    {
                        // Not base64 — use as plain text directly (some ECW versions)
                        xmlContent = attachment.data;
                    }

                    // Strip BOM if present
                    xmlContent = xmlContent.TrimStart('\uFEFF', '\u200B').Trim();

                    Console.WriteLine($"[DocRefEnc] decoded XML length: {xmlContent.Length}");
                    Console.WriteLine($"[DocRefEnc] content start: {xmlContent[..Math.Min(100, xmlContent.Length)]}");

                    string encounterDate = resource.context?.period?.start
                                           ?.ToString("yyyy-MM-dd") ?? "";

                    return new DocRefreshEncounterWithPatient.finalresonse
                    {
                        encounterxmldata = xmlContent,
                        encounterdate = encounterDate
                    };
                }
            }
            
            return encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[DocRefEnc] HTTP error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DocRefEnc] error: {ex.Message}");
            return null;
        }
    }
    
    
    //public static async Task<DocRefreshEncounterWithPatient.finalresonse> CallApiForDocrefreshEncounterwithPatientold(string accesstoken, string patientid,string encounterid)
    //{
    //    var client = new HttpClient();

    //    var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?encounter={encounterid}&patient={patientid}";
    //    //var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?encounter={encounterid}&patient={patientid}";
    //    var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

    //    // Set required headers
    //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
    //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

    //    try
    //    {
    //        var response = await client.SendAsync(request);

    //        response.EnsureSuccessStatusCode();  // throws if not 200-299

    //        var responseBody = await response.Content.ReadAsStringAsync();

    //        var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<DocRefreshEncounterWithPatient.Rootobject>(responseBody);


    //        var encounterapi = get_fhir_id.entry.Where(a => a.resource?.data != null).Select(n=> new DocRefreshEncounterWithPatient.finalresonse
    //        {
    //            encounterxmldata=n.resource.data.ToString(),
    //            encounterdate=n.resource?.context?.period?.start.ToString(),
    //        }).FirstOrDefault();
    //        //    .Select(n => new DocRefreshEncounterWithPatient.finalresonse
    //        //{
    //        //   encounterdate=n.resource.context.period.start.ToString(),
    //        //   encounterid=n.resource.id,
    //        //   encountereason=n.resource.type.text,
    //        //   encounterxmldata = n.resource.data?.ToString()

    //        //}).FirstOrDefault();



    //        return encounterapi;
    //    }
    //    catch (HttpRequestException ex)
    //    {
    //        Console.WriteLine($"Request failed: {ex.Message}");
    //        return null;
    //    }
    //}
    public static async Task<List<PatientDTO.fhirid>> CallApiforwithFhirPatientDemo(string accesstoken, string fhiriid)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient/{fhiriid}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        List<PatientDTO.fhirid> peoplefhir = new List<PatientDTO.fhirid>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<PatientDTO.Rootobject>(responseBody);
            if (get_fhir_id.entry != null)
            {
                peoplefhir = get_fhir_id.entry
          .Where(e => e.resource?.name != null && !string.IsNullOrEmpty(e.resource.id) && e.resource.meta != null)
          .SelectMany(e => e.resource.name
              .Where(n => !string.IsNullOrEmpty(n.text))
              .Select(n => new PatientDTO.fhirid
              {
                  fhir_id = e.resource.id,
                  name = n.text,
                  gender = e.resource.gender,
                  birthDate = e.resource.birthDate,
                  lastUpdated = e.resource.meta.lastUpdated,
                  fullurl = e.fullUrl,
                  active = e.resource.active,
                  bundleid = e.resource.id

              }))
          .ToList();

                return peoplefhir;
            }

            else
            {
                return peoplefhir;
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return new List<PatientDTO.fhirid>();
        }
    }


    public static async Task<List<DocumentReferenceDTO>> DocumentReferenceAPI(string accessToken, string patientId)
    {
        var client = new HttpClient();

        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientId}";

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync(url);

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Status: {(int)response.StatusCode} {response.StatusCode}\n" +
                $"Response: {responseText}");
        }

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        dynamic bundle = JsonConvert.DeserializeObject(json);

        var result = new List<DocumentReferenceDTO>();

        if (bundle.entry != null)
        {
            foreach (var entry in bundle.entry)
            {
                result.Add(new DocumentReferenceDTO
                {
                    Id = entry.resource.id,
                    Description = entry.resource.description,
                    Status = entry.resource.status,
                    Date = entry.resource.date
                });
            }
        }

        return result;
    }



    public static async Task<UnifiedClinicalRecord> GetUnifiedPatientRecord(string accessToken,string patientId, string patientname)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var record = new UnifiedClinicalRecord();

        // Run in parallel (faster)
        var PatientTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient?name={patientname}");
        var conditionTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Condition?patient={patientId}");
        var procedureTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Procedure?patient={patientId}");
        var encounterTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientId}");
        var docTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientId}");
        var ObservationTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?patient={patientId}");

        var compositionTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Composition?patient={patientId}");
        var medicationRequestTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/MedicationRequest?patient={patientId}");
        var MedicationStatementTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/MedicationStatement?patient={patientId}");
        var AllergyIntoleranceTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/AllergyIntolerance?patient={patientId}");
        var ImmunizationTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Immunization?patient={patientId}");
        var CarePlanTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/CarePlan?patient={patientId}");
        var DiagnosticReportTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DiagnosticReport?patient={patientId}");
        var SpecimenTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Specimen?patient={patientId}");
        var DeviceTask = client.GetStringAsync($"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Device?patient={patientId}");


        //await System.Threading.Tasks.Task.WhenAll(conditionTask, procedureTask, encounterTask, docTask, compositionTask);

        // Parse each response
        ExtractPatient(record, PatientTask.Result);
        ExtractDOS(record, encounterTask.Result);
        ExtractConditions(record, conditionTask.Result);
        ExtractProcedures(record, procedureTask.Result);        
        ExtractEncounters(record, encounterTask.Result);
        ExtractDocuments(record, docTask.Result);
        ExtractAllergies(record, AllergyIntoleranceTask.Result);        
        //ExtractDocumentReferences(record, docRefJson);      
        
        ExtractDiagnosticReports(record, DiagnosticReportTask.Result);

        
        

        // Build final narrative (VERY IMPORTANT for PDF)
        record.FullNarrative = BuildNarrative(record);

        WriteToTextFile(record);

        ExtractMedicationRequests(record, medicationRequestTask.Result);
        ExtractMedicationStatements(record, MedicationStatementTask.Result);
        ExtractObservations(record, ObservationTask.Result);
        ExtractImmunizations(record, ImmunizationTask.Result);
        ExtractComposition(record, compositionTask.Result);
        ExtractCarePlans(record, CarePlanTask.Result);
        ExtractSpecimens(record, SpecimenTask.Result);
        ExtractDevices(record, DeviceTask.Result);

        return record;

        
    }

    static void ExtractPatient(UnifiedClinicalRecord record, string json)
    {

        dynamic p = JsonConvert.DeserializeObject(json);

        var name = p?.name?[0]?.text?.ToString();
        var dob = p?.birthDate?.ToString();
        var gender = p?.gender?.ToString();

        record.PatientInfo = $"{name} | {dob} | {gender}";
    }
    static void WriteToTextFile(UnifiedClinicalRecord record)
    {
        var sb = new StringBuilder();

        sb.AppendLine("===== PATIENT COMPLETE RECORD =====");
        sb.AppendLine($"Date of Service: {record.DateOfService}");
        sb.AppendLine("-----------------------------------");

        sb.AppendLine("\n=== DIAGNOSES ===");
        foreach (var d in record.Diagnoses)
            sb.AppendLine("- " + d);

        sb.AppendLine("\n=== PROCEDURES ===");
        foreach (var p in record.ProceduresU)
            sb.AppendLine("- " + p);

        sb.AppendLine("\n=== ENCOUNTERS ===");
        foreach (var e in record.EncounterSummaries)
            sb.AppendLine("- " + e);

        sb.AppendLine("\n=== CLINICAL DOCUMENTS ===");
        foreach (var c in record.ClinicalDocuments)
            sb.AppendLine(c);

        sb.AppendLine("\n=== OBSERVATIONS  ===");
        foreach (var OB in record.Observations)
            sb.AppendLine(OB);

        sb.AppendLine("\n=== MEDICATIONS  ===");
        foreach (var MU in record.MedicationsU)
            sb.AppendLine(MU);

        //sb.AppendLine("\n=== OBSERVATIONS  ===");
        //foreach (var OB in record.Observations)
        //    sb.AppendLine(OB);

        //sb.AppendLine("\n=== OBSERVATIONS  ===");
        //foreach (var OB in record.Observations)
        //    sb.AppendLine(OB);

        //sb.AppendLine("\n=== OBSERVATIONS  ===");
        //foreach (var OB in record.Observations)
        //    sb.AppendLine(OB);

        sb.AppendLine("\n=== FULL NARRATIVE ===");
        sb.AppendLine(record.FullNarrative);

        File.WriteAllText(@"D:\Temp\ECW_Patient_Record.txt", sb.ToString());
    }
    static void ExtractDOS(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        DateTime latest = DateTime.MinValue;

        foreach (var entry in bundle.entry)
        {
            var start = entry?.resource?.period?.start?.ToString();

            if (DateTime.TryParse(start, out DateTime dt))
            {
                if (dt > latest)
                    latest = dt;
            }
        }

        record.DateOfService =  latest == DateTime.MinValue ? null : latest.ToString("yyyy-MM-dd");
    }
    static void ExtractConditions(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var text = entry?.resource?.code?.text?.ToString();
            if (!string.IsNullOrEmpty(text))
                record.Diagnoses.Add(text);
            Console.WriteLine("Conditions: " + text);
            
        }
    }

    static void ExtractProcedures(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var proc = entry?.resource;

            string name = proc?.code?.text?.ToString();
            string note = proc?.note?[0]?.text?.ToString();

            if (!string.IsNullOrEmpty(name))
                record.ProceduresU.Add(name + (note != null ? " - " + note : ""));
            Console.WriteLine("Procedures: " + name);

        }
    }

    static void ExtractObservations(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var text = entry?.resource?.code?.text?.ToString();
            var value = entry?.resource?.valueQuantity?.value?.ToString();

            if (!string.IsNullOrEmpty(text))
                record.Observations.Add($"{text}: {value}");
        }
    }

    static void ExtractMedicationRequests(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var med = entry?.resource?.medicationCodeableConcept?.text?.ToString();

            if (!string.IsNullOrEmpty(med))
                record.MedicationsU.Add(med);
        }
    }

    static void ExtractMedicationStatements(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var med = entry?.resource?.medicationCodeableConcept?.text?.ToString();

            if (!string.IsNullOrEmpty(med))
                record.MedicationsU.Add("Statement: " + med);
        }
    }

    static void ExtractAllergies(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var allergy = entry?.resource?.code?.text?.ToString();
            var reaction = entry?.resource?.reaction?[0]?.manifestation?[0]?.text?.ToString();

            if (!string.IsNullOrEmpty(allergy))
                record.Allergiesu.Add($"{allergy} - {reaction}");
        }
    }

    static void ExtractImmunizations(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var vac = entry?.resource?.vaccineCode?.text?.ToString();

            if (!string.IsNullOrEmpty(vac))
                record.Immunizationsu.Add(vac);
        }
    }

    static void ExtractDocumentReferences(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var resource = entry?.resource;

            var text = resource?.content?[0]?.attachment?.data?.ToString();
            var url = resource?.content?[0]?.attachment?.url?.ToString();

            if (!string.IsNullOrEmpty(text))
                record.Documentsu.Add(text);

            if (!string.IsNullOrEmpty(url))
                record.Documentsu.Add(url);
        }
    }

    
    static void ExtractEncounters(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var reason = entry?.resource?.reasonCode?[0]?.text?.ToString();

            if (!string.IsNullOrEmpty(reason))
                record.EncounterSummaries.Add(reason);
            Console.WriteLine("Encounters: " + reason);

        }
    }


    static void ExtractDocuments(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var resource = entry?.resource;

            // Most important field (clinical note text)
            string text = resource?.content?[0]?.attachment?.data;

            // If base64 encoded
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(text));
                    record.ClinicalDocuments.Add(decoded);
                    Console.WriteLine("Document: " + decoded);

                }
                catch
                {
                    record.ClinicalDocuments.Add(text);
                }
            }

            // Sometimes plain text is present
            string urlText = resource?.content?[0]?.attachment?.url?.ToString();
            if (!string.IsNullOrEmpty(urlText))
                record.ClinicalDocuments.Add(urlText);
            Console.WriteLine("Document Text: " + urlText);

        }
    }

    static void ExtractComposition(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var div = entry?.resource?.section?[0]?.text?.div?.ToString();

            if (!string.IsNullOrEmpty(div))
                record.ClinicalDocuments.Add(div);
            Console.WriteLine("Clinical Document Text: " + div);

        }
    }

    static void ExtractCarePlans(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var activity = entry?.resource?.activity?[0]?.detail?.code?.text?.ToString();

            if (!string.IsNullOrEmpty(activity))
                record.CarePlans.Add(activity);
        }
    }

    static void ExtractDiagnosticReports(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var report = entry?.resource?.code?.text?.ToString();

            if (!string.IsNullOrEmpty(report))
                record.DiagnosticReports.Add(report);
        }
    }

    static void ExtractSpecimens(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var type = entry?.resource?.type?.text?.ToString();

            if (!string.IsNullOrEmpty(type))
                record.Specimens.Add(type);
        }
    }

    static void ExtractDevices(UnifiedClinicalRecord record, string json)
    {
        dynamic bundle = JsonConvert.DeserializeObject(json);

        if (bundle?.entry == null) return;

        foreach (var entry in bundle.entry)
        {
            var device = entry?.resource?.type?.text?.ToString();

            if (!string.IsNullOrEmpty(device))
                record.Devices.Add(device);
        }
    }
    
    static string BuildNarrative(UnifiedClinicalRecord r)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== DIAGNOSES ===");
        foreach (var d in r.Diagnoses)
            sb.AppendLine("- " + d);

        sb.AppendLine("\n=== PROCEDURES ===");
        foreach (var p in r.ProceduresU)
            sb.AppendLine("- " + p);

        sb.AppendLine("\n=== ENCOUNTERS ===");
        foreach (var e in r.EncounterSummaries)
            sb.AppendLine("- " + e);

        sb.AppendLine("\n=== CLINICAL DOCUMENTS / OPERATIVE NOTES ===");
        foreach (var c in r.ClinicalDocuments)
            sb.AppendLine(c);

        return sb.ToString();
    }


    public static async Task<string> GetCcFromEncounterResource(string accessToken, string encounterId)
    {
        if (string.IsNullOrEmpty(encounterId)) return "";

        using var client = new HttpClient();
        // GET /Encounter/{id} — direct resource fetch, NOT a search
        var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter/{encounterId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return "";

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[GetCC] Encounter/{encounterId} response: {json[..Math.Min(300, json.Length)]}");

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Try reasonCode[].text first
            if (root.TryGetProperty("reasonCode", out var reasonCodes))
            {
                var texts = new List<string>();
                foreach (var rc in reasonCodes.EnumerateArray())
                {
                    if (rc.TryGetProperty("text", out var t))
                    {
                        string v = t.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(v)) texts.Add(v);
                    }
                    else if (rc.TryGetProperty("coding", out var codings))
                    {
                        foreach (var coding in codings.EnumerateArray())
                            if (coding.TryGetProperty("display", out var d))
                            {
                                string v = d.GetString()?.Trim();
                                if (!string.IsNullOrEmpty(v)) texts.Add(v);
                            }
                    }
                }
                if (texts.Any()) return string.Join("; ", texts);
            }

            // Try reasonReference (some ECW versions use this instead)
            if (root.TryGetProperty("reasonReference", out var reasonRefs))
            {
                var texts = new List<string>();
                foreach (var rr in reasonRefs.EnumerateArray())
                {
                    if (rr.TryGetProperty("display", out var d))
                    {
                        string v = d.GetString()?.Trim();
                        if (!string.IsNullOrEmpty(v)) texts.Add(v);
                    }
                }
                if (texts.Any()) return string.Join("; ", texts);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GetCC] Error: {ex.Message}");
        }

        return "";
    }

    public static async Task<string> GetRosFromDocumentReference(
        string accessToken, string patientId, string encounterId, string dos)
    {
        using var client = new HttpClient();

        // Try 1: encounter-specific search
        string url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/" +
                     $"DocumentReference?encounter={encounterId}&patient={patientId}";

        string noteText = await FetchNoteTextAsync(client, accessToken, url);

        // Try 2: patient + date search if encounter search fails
        if (string.IsNullOrEmpty(noteText))
        {
            // Convert dos "2026-06-02" to FHIR date format
            url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/" +
                  $"DocumentReference?patient={patientId}&date={dos}&category=clinical-note";
            noteText = await FetchNoteTextAsync(client, accessToken, url);
        }

        // Try 3: patient search, get most recent clinical note
        if (string.IsNullOrEmpty(noteText))
        {
            url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/" +
                  $"DocumentReference?patient={patientId}&category=clinical-note&_sort=-date&_count=1";
            noteText = await FetchNoteTextAsync(client, accessToken, url);
        }

        if (string.IsNullOrEmpty(noteText)) return "";

        // Extract ROS section from the note text
        return ExtractSectionFromNote(noteText, new[]
        {
        "review of systems", "ros:", "ros\n", "systems review"
    });
    }

    private static async Task<string> FetchNoteTextAsync(
        HttpClient client, string accessToken, string url)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(req);
            if (!response.IsSuccessStatusCode) return "";

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DocRef] url={url.Split('?')[1]} → {json.Length} chars");

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("entry", out var entries)) return "";

            foreach (var entry in entries.EnumerateArray())
            {
                if (!entry.TryGetProperty("resource", out var resource)) continue;

                // Get content[].attachment
                if (!resource.TryGetProperty("content", out var content)) continue;

                foreach (var c in content.EnumerateArray())
                {
                    if (!c.TryGetProperty("attachment", out var att)) continue;

                    // Inline base64 data
                    if (att.TryGetProperty("data", out var dataEl))
                    {
                        string b64 = dataEl.GetString() ?? "";
                        if (!string.IsNullOrEmpty(b64))
                        {
                            try
                            {
                                byte[] bytes = Convert.FromBase64String(b64);
                                string text = System.Text.Encoding.UTF8.GetString(bytes);
                                text = text.TrimStart('\uFEFF', '\u200B');
                                // Strip HTML tags if present
                                text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", " ");
                                Console.WriteLine($"[DocRef] decoded {bytes.Length} bytes, starts: {text[..Math.Min(100, text.Length)]}");
                                if (text.Length > 50) return text;
                            }
                            catch { }
                        }
                    }

                    // URL reference — fetch separately
                    if (att.TryGetProperty("url", out var urlEl))
                    {
                        string binaryUrl = urlEl.GetString() ?? "";
                        if (!string.IsNullOrEmpty(binaryUrl))
                        {
                            try
                            {
                                var req2 = new HttpRequestMessage(HttpMethod.Get, binaryUrl);
                                req2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                                var resp2 = await client.SendAsync(req2);
                                if (resp2.IsSuccessStatusCode)
                                {
                                    string raw = await resp2.Content.ReadAsStringAsync();
                                    // If it's a FHIR Binary resource
                                    if (raw.TrimStart().StartsWith("{"))
                                    {
                                        using var binDoc = System.Text.Json.JsonDocument.Parse(raw);
                                        if (binDoc.RootElement.TryGetProperty("data", out var binData))
                                        {
                                            byte[] bytes = Convert.FromBase64String(binData.GetString() ?? "");
                                            return System.Text.Encoding.UTF8.GetString(bytes);
                                        }
                                    }
                                    return raw;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DocRef] Error: {ex.Message}");
        }

        return "";
    }

    private static string ExtractSectionFromNote(string text, string[] headings)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        string stopPattern =
            @"chief complaint|review of systems|ros:|history of present|hpi|" +
            @"past medical|medication|allerg|physical exam|assessment|plan|" +
            @"vital|procedure|immunization|social|family";

        string headingPattern = string.Join("|",
            headings.Select(h => System.Text.RegularExpressions.Regex.Escape(h)));

        var match = System.Text.RegularExpressions.Regex.Match(text,
            $@"(?i)(?:{headingPattern})\s*:?\s*\r?\n?(.*?)(?=\r?\n\s*(?:{stopPattern})\s*:|\Z)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase |
            System.Text.RegularExpressions.RegexOptions.Singleline);

        if (!match.Success) return "";

        string result = match.Groups[1].Value.Trim();
        if (result.Length > 2000) result = result[..2000].Trim() + "...";
        return result;
    }



}
