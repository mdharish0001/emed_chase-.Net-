using emedl_chase.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Linq;

namespace emedl_chase.Helper
{
    public class ECWFHIRApiCaller
    {
        public ECWFHIRApiCaller() { }

        public static async Task<string> GetCcdaXmlByEncounter(string bearer, string patientId, string encounterId, string name, string dos)
        {
            using var client = new HttpClient();

            // ECW: fetch all DocumentReferences for this patient, then filter by encounter
            var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientId}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"DocumentReference status: {(int)response.StatusCode}");
            Console.WriteLine($"DocumentReference body: {json}");

            if (!response.IsSuccessStatusCode) return null;

            var bundle = System.Text.Json.JsonSerializer.Deserialize<DocumentReferenceBundle>(json);
            if (bundle?.entry == null || !bundle.entry.Any()) return null;

            // Find the DocumentReference whose context.encounter matches our encounterId
            var matchedEntry = bundle.entry.FirstOrDefault(e =>
            {
                var encounterRefs = e.resource?.context?.encounter;
                if (encounterRefs == null) return false;

                return encounterRefs.Any(er =>
                    er.reference != null && (
                        er.reference == $"Encounter/{encounterId}" ||
                        er.reference == encounterId ||
                        er.reference.EndsWith($"/{encounterId}")
                    ));
            });

            // Fallback: match by date if encounter reference not found
            if (matchedEntry == null && !string.IsNullOrEmpty(dos))
            {
                matchedEntry = bundle.entry.FirstOrDefault(e =>
                {
                    string docDate = e.resource?.context?.period?.start;
                    if (string.IsNullOrEmpty(docDate)) return false;               
                    return docDate.StartsWith(dos);
                });
            }

            Console.WriteLine(matchedEntry != null
                ? $"Matched DocumentReference for encounter {encounterId}"
                : "No matching DocumentReference found for this encounter");

            if (matchedEntry == null) return null;

            // Get the Binary URL from the attachment
            var attachmentUrl = matchedEntry.resource?.content
                ?.Select(c => c.attachment?.url)
                ?.FirstOrDefault(u => !string.IsNullOrEmpty(u));

            Console.WriteLine($"Attachment URL: {attachmentUrl}");
            if (string.IsNullOrEmpty(attachmentUrl)) return null;

            var binaryId = attachmentUrl.Split('/').LastOrDefault();
            Console.WriteLine($"Binary ID: {binaryId}");
            if (string.IsNullOrEmpty(binaryId)) return null;

            return await GetCcdaXml(bearer, binaryId, name, dos);
        }
        public static async Task<string> GetCcdaXmlByEncounter_withoutDate(string bearer, string patientId, string encounterId, string name, string dos)
        {
            using var client = new HttpClient();

            // Option A: DocumentReference filtered by encounter (preferred)
            var docRefUrl = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference" +  $"?patient={patientId}&encounter={encounterId}&type=34133-9";

            var request = new HttpRequestMessage(HttpMethod.Get, docRefUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var bundle = System.Text.Json.JsonSerializer.Deserialize<DocumentReferenceBundle>(json);

            // Get the binary ID from the DocumentReference attachment
            var binaryId = bundle?.entry?.FirstOrDefault()
                                 ?.resource?.content?.FirstOrDefault()
                                 ?.attachment?.url
                                 ?.Split('/').LastOrDefault();

            if (string.IsNullOrEmpty(binaryId))
                return null;

            // Now fetch the actual Binary CCDA XML
            return await GetCcdaXml(bearer, binaryId, name, dos);
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

                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

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



        // ============================================================
        // 1. FIXED: DocumentReference API with Date Filtering
        // ============================================================
        public static async Task<List<DocumentReferenceDTO>> DocumentReferenceAPI(
            string accessToken, string patientId, string dos = null)
        {
            var client = new HttpClient();
            var url = $"https://fhir4.eclinicalworks.com/fhir/r4/JFABDD/DocumentReference?patient={patientId}";
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Status: {(int)response.StatusCode}\nResponse: {responseText}");

            dynamic bundle = JsonConvert.DeserializeObject(responseText);
            var result = new List<DocumentReferenceDTO>();

            if (bundle?.entry == null) return result;

            foreach (var entry in bundle.entry)
            {
                // Safely parse the date string — dynamic JSON won't have a .Date property
                string rawDate = entry.resource?.date?.ToString();
                if (string.IsNullOrEmpty(rawDate)) continue;

                // Parse: FHIR dates can be "2024-03-15T10:30:00Z" or just "2024-03-15"
                if (!DateTime.TryParse(rawDate, null,
                    System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsedDate))
                    continue;

                string docDateStr = parsedDate.ToString("yyyy-MM-dd");

                // Filter by DOS if provided
                if (!string.IsNullOrEmpty(dos) && docDateStr != dos)
                    continue;

                result.Add(new DocumentReferenceDTO
                {
                    Id = entry.resource.id?.ToString(),
                    Description = entry.resource.description?.ToString(),
                    Status = entry.resource.status?.ToString(),
                    Date = docDateStr,
                    // Also grab the raw CCDA content if present
                    RawContent = ExtractDocumentContent(entry.resource)
                });
            }

            return result;
        }

        // Helper: pull base64-encoded CCDA out of the DocumentReference content array
        private static string ExtractDocumentContent(dynamic resource)
        {
            try
            {
                foreach (var content in resource.content)
                {
                    string contentType = content.attachment?.contentType?.ToString() ?? "";
                    if (contentType.Contains("xml") || contentType.Contains("cda"))
                    {
                        string b64 = content.attachment?.data?.ToString();
                        if (!string.IsNullOrEmpty(b64))
                            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(b64));

                        // Some ECW responses use a URL instead of inline data
                        string url = content.attachment?.url?.ToString();
                        // fetch separately if needed — return url as marker
                        if (!string.IsNullOrEmpty(url)) return $"URL:{url}";
                    }
                }
            }
            catch { /* content node absent */ }
            return null;
        }


        // ============================================================
        // 2. FIXED: Encounter API with correct date parsing
        // ============================================================
        public static List<EncounterDTO.finalresponse> ParseEncounters( dynamic bundle, string dos = null)
        {
            var result = new List<EncounterDTO.finalresponse>();

            if (bundle?.entry == null) return result;

            foreach (var e in bundle.entry)
            {
                // FIX: parse the string, don't call .Date on a JToken
                string rawStart = e.resource?.period?.start?.ToString();
                if (string.IsNullOrEmpty(rawStart)) continue;

                if (!DateTime.TryParse(rawStart, null,
                    System.Globalization.DateTimeStyles.RoundtripKind, out DateTime startDate))
                    continue;

                string encDateStr = startDate.ToString("yyyy-MM-dd");

                // Filter
                if (!string.IsNullOrEmpty(dos) && encDateStr != dos)
                    continue;

                result.Add(new EncounterDTO.finalresponse
                {
                    encounterid = e.resource.id?.ToString(),
                    dos = encDateStr,
                    encounternote = e.resource.reasonCode?[0]?.text?.ToString()
                });
            }

            return result;
        }

    }

    public class CcdaParser
    {
        private static readonly Dictionary<string, string> SectionOids = new()
        {
            ["Chief Complaint"] = "2.16.840.1.113883.10.20.22.2.13",
            ["HPI"] = "1.3.6.1.4.1.19376.1.5.3.1.3.4",
            ["ROS"] = "1.3.6.1.4.1.19376.1.5.3.1.3.18",
            ["Medications"] = "2.16.840.1.113883.10.20.22.2.1.1",
            ["Medical History"] = "2.16.840.1.113883.10.20.22.2.5.1",
            ["Vitals"] = "2.16.840.1.113883.10.20.22.2.4.1",
            ["Assessment"] = "2.16.840.1.113883.10.20.22.2.8",
            ["Plan"] = "2.16.840.1.113883.10.20.22.2.10",
            ["Clinical Notes"] = "2.16.840.1.113883.10.20.22.2.65",
        };

        public static ClinicalNoteNew Parse(string ccdaXml)
        {
            var note = new ClinicalNoteNew();
            if (string.IsNullOrWhiteSpace(ccdaXml)) return note;

            XDocument doc;
            try { doc = XDocument.Parse(ccdaXml); }
            catch { return note; }

            XNamespace ns = "urn:hl7-org:v3";

            // Patient name
            var patientName = doc.Descendants(ns + "patient").FirstOrDefault()
                ?.Descendants(ns + "name").FirstOrDefault();
            if (patientName != null)
            {
                string given = patientName.Element(ns + "given")?.Value ?? "";
                string family = patientName.Element(ns + "family")?.Value ?? "";
                note.PatientName = $"{given} {family}".Trim();
            }

            // Date of service — from encompassingEncounter or effectiveTime
            string dos = doc.Descendants(ns + "encompassingEncounter")
                .FirstOrDefault()
                ?.Descendants(ns + "effectiveTime")
                .FirstOrDefault()
                ?.Attribute("value")?.Value
                ?? doc.Root?.Element(ns + "effectiveTime")?.Attribute("value")?.Value;

            if (!string.IsNullOrEmpty(dos) && dos.Length >= 8)
            {
                note.DateOfService = $"{dos[..4]}-{dos[4..6]}-{dos[6..8]}";
            }

            // Provider
            note.Provider = doc.Descendants(ns + "assignedPerson")
                .FirstOrDefault()
                ?.Descendants(ns + "name").FirstOrDefault()
                ?.Value?.Trim();

            // Extract each clinical section by OID
            foreach (var section in doc.Descendants(ns + "section"))
            {
                string oid = section.Elements(ns + "templateId")
                    .Select(t => t.Attribute("root")?.Value)
                    .FirstOrDefault(o => o != null && SectionOids.ContainsValue(o));

                if (oid == null) continue;

                string sectionName = SectionOids.FirstOrDefault(kv => kv.Value == oid).Key;
                string sectionTitle = section.Element(ns + "title")?.Value ?? sectionName;
                // Get the human-readable text block (strips XML tags)
                string sectionText = ExtractNarrativeText(section, ns);

                if (!note.Sections.ContainsKey(sectionName))
                    note.Sections[sectionName] = new List<string>();

                note.Sections[sectionName].Add(sectionText);
            }

            return note;
        }

        private static string ExtractNarrativeText(XElement section, XNamespace ns)
        {
            var textEl = section.Element(ns + "text");
            if (textEl == null) return "";

            // Concatenate all text nodes, preserving paragraph breaks
            var sb = new System.Text.StringBuilder();
            foreach (var node in textEl.DescendantNodes())
            {
                if (node is XText xt)
                {
                    string val = xt.Value.Trim();
                    if (!string.IsNullOrEmpty(val)) sb.AppendLine(val);
                }
                else if (node is XElement el &&
                    (el.Name.LocalName == "br" || el.Name.LocalName == "paragraph"))
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString().Trim();
        }


    }


    public class ClinicalNoteNew
    {
        public string PatientName { get; set; }
        public string DateOfService { get; set; }
        public string Provider { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string AccountNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Dictionary<string, List<string>> Sections { get; set; } = new();
    }
}
