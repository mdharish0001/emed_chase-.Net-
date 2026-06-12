using emedl_chase.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;

namespace emedl_chase.Helper
{
    public class ECWFHIRApi
    {
        public static async Task<List<PatientDTO.fhirid>> PatientDemoAPI(string accesstoken, string patientname)
        {
            var client = new HttpClient();

            var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient?name={patientname}";
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

                //Console.WriteLine($"Content Type: {get_fhir_id.resourceType}");

                //Console.WriteLine($"Base URL: {baseurl}");
                //Console.WriteLine($"Response Body: {responseBody}");

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

        public static async Task<List<Alergy>> AllergyAPI(string accessToken, string patientId)
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

                List<Alergy> allergyList = new();

                if (allergyBundle?.entry != null)
                {
                    allergyList = allergyBundle.entry
                        .Where(x => x.resource != null)
                        .Select(x => new Alergy
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
                return new List<Alergy>();
            }

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

        //public static async Task<List<EncounterDTO>> EncounterAPI(string accesstoken, string patientname, string dos)
        //{
        //    var client = new HttpClient();

        //    var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientname}";
        //    var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        //    // Set required headers
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        //    List<EncounterDTO.finalresponse> encounterapi = new List<EncounterDTO.finalresponse>();
        //    try
        //    {
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();  // throws if not 200-299
        //        var responseBody = await response.Content.ReadAsStringAsync();

        //        var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<EncounterDTO.Rootobject>(responseBody);

        //        encounterapi = get_fhir_id.entry.Where(a => a.resource?.reasonCode != null && a.resource?.period?.start.ToString("yyyy-MM-dd") == dos).SelectMany(e => e.resource.reasonCode
        //            .Where(n => !string.IsNullOrEmpty(n.text)).Select(n => new EncounterDTO.finalresponse
        //            {
        //                encounternote = n.text,
        //                dos = e.resource.period.start.ToString("yyyy-MM-dd"),
        //                encounterid = e.resource.id,
        //            })).ToList();

        //        return encounterapi;
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"Request failed: {ex.Message}");
        //        return new List<EncounterDTO>();
        //    }
        //}

        public static async Task<List<ConditionDTO>>ConditionAPI(string accessToken,string patientId)
        {
            var client = new HttpClient();

            var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Condition?patient={patientId}";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var json = await client.GetStringAsync(url);

            dynamic bundle = JsonConvert.DeserializeObject(json);

            var result = new List<ConditionDTO>();

            if (bundle?.entry != null)
            {
                foreach (var entry in bundle.entry)
                {
                    var resource = entry?.resource;

                    if (resource == null) continue;

                    result.Add(new ConditionDTO
                    {
                        Id = resource?.id,
                        Name = resource?.code?.text,
                        ClinicalStatus = resource?.clinicalStatus?.coding?[0]?.code,
                        VerificationStatus = resource?.verificationStatus?.coding?[0]?.code,
                        RecordedDate = resource?.recordedDate
                    });
                }
            }

            return result;
        }

        public static async Task<List<ObservationDTO>>ObservationAPI(string accessToken,string patientId)
        {
            var client = new HttpClient();

            var url =  $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Observation?patient={patientId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Set required headers
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue( "Bearer", accessToken);

            //var json =   await client.GetStringAsync(url);

            // dynamic bundle = JsonConvert.DeserializeObject(json);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();
            var get_fhir_id = System.Text.Json.JsonSerializer.Deserialize<ObservationDTO>(responseBody);

            var list =   new List<ObservationDTO>();

            //foreach (var entry in get_fhir_id.entry)
            //{
            //    var resource = entry.resource;

            //    list.Add(
            //        new ObservationDTO
            //        {
            //            Name =
            //                resource.code.text,

            //            Value =
            //                resource.valueQuantity?.value
            //                    ?.ToString(),

            //            Unit =
            //                resource.valueQuantity?.unit,

            //            EffectiveDate =
            //                resource.effectiveDateTime
            //        });
            //}

            return list;
        }

        public static async Task<List<ProcedureDTO>>ProcedureAPI(string accessToken,string patientId)
        {
            var client = new HttpClient();

            var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Procedure?patient={patientId}";

            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue(  "Bearer", accessToken);

            var json =   await client.GetStringAsync(url);

            dynamic bundle =JsonConvert.DeserializeObject(json);

            var result = new List<ProcedureDTO>();

            foreach (var entry in bundle.entry)
            {
                result.Add(
                    new ProcedureDTO
                    {
                        ProcedureName = entry.resource.code.text,

                        Status =  entry.resource.status,

                        PerformedDate =  entry.resource.performedDateTime
                    });
            }

            return result;
        }

        public static async Task<List<DiagnosticReportDTO>>DiagnosticReportAPI(string accessToken,string patientId)
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

        public static async Task<List<ImmunizationDTO>>ImmunizationAPI(string accessToken,string patientId)
        {
            var client = new HttpClient();

            var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Immunization?patient={patientId}";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            var json =
                await client.GetStringAsync(url);

            dynamic bundle =
                JsonConvert.DeserializeObject(json);

            var result =
                new List<ImmunizationDTO>();

            foreach (var entry in bundle.entry)
            {
                result.Add(
                    new ImmunizationDTO
                    {
                        Vaccine =
                            entry.resource.vaccineCode.text,

                        Status =
                            entry.resource.status,

                        Date =
                            entry.resource.occurrenceDateTime
                    });
            }

            return result;
        }

        public static async Task<List<DocumentReferenceDTO>>DocumentReferenceAPI( string accessToken, string patientId)
        {
            var client = new HttpClient();

            var url =
                $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientId}";

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    accessToken);

            var response =
                await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content.ReadAsStringAsync();

            dynamic bundle =
                JsonConvert.DeserializeObject(json);

            var result =
                new List<DocumentReferenceDTO>();

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
    }
}
