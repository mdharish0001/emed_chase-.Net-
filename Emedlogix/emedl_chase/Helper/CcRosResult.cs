// ═══════════════════════════════════════════════════════════════════════
//  FhirCcRosHelper — Fixed CC + ROS extraction
//
//  ROOT CAUSE: DocumentReference has MULTIPLE content items.
//  The first item (5329 bytes) is the H&P table — starts with "HPI (History..."
//  The CC and ROS are in a DIFFERENT content item (the full encounter note).
//
//  FIX: Check ALL content items, search each for "Chief Complaint" and "ROS".
//  Also: keep HTML before stripping so we can search for bold labels.
// ═══════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace emedl_chase.Helper
{
    public static class FhirCcRosHelper
    {
        private static readonly HttpClient _http = new HttpClient();

        // ── PUBLIC: Get CC and ROS from encounter note ────────────────
        public static async Task<(string CC, string ROS)> GetCcRosAsync(
            string bearer, string baseUrl, string patientId, string encounterId)
        {
            string cc = "", ros = "";

            // Strategy 1: CC from Encounter.reasonCode (structured, reliable)
            cc = await GetCcFromEncounterAsync(bearer, baseUrl, encounterId);
            Console.WriteLine($"[CcRos] Strategy1 CC from reasonCode: '{cc}'");

            // Strategy 2: CC + ROS from DocumentReference (all content items)
            var (ccDoc, rosDoc) = await GetCcRosFromAllDocumentContentAsync(
                bearer, baseUrl, patientId, encounterId);

            if (string.IsNullOrEmpty(cc) && !string.IsNullOrEmpty(ccDoc))
                cc = ccDoc;
            if (!string.IsNullOrEmpty(rosDoc))
                ros = rosDoc;

            Console.WriteLine($"[CcRos] Strategy2 CC: '{cc}' ROS: {ros?.Length ?? 0} chars");

            return (cc ?? "", ros ?? "");
        }

        // ── Strategy 1: GET /Encounter/{id} → reasonCode[].text ──────
        private static async Task<string> GetCcFromEncounterAsync(
            string bearer, string baseUrl, string encounterId)
        {
            if (string.IsNullOrEmpty(encounterId)) return "";
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get,
                    $"{baseUrl}/Encounter/{encounterId}");
                req.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/fhir+json"));
                req.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);

                var resp = await _http.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return "";

                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                var root = doc.RootElement;

                if (root.TryGetProperty("reasonCode", out var codes))
                    foreach (var rc in codes.EnumerateArray())
                    {
                        if (rc.TryGetProperty("text", out var t) &&
                            !string.IsNullOrWhiteSpace(t.GetString()))
                        {
                            string val = t.GetString()!.Trim();
                            if (!IsAdminBillingText(val)) return val;
                            Console.WriteLine($"[CcRos] Skipping admin reasonCode: {val[..Math.Min(60, val.Length)]}");
                        }
                        if (rc.TryGetProperty("coding", out var codings))
                            foreach (var c in codings.EnumerateArray())
                                if (c.TryGetProperty("display", out var d) &&
                                    !string.IsNullOrWhiteSpace(d.GetString()))
                                {
                                    string val = d.GetString()!.Trim();
                                    if (!IsAdminBillingText(val)) return val;
                                }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CcRos] GetCcFromEncounter error: {ex.Message}");
            }
            return "";
        }

        // ── Strategy 2: DocumentReference → check ALL content items ──
        private static async Task<(string CC, string ROS)>
            GetCcRosFromAllDocumentContentAsync(
                string bearer, string baseUrl,
                string patientId, string encounterId)
        {
            string cc = "", ros = "";

            try
            {
                var url = $"{baseUrl}/DocumentReference" +
                          $"?encounter={encounterId}&patient={patientId}";

                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/fhir+json"));
                req.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);

                var resp = await _http.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return ("", "");

                var json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"[CcRos] DocRef bundle: {json.Length} chars");

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("entry", out var entries)) return ("", "");

                foreach (var entry in entries.EnumerateArray())
                {
                    if (!entry.TryGetProperty("resource", out var resource)) continue;
                    if (!resource.TryGetProperty("content", out var contentArr)) continue;

                    // ── Check EVERY content item, not just the first ──
                    foreach (var contentItem in contentArr.EnumerateArray())
                    {
                        if (!contentItem.TryGetProperty("attachment", out var att)) continue;

                        string text = "";

                        // Inline base64 data
                        if (att.TryGetProperty("data", out var dataEl))
                        {
                            string b64 = dataEl.GetString() ?? "";
                            if (!string.IsNullOrEmpty(b64))
                            {
                                try
                                {
                                    byte[] bytes = Convert.FromBase64String(b64);
                                    text = Encoding.UTF8.GetString(bytes)
                                               .TrimStart('\uFEFF', '\u200B');
                                }
                                catch { continue; }
                            }
                        }
                        // URL reference
                        else if (att.TryGetProperty("url", out var urlEl))
                        {
                            string binaryUrl = urlEl.GetString() ?? "";
                            if (!string.IsNullOrEmpty(binaryUrl))
                                text = await FetchBinaryTextAsync(bearer, binaryUrl);
                        }

                        if (string.IsNullOrEmpty(text)) continue;

                        Console.WriteLine($"[CcRos] Content item {text.Length} chars, " +
                            $"starts: {text[..Math.Min(80, text.Length)]}");

                        // Search this content item for CC and ROS
                        string itemCC = ExtractCcFromText(text);
                        string itemROS = ExtractRosFromText(text);

                        if (string.IsNullOrEmpty(cc) && !string.IsNullOrEmpty(itemCC))
                            cc = itemCC;
                        if (string.IsNullOrEmpty(ros) && !string.IsNullOrEmpty(itemROS))
                            ros = itemROS;

                        // If we found both, no need to continue
                        if (!string.IsNullOrEmpty(cc) && !string.IsNullOrEmpty(ros))
                            break;
                    }

                    if (!string.IsNullOrEmpty(cc) && !string.IsNullOrEmpty(ros))
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CcRos] GetAllDocumentContent error: {ex.Message}");
            }

            return (cc, ros);
        }

        private static async Task<string> FetchBinaryTextAsync(
            string bearer, string url)
        {
            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearer);
                var resp = await _http.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return "";

                string raw = await resp.Content.ReadAsStringAsync();
                // FHIR Binary resource
                if (raw.TrimStart().StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(raw);
                    if (doc.RootElement.TryGetProperty("data", out var d))
                    {
                        byte[] bytes = Convert.FromBase64String(d.GetString() ?? "");
                        return Encoding.UTF8.GetString(bytes);
                    }
                }
                return raw;
            }
            catch { return ""; }
        }

        // ── Extract Chief Complaint from note text ────────────────────
        // ECW note formats:
        //   "Chief Complaints:\n  1. PP check."
        //   "<b>Chief Complaints:</b><br>1. PP check."
        //   "CHIEF COMPLAINTS\n1. PP check."
        private static string ExtractCcFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            // Preserve HTML structure before stripping — use it to find labels
            // ECW uses <b>Label:</b> pattern for section headers
            // Replace </b> and </br> with newline so section breaks survive tag stripping
            string processed = text
                .Replace("</b>", "\n")
                .Replace("</B>", "\n")
                .Replace("<br/>", "\n")
                .Replace("<br />", "\n")
                .Replace("<BR/>", "\n")
                .Replace("<br>", "\n")
                .Replace("</tr>", "\n")
                .Replace("</td>", " | ");

            // Strip remaining HTML tags
            processed = Regex.Replace(processed, @"<[^>]+>", "");
            // Normalize whitespace but keep newlines
            processed = Regex.Replace(processed, @"[ \t]{2,}", " ");

            string[] ccLabels = {
                "chief complaints", "chief complaint",
                "reason for visit", "cc:", "reason for encounter"
            };

            foreach (var label in ccLabels)
            {
                var match = Regex.Match(processed,
                    $@"(?i){Regex.Escape(label)}\s*:?\s*\r?\n?(.*?)" +
                    $@"(?=\r?\n\s*(?:hpi|history|ros|review|medic|allerg|vital|exam|" +
                    $@"objective|assessment|plan|physical)\b|\Z)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                {
                    string result = CleanSectionText(match.Groups[1].Value);
                    if (!string.IsNullOrEmpty(result) && !IsAdminBillingText(result))
                    {
                        Console.WriteLine($"[CcRos] Found CC via '{label}': {result[..Math.Min(80, result.Length)]}");
                        return result;
                    }
                }
            }

            return "";
        }

        // ── Extract Review of Systems from note text ──────────────────
        // ECW ROS format:
        //   "ROS:\n  General / Constitutional:\n  Chills denies. Fatigue denies..."
        //
        // STRICT validation: only accept content that:
        // 1. Has a clear "Review of Systems:" or "ROS:" label header
        // 2. Contains organ-system keywords (General:, Cardiovascular:, etc.)
        // This prevents H&P narrative text from being misidentified as ROS
        private static string ExtractRosFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            string processed = text
                .Replace("</b>", "\n").Replace("</B>", "\n")
                .Replace("<br/>", "\n").Replace("<br />", "\n")
                .Replace("<br>", "\n").Replace("<BR/>", "\n")
                .Replace("</tr>", "\n").Replace("</td>", " | ");

            processed = Regex.Replace(processed, @"<[^>]+>", "");
            processed = Regex.Replace(processed, @"[ \t]{2,}", " ");

            // ROS must start with an explicit ROS header label
            // Do NOT use loose patterns like "ros\n" that match mid-paragraph
            string[] rosLabels = {
                "review of systems", "ros:"
            };

            // Organ system keywords — valid ROS must contain at least one
            string[] organSystems = {
                "general:", "general /", "constitutional:",
                "cardiovascular:", "respiratory:", "neurological:",
                "musculoskeletal:", "gastrointestinal:", "genitourinary:",
                "ophthalmologic:", "ent:", "psychiatric:", "skin:",
                "endocrine:", "hematologic:", "allergic:", "immunologic:",
                "head, eyes", "eyes:", "ears:", "nose:", "throat:"
            };

            foreach (var label in rosLabels)
            {
                var match = Regex.Match(processed,
                    $@"(?i){Regex.Escape(label)}\s*:?\s*
?
?(.*?)" +
                    $@"(?=
?
\s*(?:medic|allerg|vital|physical exam|physical find|" +
                    $@"objective|exam|assessment|plan|screening|immun|hpi|history of)|\Z)",
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (!match.Success) continue;

                string result = CleanSectionText(match.Groups[1].Value);

                // Must be substantial but not too long (HPI narratives are 2000+ chars)
                if (result.Length < 20 || result.Length > 3000) continue;

                // Must contain at least one organ system keyword to be valid ROS
                bool hasOrganSystem = organSystems.Any(os =>
                    result.IndexOf(os, StringComparison.OrdinalIgnoreCase) >= 0);

                if (!hasOrganSystem)
                {
                    Console.WriteLine($"[CcRos] Rejected false ROS (no organ system keywords): {result[..Math.Min(80, result.Length)]}");
                    continue;
                }

                Console.WriteLine($"[CcRos] Found valid ROS via '{label}': {result.Length} chars");
                return result;
            }

            return "";
        }

        // Detects ECW administrative/billing text that ECW sometimes puts in reasonCode
        // instead of a clinical chief complaint.
        // Returns true if the text looks like billing info (should be skipped as CC).
        private static bool IsAdminBillingText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;

            // Count how many billing indicators are present
            string[] billingIndicators = {
                "PT Type", "PT Balance", "Ins -", "Eff From",
                "Ded Remaining", "Copay", "Deductible", "Health Plan",
                "term date", "Coverage -", "Co-pay", "$ 0", "$ 25",
                "No term", "will be covered", "WL F/U", "WL-F/U",
            };

            int count = billingIndicators.Count(indicator =>
                text.IndexOf(indicator, StringComparison.OrdinalIgnoreCase) >= 0);

            // If 2+ billing indicators → definitely admin text
            if (count >= 2) return true;

            // Also reject if text starts with known admin prefixes
            string[] adminPrefixes = { "PT Type", "Ins -", "Balance" };
            if (adminPrefixes.Any(p =>
                text.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return true;

            return false;
        }

        private static string CleanSectionText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            // Remove leading numbers/bullets, collapse spaces
            text = Regex.Replace(text, @"^\s*\d+\.\s*", "", RegexOptions.Multiline);
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
            text = text.Trim();
            if (text.Length > 3000) text = text[..3000].Trim() + "...";
            return text;
        }

        // ── SYNC version called from controller ───────────────────────
        // Used when the encounter note XML is already fetched
        public static (string CC, string ROS) ExtractCcRosFromEncounterXml(
            string encounterContent)
        {
            if (string.IsNullOrWhiteSpace(encounterContent)) return ("", "");

            encounterContent = encounterContent.TrimStart('\uFEFF', '\u200B').Trim();

            Console.WriteLine($"[CcRos] ExtractCcRos content type: " +
                $"{(encounterContent.StartsWith("<") ? "XML" : encounterContent.StartsWith("{") ? "JSON" : "TEXT")}" +
                $", length={encounterContent.Length}");

            string cc = ExtractCcFromText(encounterContent);
            string ros = ExtractRosFromText(encounterContent);

            // If XML, also try structured section approach
            if (string.IsNullOrEmpty(cc) || string.IsNullOrEmpty(ros))
            {
                if (encounterContent.TrimStart().StartsWith("<"))
                {
                    var (xmlCc, xmlRos) = ExtractFromCdaXml(encounterContent);
                    if (string.IsNullOrEmpty(cc) && !string.IsNullOrEmpty(xmlCc)) cc = xmlCc;
                    if (string.IsNullOrEmpty(ros) && !string.IsNullOrEmpty(xmlRos)) ros = xmlRos;
                }
            }

            return (cc, ros);
        }

        private static (string CC, string ROS) ExtractFromCdaXml(string xml)
        {
            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                var ns = new System.Xml.XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("cda", "urn:hl7-org:v3");

                string cc = ExtractXmlSection(doc, ns,
                    new[] { "Chief Complaints", "Chief Complaint", "CC", "Reason for Visit" });
                string ros = ExtractXmlSection(doc, ns,
                    new[] { "Review of Systems", "ROS", "Systems Review" });

                return (cc, ros);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CcRos] CDA parse error: {ex.Message}");
                return ("", "");
            }
        }

        private static string ExtractXmlSection(
            System.Xml.XmlDocument doc,
            System.Xml.XmlNamespaceManager ns,
            string[] titleVariants)
        {
            foreach (var title in titleVariants)
            {
                // With CDA namespace
                var section =
                    doc.SelectSingleNode($"//cda:section[cda:title='{title}']", ns) ??
                    doc.SelectSingleNode($"//cda:section[contains(cda:title,'{title}')]", ns) ??
                    // Without namespace (some ECW XML omits it)
                    doc.SelectSingleNode($"//section[title='{title}']") ??
                    doc.SelectSingleNode($"//section[contains(title,'{title}')]");

                if (section == null) continue;

                var textNode =
                    section.SelectSingleNode("cda:text", ns) ??
                    section.SelectSingleNode("text");
                if (textNode == null) continue;

                string raw = textNode.InnerText?.Trim() ?? "";
                if (!string.IsNullOrEmpty(raw)) return raw;
            }
            return "";
        }
    }
}