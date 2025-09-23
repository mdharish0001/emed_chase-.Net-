using emedl_chase.Model;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

public class FhirApiCaller
{
    public static async Task<List<Patient.fhirid>> CallApiforPatientDemo(string accesstoken,string patientname)
    {
        var client = new HttpClient();

        var baseurl =$"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient?name={patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get,baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",accesstoken);

        List<Patient.fhirid> peoplefhir =new List<Patient.fhirid>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id=JsonSerializer.Deserialize<Patient.Rootobject>(responseBody);
            if (get_fhir_id.entry != null)
            {
                peoplefhir = get_fhir_id.entry
          .Where(e => e.resource?.name != null && !string.IsNullOrEmpty(e.resource.id) && e.resource.meta != null)
          .SelectMany(e => e.resource.name
              .Where(n => !string.IsNullOrEmpty(n.text))
              .Select(n => new Patient.fhirid
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
            return new List<Patient.fhirid>();
        }
    }


    public static async Task<List<Encounter.finalresponse>> CallApiForEncounter(string accesstoken, string patientname, string dos)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Encounter?patient={patientname}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        List<Encounter.finalresponse> encounterapi = new List<Encounter.finalresponse>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id = JsonSerializer.Deserialize<Encounter.Rootobject>(responseBody);

            encounterapi = get_fhir_id.entry.Where(a => a.resource?.reasonCode != null && a.resource?.period?.start.ToString("yyyy-MM-dd") == dos).SelectMany(e => e.resource.reasonCode
                .Where(n => !string.IsNullOrEmpty(n.text)).Select(n => new Encounter.finalresponse
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
            return new List<Encounter.finalresponse>();
        }
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

            var get_fhir_id = JsonSerializer.Deserialize<DocRefresh.Rootobject>(responseBody);


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

            var get_fhir_id = JsonSerializer.Deserialize<Binary.Rootobject>(responseBody);


            var encounterapi = get_fhir_id.data.ToString();
       
            return encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
    }


    public static async Task<DocRefreshEncounterWithPatient.finalresonse> CallApiForDocrefreshEncounterwithPatient(string accesstoken, string patientid,string encounterid)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?encounter={encounterid}&patient={patientid}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        try
        {
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();  // throws if not 200-299

            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id = JsonSerializer.Deserialize<DocRefreshEncounterWithPatient.Rootobject>(responseBody);


            var encounterapi = get_fhir_id.entry.Where(a => a.resource?.data != null).Select(n=> new DocRefreshEncounterWithPatient.finalresonse
            {
                encounterxmldata=n.resource.data.ToString(),
                encounterdate=n.resource?.context?.period?.start.ToString(),
            }).FirstOrDefault();
            //    .Select(n => new DocRefreshEncounterWithPatient.finalresonse
            //{
            //   encounterdate=n.resource.context.period.start.ToString(),
            //   encounterid=n.resource.id,
            //   encountereason=n.resource.type.text,
            //   encounterxmldata = n.resource.data?.ToString()

            //}).FirstOrDefault();



            return encounterapi;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
    }


    public static async Task<List<Patient.fhirid>> CallApiforwithFhirPatientDemo(string accesstoken, string fhiriid)
    {
        var client = new HttpClient();

        var baseurl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/Patient/{fhiriid}";
        var request = new HttpRequestMessage(HttpMethod.Get, baseurl);

        // Set required headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+fhir"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken);

        List<Patient.fhirid> peoplefhir = new List<Patient.fhirid>();
        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();  // throws if not 200-299
            var responseBody = await response.Content.ReadAsStringAsync();

            var get_fhir_id = JsonSerializer.Deserialize<Patient.Rootobject>(responseBody);
            if (get_fhir_id.entry != null)
            {
                peoplefhir = get_fhir_id.entry
          .Where(e => e.resource?.name != null && !string.IsNullOrEmpty(e.resource.id) && e.resource.meta != null)
          .SelectMany(e => e.resource.name
              .Where(n => !string.IsNullOrEmpty(n.text))
              .Select(n => new Patient.fhirid
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
            return new List<Patient.fhirid>();
        }
    }

}
