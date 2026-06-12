using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;

namespace emedl_chase.Model
{
    // ═══════════════════════════════════════════════════════════════
    //  DATA MODELS
    // ═══════════════════════════════════════════════════════════════

    public class ClinicalNoteNew
    {
        public string PatientName { get; set; }
        public string DateOfBirth { get; set; }
        public string Age { get; set; }
        public string AccountNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Provider { get; set; }
        public string DateOfService { get; set; }

        public Dictionary<string, List<string>> Sections { get; set; }
            = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
    }

    public class PatientInfo
    {
        public string Name { get; set; }
        public string DOB { get; set; }
        public string Age { get; set; }
        public string AccountNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Provider { get; set; }
        public string DateOfService { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    //  CCDAPARSER v4
    //  Strategy: read EVERYTHING in the XML.
    //  DOS filter: if DOS match exists in section → show only that.
    //              if no DOS match → show all available data.
    //  Nothing is ever dropped.
    // ═══════════════════════════════════════════════════════════════

    public static class CcdaParserNew
    {
        // ── Canonical section keys ──────────────────────────────────
        public const string SEC_HPI = "History and Physical Notes";
        public const string SEC_GEN_HISTORY = "General Medical History";
        public const string SEC_PMH = "PMH";
        public const string SEC_PMSFH = "PMSFH";
        public const string SEC_ALLERGY = "Allergy";
        public const string SEC_MEDICATIONS = "Medications";
        public const string SEC_IMMUNIZATION = "Immunizations";
        public const string SEC_CC = "Chief Complaint";
        public const string SEC_ROS = "Review of Systems";
        public const string SEC_VITALS = "Vital Signs";
        public const string SEC_EXAM = "Progress Notes";
        public const string SEC_RESULTS = "Results";
        public const string SEC_ASSESSMENT = "MDM or Assessment and Plan";
        public const string SEC_CLINICAL_NOTES = "Clinical Notes";
        public const string SEC_TREATMENT = "Treatment Plan";
        public const string SEC_PROCEDURES = "Procedures";
        public const string SEC_ENCOUNTERS = "Encounters";
        public const string SEC_INSURANCE = "Insurance Providers";
        public const string SEC_REFERRAL = "Reason For Referral";
        public const string SEC_CONSULT = "Consultation Notes";
        public const string SEC_GOALS = "Goals";
        public const string SEC_CONCERNS = "Health Concerns";
        public const string SEC_EQUIPMENT = "Medical Equipment";

        // ── Section map: LOINC code + title keywords → canonical key
        // LOINC is checked first (exact), then title keywords (contains).
        private static readonly List<(string Key, string[] LoincCodes, string[] TitleKeywords)> SectionMap
            = new()
        {
            (SEC_HPI,          new[]{"34117-2","10164-2","11492-6"},
             new[]{"history and physical","h&p","hpi","history of present illness","transition of care","subjective"}),

            (SEC_GEN_HISTORY,  new[]{"11348-0","11329-0"},
             new[]{"medical history","general history","medical (general) history","general medical history"}),

            (SEC_PMH,          new[]{"11450-4","69572-5"},
             new[]{"past medical history","pmh","problem list","problems","active problems"}),

            (SEC_PMSFH,        new[]{"29762-2","10157-6"},
             new[]{"social history","family history","pmsfh","social/family"}),

            (SEC_ALLERGY,      new[]{"48765-2"},
             new[]{"allerg","allergy","allergies","adverse reaction"}),

            (SEC_MEDICATIONS,  new[]{"10160-0","29549-3"},
             new[]{"medication","current medication","medication list","history of medication use"}),

            (SEC_IMMUNIZATION, new[]{"11369-6"},
             new[]{"immunization","vaccination","vaccine"}),

            (SEC_CC,           new[]{"46239-0","10154-3"},
             new[]{"chief complaint","reason for visit","reason for encounter"}),

            (SEC_ROS,          new[]{"10187-3"},
             new[]{"review of systems","ros","systems review"}),

            (SEC_VITALS,       new[]{"8716-3"},
             new[]{"vital sign","vitals","vital statistics"}),

            (SEC_EXAM,         new[]{"11506-3","29545-1"},
             new[]{"physical exam","physical examination","progress note","general examination","physical finding"}),

            (SEC_RESULTS,      new[]{"30954-2"},
             new[]{"result","lab result","laboratory","diagnostic result","labs"}),

            (SEC_ASSESSMENT,   new[]{"51847-2","51848-0"},
             new[]{"assessment","assessments","assessment and plan","mdm","diagnoses","impression"}),

            (SEC_TREATMENT,    new[]{"18776-5"},
             new[]{"treatment plan","plan of care","care plan","plan of treatment"}),

            (SEC_PROCEDURES,   new[]{"47519-4"},
             new[]{"procedure","procedures","procedures performed"}),

            (SEC_ENCOUNTERS,   new[]{"46240-8"},
             new[]{"encounter","encounters","encounter history","encounter list"}),

            (SEC_INSURANCE,    new[]{"48768-6"},
             new[]{"insurance","payer","coverage","insurance provider"}),

            (SEC_REFERRAL,     new[]{"42349-1"},
             new[]{"reason for referral","referral","reason for consultation"}),

            (SEC_CONSULT,      new[]{"11488-4"},
             new[]{"consultation request","consultation notes","consult note","consultation"}),

            (SEC_GOALS,        new[]{"61146-7"},
             new[]{"goal","goals section","care goal","patient goal"}),

            (SEC_CONCERNS,     new[]{"75310-3"},
             new[]{"health concern","health concerns"}),

            (SEC_EQUIPMENT,    new[]{"46264-8"},
             new[]{"medical equipment","equipment","durable medical"}),
        };

        // ── Resolve XML title/code → canonical key ──────────────────
        private static string ResolveCanonical(string title, string loinc)
        {
            string tl = (title ?? "").ToLowerInvariant().Trim();
            string lc = (loinc ?? "").Trim();

            foreach (var (key, loincCodes, keywords) in SectionMap)
            {
                if (!string.IsNullOrEmpty(lc) && loincCodes.Contains(lc)) return key;
                if (!string.IsNullOrEmpty(tl) && keywords.Any(k => tl.Contains(k))) return key;
            }
            // Unknown section: use raw title so nothing is lost
            return string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        }

        // ═══════════════════════════════════════════════════════════
        //  PUBLIC ENTRY POINT
        // ═══════════════════════════════════════════════════════════

        public static ClinicalNoteNew Parse(string ccdaXml, string dosFilter)
        {
            var note = new ClinicalNoteNew();
            var patient = ExtractPatient(ccdaXml);

            note.PatientName = patient.Name;
            note.DateOfBirth = patient.DOB;
            note.Age = patient.Age;
            note.AccountNumber = patient.AccountNumber;
            note.Phone = patient.Phone;
            note.Address = patient.Address;
            note.Provider = patient.Provider;
            note.DateOfService = !string.IsNullOrEmpty(dosFilter) ? dosFilter : patient.DateOfService;

            if (string.IsNullOrWhiteSpace(ccdaXml)) return note;

            // Strip BOM — ECW returns XML with UTF-8 BOM prefix
            ccdaXml = ccdaXml.TrimStart('\uFEFF', '\u200B').Trim();

            var doc = new XmlDocument();
            doc.LoadXml(ccdaXml);
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");

            DateTime? dos = ParseDate(dosFilter);

            Console.WriteLine("=== CCDA SECTIONS FOUND ===");
            foreach (XmlNode s in doc.SelectNodes("//cda:section", ns))
            {
                string tt = s.SelectSingleNode("cda:title", ns)?.InnerText;
                string cc = s.SelectSingleNode("cda:code", ns)?.Attributes?["code"]?.Value;
                Console.WriteLine($"  title='{tt}'  loinc='{cc}'  → {ResolveCanonical(tt, cc)}");
            }

            foreach (XmlNode section in doc.SelectNodes("//cda:section", ns))
            {
                // Skip nullFlavor=NI sections (explicitly empty)
                if ((section as XmlElement)?.GetAttribute("nullFlavor") == "NI") continue;

                string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();
                string loincCode = section.SelectSingleNode("cda:code", ns)?.Attributes?["code"]?.Value;
                string canonical = ResolveCanonical(rawTitle, loincCode);
                if (string.IsNullOrEmpty(canonical)) continue;

                var rows = ParseSection(canonical, section, ns, dos);
                if (rows == null || !rows.Any()) continue;

                if (!note.Sections.ContainsKey(canonical))
                    note.Sections[canonical] = new List<string>();
                note.Sections[canonical].AddRange(rows);
            }

            // Allergy key must always exist (shows "No Information" if empty)
            if (!note.Sections.ContainsKey(SEC_ALLERGY))
                note.Sections[SEC_ALLERGY] = new List<string>();


            // Extract Clinical Notes (CLINICAL_NOTE rows) from Assessment
            // into their own section — displayed separately after Lab Results
            if (note.Sections.TryGetValue(SEC_ASSESSMENT, out var assessRows))
            {
                var clinNotes = assessRows
                    .Where(r => r.StartsWith("CLINICAL_NOTE\n"))
                    .Select(r => r.Substring("CLINICAL_NOTE\n".Length).Trim())
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .ToList();
                if (clinNotes.Any())
                {
                    note.Sections[SEC_CLINICAL_NOTES] = clinNotes;
                    note.Sections[SEC_ASSESSMENT] = assessRows
                        .Where(r => !r.StartsWith("CLINICAL_NOTE\n"))
                        .ToList();
                }
            }

            Console.WriteLine("=== PARSED SECTIONS ===");
            foreach (var kvp in note.Sections)
                Console.WriteLine($"  {kvp.Key}: {kvp.Value.Count} rows");

            return note;
        }

        // ═══════════════════════════════════════════════════════════
        //  SECTION ROUTER
        // ═══════════════════════════════════════════════════════════

        private static List<string> ParseSection(
            string key, XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            return key switch
            {
                SEC_HPI => ParseHpi(section, ns, dos),
                SEC_VITALS => ParseVitals(section, ns, dos),
                SEC_EXAM => ParseExam(section, ns, dos),
                SEC_RESULTS => ParseResults(section, ns, dos),
                SEC_ASSESSMENT => ParseAssessment(section, ns, dos),
                SEC_ALLERGY => ParseAllergies(section, ns),
                SEC_PMH => ParseProblemList(section, ns),
                SEC_TREATMENT => ParseTreatmentPlan(section, ns, dos),
                SEC_PROCEDURES => ParseProcedures(section, ns, dos),
                SEC_ENCOUNTERS => ParseEncounters(section, ns, dos),
                SEC_MEDICATIONS => ParseNarrativeTable(section, ns),
                _ => ParseNarrativeText(section, ns),
            };
        }

        // ═══════════════════════════════════════════════════════════
        //  CORE HELPER: DOS-or-All filter
        //  Given a list of (date, rows) pairs:
        //    - If DOS given AND any entry matches → return only matching
        //    - Otherwise → return all entries
        // ═══════════════════════════════════════════════════════════

        // FilterOrAll — core date filter logic:
        //   1. Exact DOS match exists → return only those rows
        //   2. No exact match but ±7 day match exists → return closest date rows
        //      (handles: televisit 06/02 with exam done 06/01; vitals 06/08 for DOS 06/02)
        //   3. No match at all → return ALL rows (never drop data)
        private static List<string> FilterOrAll(
            List<(DateTime Date, List<string> Rows)> buckets, DateTime? dos,
            int windowDays = 7)
        {
            if (!buckets.Any()) return new List<string>();

            if (dos.HasValue)
            {
                // Pass 1: exact date match
                var exact = buckets.Where(b => b.Date.Date == dos.Value.Date)
                                   .SelectMany(b => b.Rows).ToList();
                if (exact.Any())
                {
                    Console.WriteLine($"  → Exact DOS match {dos.Value:MM/dd/yyyy}: {exact.Count} rows");
                    return exact;
                }

                // Pass 2: within ±windowDays
                var nearby = buckets
                    .Where(b => Math.Abs((b.Date.Date - dos.Value.Date).TotalDays) <= windowDays)
                    .OrderBy(b => Math.Abs((b.Date.Date - dos.Value.Date).TotalDays))
                    .ToList();

                if (nearby.Any())
                {
                    // Use only the closest date group
                    var closestDate = nearby.First().Date.Date;
                    var close = nearby.Where(b => b.Date.Date == closestDate)
                                      .SelectMany(b => b.Rows).ToList();
                    Console.WriteLine($"  → ±{windowDays}d match {closestDate:MM/dd/yyyy} (diff={(int)Math.Abs((closestDate - dos.Value.Date).TotalDays)}d): {close.Count} rows");
                    return close;
                }

                Console.WriteLine($"  → No match within ±{windowDays}d of {dos.Value:MM/dd/yyyy} — showing ALL {buckets.Count} date groups");
            }

            return buckets.SelectMany(b => b.Rows).ToList();
        }

        // ═══════════════════════════════════════════════════════════
        //  SECTION PARSERS
        // ═══════════════════════════════════════════════════════════

        // ── H&P ─────────────────────────────────────────────────────
        // ECW: <list><item><paragraph>Label<table><tbody><tr>
        //   td[0]=date  td[1]=category  td[2]=c/o  td[3-5]=empty  td[6]=notes(<paragraph>)
        private static List<string> ParseHpi(XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var buckets = new List<(DateTime, List<string>)>();
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode == null) return new List<string>();

            foreach (XmlNode item in textNode.SelectNodes(".//cda:item", ns))
            {
                foreach (XmlNode tr in item.SelectNodes(".//cda:tbody/cda:tr", ns))
                {
                    var tds = tr.SelectNodes("cda:td", ns);
                    if (tds.Count < 2) continue;

                    string dateStr = tds[0].InnerText.Trim();
                    string category = tds[1].InnerText.Trim();
                    string cc = tds.Count > 2 ? tds[2].InnerText.Trim() : "";

                    // Note text is in the last td.
                    // It may have multiple <paragraph> children — each is a separate
                    // visit note stored by ECW (no individual dates per paragraph).
                    // Show ALL paragraphs to avoid missing content.
                    XmlNode lastTd = tds[tds.Count - 1];

                    if (!ParseDate(dateStr, out var d)) d = DateTime.Today;

                    var rows = new List<string>();
                    if (!string.IsNullOrWhiteSpace(cc))
                        rows.Insert(0, $"CC | {cc}");

                    // Section heading
                    rows.Add($"{dateStr} | {category} | HEADER");

                    // Get all paragraphs — join their content preserving line breaks
                    var allParas = lastTd.SelectNodes("cda:paragraph", ns);
                    bool anyContent = false;

                    if (allParas.Count > 0)
                    {
                        // ECW adds paragraphs newest-last. When no DOS filter,
                        // reverse so the most recent visit note appears first.
                        var paraList = allParas.Cast<XmlNode>().ToList();
                        if (!dos.HasValue) paraList = Enumerable.Reverse(paraList).ToList();

                        bool firstPara = true;
                        foreach (XmlNode para in paraList)
                        {
                            string paraText = GetInnerTextWithSpaces(para).Trim();
                            if (string.IsNullOrWhiteSpace(paraText)) continue;

                            // Separate multiple visit notes with a divider
                            if (!firstPara)
                                rows.Add($"LINE | ---");
                            firstPara = false;

                            foreach (var line in paraText.Split(
                                new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string l = line.Trim();
                                if (!string.IsNullOrEmpty(l))
                                { rows.Add($"LINE | {l}"); anyContent = true; }
                            }
                        }
                    }

                    if (!anyContent)
                    {
                        // Fallback: raw InnerText
                        string raw = GetInnerTextWithSpaces(lastTd).Trim();
                        foreach (var line in raw.Split(
                            new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string l = line.Trim();
                            if (!string.IsNullOrEmpty(l)) rows.Add($"LINE | {l}");
                        }
                    }

                    buckets.Add((d, rows));
                }
            }

            return FilterOrAll(buckets, dos);
        }

        // ── VITALS ──────────────────────────────────────────────────
        // ECW: entry/organizer/component/observation
        // Each obs has its own effectiveTime.
        private static List<string> ParseVitals(XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            // Group observations by date
            var byDate = new Dictionary<DateTime, (string Sys, string Dia, List<string> Others)>();

            foreach (XmlNode obs in section.SelectNodes(".//cda:observation", ns))
            {
                string loinc = obs.SelectSingleNode("cda:code", ns)?.Attributes?["code"]?.Value ?? "";
                string codeName = obs.SelectSingleNode("cda:code", ns)?.Attributes?["displayName"]?.Value ?? "";
                var valNode = obs.SelectSingleNode("cda:value", ns);
                string value = valNode?.Attributes?["value"]?.Value
                               ?? valNode?.SelectSingleNode("cda:translation", ns)?.Attributes?["value"]?.Value
                               ?? valNode?.InnerText?.Trim() ?? "";
                string unit = CleanUnit(valNode?.Attributes?["unit"]?.Value ?? "");
                string rawDate = obs.SelectSingleNode("cda:effectiveTime", ns)?.Attributes?["value"]?.Value ?? "";

                if (string.IsNullOrWhiteSpace(codeName) && string.IsNullOrWhiteSpace(value)) continue;
                string dateStr = CcdaDateStr(rawDate);
                ParseDate(dateStr, out var d);

                if (!byDate.ContainsKey(d))
                    byDate[d] = ("", "", new List<string>());
                var entry = byDate[d];

                if (loinc == "8480-6") { byDate[d] = (value, entry.Dia, entry.Others); }
                else if (loinc == "8462-4") { byDate[d] = (entry.Sys, value, entry.Others); }
                else
                {
                    string display = string.IsNullOrEmpty(unit) ? value : $"{value} {unit}";
                    entry.Others.Add($"{codeName} | {display} | {dateStr}");
                    byDate[d] = entry;
                }
            }

            // Build buckets — BP merged into single row
            var buckets = new List<(DateTime, List<string>)>();
            foreach (var kvp in byDate.OrderByDescending(x => x.Key))
            {
                var (sys, dia, others) = kvp.Value;
                string dateStr = kvp.Key == DateTime.MinValue ? "" : kvp.Key.ToString("MM'/'dd'/'yyyy");
                var rows = new List<string>();

                if (!string.IsNullOrEmpty(sys) || !string.IsNullOrEmpty(dia))
                {
                    string bp = (sys != "" && dia != "") ? $"{sys}/{dia}" : $"{sys}{dia}";
                    rows.Add($"Blood Pressure | {bp} mmHg | {dateStr}");
                }
                rows.AddRange(others);
                buckets.Add((kvp.Key, rows));
            }

            return FilterOrAll(buckets, dos);
        }

        // ── PHYSICAL EXAM ────────────────────────────────────────────
        // ECW: list of items, each item = one visit date.
        // row[0]: date(rowspan) | category(rowspan) | field | details(colspan) | notes
        // row[1-N]: field | details
        private static List<string> ParseExam(XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var buckets = new List<(DateTime, List<string>)>();
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode == null) return new List<string>();

            foreach (XmlNode item in textNode.SelectNodes(".//cda:item", ns))
            {
                var tbody = item.SelectSingleNode(".//cda:tbody", ns);
                if (tbody == null) continue;
                var allRows = tbody.SelectNodes("cda:tr", ns);
                if (allRows.Count == 0) continue;

                var firstTds = allRows[0].SelectNodes("cda:td", ns);
                if (firstTds.Count == 0) continue;

                string itemDateStr = firstTds[0].InnerText.Trim();
                ParseDate(itemDateStr, out var itemDate);

                var rows = new List<string>();
                foreach (XmlNode tr in allRows)
                {
                    var tds = tr.SelectNodes("cda:td", ns);
                    string field, details;
                    if (tds.Count >= 4)
                    { field = tds[2].InnerText.Trim(); details = tds[3].InnerText.Trim(); }
                    else if (tds.Count >= 2)
                    { field = tds[0].InnerText.Trim(); details = tds[1].InnerText.Trim(); }
                    else continue;
                    if (!string.IsNullOrWhiteSpace(details))
                        rows.Add(string.IsNullOrEmpty(field) ? details : $"{field} | {details}");
                }

                if (rows.Any()) buckets.Add((itemDate, rows));
            }

            return FilterOrAll(buckets, dos);
        }

        // ── ASSESSMENT ───────────────────────────────────────────────
        // ECW: narrative table — td[0]=date  td[1]=diagnosis  td[3]=treatment notes
        private static List<string> ParseAssessment(
            XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode == null) goto Fallback;

            {
                // Group ALL rows by date WITHOUT global dedup
                // (same diagnosis appears on multiple visit dates in ECW)
                var byDate = new Dictionary<DateTime, List<(string Dx, string TreatNote, bool IsOther)>>();

                foreach (XmlNode tr in textNode.SelectNodes(".//cda:tr", ns))
                {
                    var tds = tr.SelectNodes("cda:td", ns);
                    if (tds.Count < 2) continue;

                    string dateCell = tds[0].InnerText.Trim();
                    string dxCell = tds[1].InnerText.Trim();
                    string treatNote = tds.Count > 3 ? GetInnerTextWithSpaces(tds[3]) : "";

                    if (string.IsNullOrWhiteSpace(dxCell)) continue;
                    if (dxCell.StartsWith("Diagnosis", StringComparison.OrdinalIgnoreCase)) continue;
                    if (dxCell.StartsWith("Encounter", StringComparison.OrdinalIgnoreCase)) continue;

                    if (!ParseDate(dateCell, out var rowDate)) continue;

                    if (!byDate.ContainsKey(rowDate))
                        byDate[rowDate] = new List<(string, string, bool)>();

                    bool isOther = dxCell.Equals("Other", StringComparison.OrdinalIgnoreCase);
                    byDate[rowDate].Add((dxCell, treatNote, isOther));
                }

                if (!byDate.Any()) goto Fallback;

                // Apply FilterOrAll to select which date(s) to show
                List<DateTime> selectedDates;
                if (dos.HasValue)
                {
                    // Exact DOS match
                    var exact = byDate.Keys.Where(k => k.Date == dos.Value.Date).ToList();
                    if (exact.Any())
                    {
                        selectedDates = exact;
                        Console.WriteLine($"[Assessment] Exact DOS match {dos.Value:MM/dd/yyyy}: {exact.Count} date(s)");
                    }
                    else
                    {
                        // No exact match — show most recent
                        selectedDates = new List<DateTime> { byDate.Keys.Max() };
                        Console.WriteLine($"[Assessment] No DOS match — using most recent {selectedDates[0]:MM/dd/yyyy}");
                    }
                }
                else
                {
                    selectedDates = byDate.Keys.OrderByDescending(k => k).ToList();
                }

                // Build output rows with per-date deduplication
                var rows = new List<string>();
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var date in selectedDates)
                {
                    foreach (var (dxCell, treatNote, isOther) in byDate[date])
                    {
                        if (isOther)
                        {
                            if (!string.IsNullOrWhiteSpace(treatNote))
                            {
                                string nk = $"CN:{treatNote[..Math.Min(50, treatNote.Length)]}";
                                if (seen.Add(nk))
                                    rows.Add($"CLINICAL_NOTE\n{treatNote}");
                            }
                            continue;
                        }

                        string display = dxCell
                            .Replace("(ICD-10 - ", "- ")
                            .Replace("(ICD-10-CM - ", "- ")
                            .TrimEnd(')');

                        // Strip "Specify : ..." ECW suffix
                        int si = display.IndexOf("Specify :", StringComparison.OrdinalIgnoreCase);
                        if (si > 0) display = display[..si].Trim().TrimEnd('-').Trim();
                        si = display.IndexOf("Specify:", StringComparison.OrdinalIgnoreCase);
                        if (si > 0) display = display[..si].Trim().TrimEnd('-').Trim();

                        if (!seen.Add(display)) continue;

                        rows.Add(string.IsNullOrWhiteSpace(treatNote)
                            ? display
                            : $"{display}\nNotes: {treatNote}");
                    }
                }

                Console.WriteLine($"[Assessment] dos={dos?.ToString("MM/dd/yyyy") ?? "none"} → {rows.Count} rows");
                return rows;
            }

        Fallback:
            if (dos.HasValue)
            {
                var encSection = section.OwnerDocument
                    .SelectSingleNode("//cda:section[cda:code[@code='46240-8']]",
                        BuildNs(section.OwnerDocument));
                if (encSection != null)
                    return ExtractDxFromEncounter(encSection, BuildNs(section.OwnerDocument), dos.Value);
            }

            Console.WriteLine("[Assessment] no data found");
            return new List<string>();
        }

        private static List<string> ExtractDxFromEncounter(
            XmlNode encSection, XmlNamespaceManager ns, DateTime dos)
        {
            var rows = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (XmlNode entry in encSection.SelectNodes("cda:entry", ns))
            {
                var enc = entry.SelectSingleNode("cda:encounter", ns);
                if (enc == null) continue;
                string lowVal = enc.SelectSingleNode("cda:effectiveTime/cda:low", ns)
                                   ?.Attributes?["value"]?.Value ?? "";
                if (!MatchesCcdaDateStr(lowVal, dos)) continue;

                foreach (XmlNode obs in enc.SelectNodes(
                    "cda:entryRelationship/cda:act/cda:entryRelationship/cda:observation", ns))
                {
                    var trans = obs.SelectSingleNode("cda:value/cda:translation", ns);
                    string d = trans?.Attributes?["displayName"]?.Value ?? "";
                    if (!string.IsNullOrWhiteSpace(d) && seen.Add(d)) rows.Add(d);
                }
                if (rows.Any()) break;
            }
            return rows;
        }

        // ── TREATMENT PLAN ───────────────────────────────────────────
        // ECW: observation moodCode=INT, text/reference → narrative td ID
        private static List<string> ParseTreatmentPlan(
            XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var idToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var idToDate = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Build ID→name and ID→orderDate from narrative table
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode != null)
            {
                bool skipHdr = true;
                foreach (XmlNode tr in textNode.SelectNodes(".//cda:tr", ns))
                {
                    if (skipHdr) { skipHdr = false; continue; }
                    var tds = tr.SelectNodes("cda:td", ns);
                    if (tds.Count < 1) continue;
                    string tdId = (tds[0] as XmlElement)?.GetAttribute("ID") ?? "";
                    string tdTxt = tds[0].InnerText.Trim();
                    string tdDt = tds.Count > 1 ? tds[1].InnerText.Trim() : "";
                    if (!string.IsNullOrEmpty(tdId) && !string.IsNullOrEmpty(tdTxt))
                    {
                        idToName[tdId] = tdTxt;
                        if (!string.IsNullOrEmpty(tdDt)) idToDate[tdId] = tdDt;
                    }
                }
            }

            // Collect all entries bucketed by their CCDA effectiveTime date
            var byDate = new Dictionary<DateTime, List<string>>();
            var dosRows = new List<string>();

            foreach (XmlNode entry in section.SelectNodes("cda:entry", ns))
            {
                var obs = entry.SelectSingleNode("cda:observation", ns);
                if (obs == null) continue;

                string rawDate = obs.SelectSingleNode("cda:effectiveTime", ns)
                                    ?.Attributes?["value"]?.Value ?? "";
                string refId = obs.SelectSingleNode("cda:text/cda:reference", ns)
                                    ?.Attributes?["value"]?.Value?.TrimStart('#') ?? "";
                string name = idToName.TryGetValue(refId, out var n) ? n : "";
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (!seen.Add(name)) continue;

                string orderDate = idToDate.TryGetValue(refId, out var od) ? od : CcdaDateStr(rawDate);
                string row = string.IsNullOrEmpty(orderDate) ? name : $"{name} | {orderDate}";

                DateTime.TryParseExact(rawDate.Length >= 8 ? rawDate[..8] : "",
                    "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var d);
                if (!byDate.ContainsKey(d)) byDate[d] = new List<string>();
                byDate[d].Add(row);

                // Track DOS ±7 day matches separately
                if (dos.HasValue && Math.Abs((d.Date - dos.Value.Date).TotalDays) <= 7)
                    dosRows.Add(row);
            }

            // FilterOrAll: DOS match → return only those; else return ALL
            if (dosRows.Any())
            {
                Console.WriteLine($"[TreatmentPlan] DOS match: {dosRows.Count} rows");
                return dosRows;
            }

            var allRows = byDate.OrderByDescending(x => x.Key)
                                .SelectMany(x => x.Value).ToList();

            if (allRows.Any())
            {
                Console.WriteLine($"[TreatmentPlan] No DOS match — showing ALL {allRows.Count} rows");
                return allRows;
            }

            Console.WriteLine("[TreatmentPlan] no data found");
            return new List<string>();
        }

        private static List<string> ParseProcedures(
            XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var seenAll = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var dosRows = new List<string>();
            var allRows = new List<string>();

            foreach (XmlNode entry in section.SelectNodes("cda:entry", ns))
            {
                var proc = entry.SelectSingleNode("cda:procedure", ns);
                if (proc == null) continue;

                string name = proc.SelectSingleNode("cda:code", ns)
                                     ?.Attributes?["displayName"]?.Value ?? "";
                string cptCode = proc.SelectSingleNode("cda:code", ns)
                                     ?.Attributes?["code"]?.Value ?? "";
                string procDate = proc.SelectSingleNode("cda:effectiveTime", ns)
                                      ?.Attributes?["value"]?.Value ?? "";

                if (string.IsNullOrWhiteSpace(name)) continue;
                string display = string.IsNullOrEmpty(cptCode)
                    ? name : $"{name} ({cptCode})";

                // Collect ALL entries (deduplicated by display name)
                if (seenAll.Add(display)) allRows.Add(display);

                // Also track which match DOS
                if (dos.HasValue && MatchesCcdaDateStr(procDate, dos.Value))
                    dosRows.Add(display);
            }

            // FilterOrAll: DOS match → show only those; else show all
            var rows = dos.HasValue && dosRows.Any() ? dosRows : allRows;

            Console.WriteLine("[Procedures] dos=" + (dos?.ToString("MM/dd/yyyy") ?? "none") + " -> " + rows.Count + " rows");
            return rows;
        }

        private static List<string> ParseEncounters(XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var buckets = new List<(DateTime, List<string>)>();
            var textNode = section.SelectSingleNode("cda:text", ns);

            if (textNode != null)
            {
                bool isHeader = true;
                foreach (XmlNode tr in textNode.SelectNodes(".//cda:tr", ns))
                {
                    if (isHeader) { isHeader = false; continue; }
                    var tds = tr.SelectNodes("cda:td", ns);
                    if (tds.Count < 6) continue;

                    string date = tds[0].InnerText.Trim();
                    string time = tds[1].InnerText.Trim();
                    string type = tds[2].InnerText.Trim();
                    string facility = tds[3].InnerText.Trim();
                    string provider = tds[5].InnerText.Trim();

                    if (string.IsNullOrWhiteSpace(date)) continue;
                    ParseDate(date, out var d);

                    string row = $"{date} {time} | {type} | {provider} | {facility}".Trim();
                    buckets.Add((d, new List<string> { row }));
                }
            }

            // Fallback: structured entries
            if (!buckets.Any())
            {
                foreach (XmlNode entry in section.SelectNodes("cda:entry", ns))
                {
                    var enc = entry.SelectSingleNode("cda:encounter", ns);
                    if (enc == null) continue;
                    string lowVal = enc.SelectSingleNode("cda:effectiveTime/cda:low", ns)
                                       ?.Attributes?["value"]?.Value ?? "";
                    string dateStr = CcdaDateStr(lowVal);
                    string type = enc.SelectSingleNode("cda:code", ns)?.Attributes?["displayName"]?.Value ?? "Visit";
                    if (string.IsNullOrWhiteSpace(dateStr)) continue;
                    ParseDate(dateStr, out var d);
                    buckets.Add((d, new List<string> { $"{dateStr} | {type}" }));
                }
            }

            return FilterOrAll(buckets, dos);
        }

        // ── LAB RESULTS ─────────────────────────────────────────────
        // ECW: organizer per panel, observations inside.
        // No DOS filter from the parser — show all available labs.
        private static List<string> ParseResults(XmlNode section, XmlNamespaceManager ns, DateTime? dos)
        {
            var rows = new List<string>();
            int panelIdx = 0;

            // Build panel name lookup from narrative
            var panelNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode != null)
                foreach (XmlNode content in textNode.SelectNodes(".//cda:content", ns))
                {
                    string cid = (content as XmlElement)?.GetAttribute("ID") ?? "";
                    string name = Regex.Replace(content.InnerText?.Trim() ?? "", @"\s*\(\d+\)\s*$", "").Trim();
                    if (!string.IsNullOrEmpty(cid) && !string.IsNullOrEmpty(name))
                        panelNames[cid] = name;
                }

            foreach (XmlNode organizer in section.SelectNodes(".//cda:organizer", ns))
            {
                panelIdx++;
                string panelId = $"result{panelIdx}";
                string panelName = panelNames.TryGetValue(panelId, out var pn) ? pn : $"Lab Panel {panelIdx}";
                string lowVal = organizer.SelectSingleNode("cda:effectiveTime/cda:low", ns)
                                            ?.Attributes?["value"]?.Value ?? "";
                string panelDate = CcdaDateStr(lowVal);

                rows.Add($"{panelName}|ORDER|{panelDate}");

                foreach (XmlNode obs in organizer.SelectNodes(".//cda:observation", ns))
                {
                    string testName = obs.SelectSingleNode("cda:code", ns)?.Attributes?["displayName"]?.Value ?? "";
                    var valNode = obs.SelectSingleNode("cda:value", ns);
                    string value = valNode?.Attributes?["value"]?.Value
                                   ?? valNode?.SelectSingleNode("cda:translation", ns)?.Attributes?["value"]?.Value
                                   ?? valNode?.InnerText?.Trim() ?? "";
                    string unit = CleanUnit(valNode?.Attributes?["unit"]?.Value ?? "");
                    string flag = obs.SelectSingleNode("cda:interpretationCode", ns)?.Attributes?["code"]?.Value ?? "";
                    string refText = obs.SelectSingleNode(".//cda:observationRange/cda:text", ns)?.InnerText?.Trim() ?? "";

                    if (string.IsNullOrWhiteSpace(testName)) continue;
                    string display = string.IsNullOrEmpty(unit) ? value : $"{value} {unit}";
                    rows.Add($"{testName}|{display}|{refText}|{flag}");
                }
            }

            if (!rows.Any()) rows.AddRange(ParseNarrativeText(section, ns));
            return rows; // Labs: always show all — no DOS filter
        }

        // ── ALLERGIES ────────────────────────────────────────────────
        private static List<string> ParseAllergies(XmlNode section, XmlNamespaceManager ns)
        {
            var rows = new List<string>();
            foreach (XmlNode obs in section.SelectNodes(".//cda:observation", ns))
            {
                string substance = obs.SelectSingleNode(".//cda:participant//cda:playingEntity/cda:name", ns)
                                      ?.InnerText?.Trim()
                                ?? obs.SelectSingleNode(".//cda:participantRole//cda:playingEntity/cda:name", ns)
                                      ?.InnerText?.Trim() ?? "";
                string reaction = obs.SelectSingleNode(".//cda:value", ns)?.Attributes?["displayName"]?.Value
                                ?? obs.SelectSingleNode(".//cda:entryRelationship//cda:value", ns)
                                      ?.Attributes?["displayName"]?.Value ?? "";

                if (string.IsNullOrWhiteSpace(substance)) continue;
                rows.Add(string.IsNullOrEmpty(reaction) ? substance : $"{substance} — {reaction}");
            }
            return rows;
        }

        // ── PROBLEM LIST / PMH ───────────────────────────────────────
        private static List<string> ParseProblemList(XmlNode section, XmlNamespaceManager ns)
        {
            var rows = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (XmlNode act in section.SelectNodes(".//cda:act", ns))
            {
                var obs = act.SelectSingleNode(".//cda:observation/cda:value", ns);
                if (obs == null) continue;

                string display = obs.Attributes?["displayName"]?.Value ?? "";
                string icdDisp = obs.SelectSingleNode("cda:translation[@codeSystem='2.16.840.1.113883.6.90']", ns)
                                    ?.Attributes?["displayName"]?.Value ?? "";
                string icdCode = obs.SelectSingleNode("cda:translation[@codeSystem='2.16.840.1.113883.6.90']", ns)
                                    ?.Attributes?["code"]?.Value ?? "";

                string preferred = !string.IsNullOrEmpty(icdDisp) ? icdDisp : display;
                if (string.IsNullOrWhiteSpace(preferred)) continue;

                string row = !string.IsNullOrEmpty(icdCode) ? $"{preferred} ({icdCode})" : preferred;
                if (seen.Add(row)) rows.Add(row);
            }

            if (!rows.Any()) rows.AddRange(ParseNarrativeText(section, ns));
            return rows;
        }

        // ── MEDICATIONS — narrative table ────────────────────────────
        private static List<string> ParseNarrativeTable(XmlNode section, XmlNamespaceManager ns)
        {
            var rows = new List<string>();
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode == null) return rows;

            bool isHeader = true;
            foreach (XmlNode tr in textNode.SelectNodes(".//cda:tr", ns))
            {
                if (isHeader) { isHeader = false; continue; }
                var tds = tr.SelectNodes("cda:td", ns);
                if (tds.Count < 1) continue;
                var cols = tds.Cast<XmlNode>().Select(td => td.InnerText.Trim()).ToList();
                if (!string.IsNullOrWhiteSpace(cols[0]))
                    rows.Add(string.Join(" | ", cols));
            }
            return rows;
        }

        // ── GENERIC NARRATIVE TEXT FALLBACK ─────────────────────────
        private static List<string> ParseNarrativeText(XmlNode section, XmlNamespaceManager ns)
        {
            var rows = new List<string>();
            var textNode = section.SelectSingleNode("cda:text", ns);
            if (textNode == null) return rows;

            string rawText = textNode.InnerText?.Trim();
            if (string.Equals(rawText, "No Information", StringComparison.OrdinalIgnoreCase))
                return rows;

            // Table rows (skip header)
            var trs = textNode.SelectNodes(".//cda:tr", ns);
            if (trs.Count > 0)
            {
                bool first = true;
                foreach (XmlNode tr in trs)
                {
                    if (first) { first = false; continue; }
                    var cells = tr.SelectNodes("cda:td", ns);
                    if (cells.Count == 0) continue;
                    string line = string.Join(" | ", cells.Cast<XmlNode>()
                        .Select(c => c.InnerText.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s)));
                    if (!string.IsNullOrWhiteSpace(line)) rows.Add(line);
                }
                if (rows.Any()) return rows;
            }

            // List items
            foreach (XmlNode item in textNode.SelectNodes(".//cda:item | .//cda:paragraph", ns))
            {
                string text = item.InnerText?.Trim();
                if (!string.IsNullOrWhiteSpace(text) && text.Length > 2) rows.Add(text);
            }

            // Raw text
            if (!rows.Any() && !string.IsNullOrWhiteSpace(rawText))
                foreach (var line in rawText.Split('\n'))
                {
                    string l = line.Trim();
                    if (!string.IsNullOrWhiteSpace(l) && l.Length > 2) rows.Add(l);
                }

            return rows;
        }

        // ═══════════════════════════════════════════════════════════
        //  PATIENT EXTRACTION
        // ═══════════════════════════════════════════════════════════

        public static PatientInfo ExtractPatient(string ccdaXml)
        {
            var info = new PatientInfo();
            if (string.IsNullOrWhiteSpace(ccdaXml)) return info;

            try
            {
                ccdaXml = ccdaXml.TrimStart('\uFEFF', '\u200B').Trim();
                var doc = new XmlDocument();
                doc.LoadXml(ccdaXml);
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("cda", "urn:hl7-org:v3");

                var patient = doc.SelectSingleNode("//cda:patient", ns);
                if (patient != null)
                {
                    string given = patient.SelectSingleNode("cda:name/cda:given", ns)?.InnerText?.Trim() ?? "";
                    string family = patient.SelectSingleNode("cda:name/cda:family", ns)?.InnerText?.Trim() ?? "";
                    info.Name = $"{given} {family}".Trim();

                    string dobRaw = patient.SelectSingleNode("cda:birthTime", ns)?.Attributes?["value"]?.Value ?? "";
                    if (dobRaw.Length >= 8 && DateTime.TryParseExact(dobRaw[..8], "yyyyMMdd",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                    {
                        info.DOB = dob.ToString("MM'/'dd'/'yyyy");
                        int age = DateTime.Today.Year - dob.Year;
                        if (DateTime.Today < dob.AddYears(age)) age--;
                        info.Age = age.ToString();
                    }
                }

                // Account number — prefer "ACC NO" tag
                string accNo = null, fallback = null;
                foreach (XmlNode id in doc.SelectNodes("//cda:patientRole/cda:id", ns))
                {
                    string auth = id.Attributes?["assigningAuthorityName"]?.Value ?? "";
                    string ext = id.Attributes?["extension"]?.Value ?? "";
                    if (auth.Contains("ACC NO", StringComparison.OrdinalIgnoreCase)) { accNo = ext; break; }
                    if (fallback == null && ext.Length <= 10 &&
                        !auth.Equals("HCID", StringComparison.OrdinalIgnoreCase))
                        fallback = ext;
                }
                info.AccountNumber = accNo ?? fallback ?? "";

                var tel = doc.SelectSingleNode("//cda:patientRole/cda:telecom[@use='HP']", ns)
                       ?? doc.SelectSingleNode("//cda:patientRole/cda:telecom", ns);
                info.Phone = tel?.Attributes?["value"]?.Value?.Replace("tel:", "").Replace("+1", "").Trim() ?? "";

                var addr = doc.SelectSingleNode("//cda:patientRole/cda:addr[@use='PST']", ns)
                          ?? doc.SelectSingleNode("//cda:patientRole/cda:addr", ns);
                if (addr != null)
                {
                    string street = addr.SelectSingleNode("cda:streetAddressLine", ns)?.InnerText?.Trim() ?? "";
                    string city = addr.SelectSingleNode("cda:city", ns)?.InnerText?.Trim() ?? "";
                    string state = addr.SelectSingleNode("cda:state", ns)?.InnerText?.Trim() ?? "";
                    string zip = addr.SelectSingleNode("cda:postalCode", ns)?.InnerText?.Trim() ?? "";
                    info.Address = string.Join(", ", new[] { street, city, state, zip }
                        .Where(s => !string.IsNullOrEmpty(s) && s != "nullFlavor"));
                }

                var perf = doc.SelectSingleNode(
                    "//cda:documentationOf/cda:serviceEvent/cda:performer[@typeCode='PRF'][1]//cda:assignedPerson/cda:name", ns);
                if (perf != null)
                {
                    string pg = perf.SelectSingleNode("cda:given", ns)?.InnerText?.Trim() ?? "";
                    string pf = perf.SelectSingleNode("cda:family", ns)?.InnerText?.Trim() ?? "";
                    string ps = perf.SelectSingleNode("cda:suffix", ns)?.InnerText?.Trim() ?? "";
                    info.Provider = $"{pg} {pf} {ps}".Trim();
                }

                string effRaw = doc.SelectSingleNode("//cda:effectiveTime", ns)?.Attributes?["value"]?.Value ?? "";
                if (effRaw.Length >= 8 && DateTime.TryParseExact(effRaw[..8], "yyyyMMdd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var effDate))
                    info.DateOfService = effDate.ToString("MM'/'dd'/'yyyy");
            }
            catch (Exception ex) { Console.WriteLine($"[ExtractPatient ERROR] {ex.Message}"); }

            return info;
        }

        // ═══════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════

        // Robust date parser — tries all common formats, InvariantCulture only
        private static readonly string[] DateFormats = {
            "MM/dd/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "yyyy/MM/dd",
            "M/d/yyyy",   "M-d-yyyy",   "dd/MM/yyyy", "dd-MM-yyyy",
            "MM/dd/yy",   "M/d/yy",     "yyyyMMdd"
        };

        public static bool ParseDate(string s, out DateTime result)
        {
            result = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;
            if (DateTime.TryParseExact(s.Trim(), DateFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return true;
            return DateTime.TryParse(s.Trim(), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result);
        }

        private static DateTime? ParseDate(string s)
            => ParseDate(s, out var d) ? d : (DateTime?)null;

        // Convert CCDA yyyyMMdd[tz] → "MM/dd/yyyy" using literal slash
        private static string CcdaDateStr(string raw)
        {
            if (string.IsNullOrEmpty(raw) || raw.Length < 8) return "";
            return DateTime.TryParseExact(raw[..8], "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                ? d.ToString("MM'/'dd'/'yyyy")
                : "";
        }

        // Check CCDA yyyyMMdd[tz] against target date
        private static bool MatchesCcdaDateStr(string raw, DateTime target)
        {
            if (string.IsNullOrEmpty(raw) || raw.Length < 8) return false;
            return DateTime.TryParseExact(raw[..8], "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                && d.Date == target.Date;
        }


        // Concatenates all text in a node, adding a space between child elements
        // to prevent word run-on when ECW splits text across <paragraph> children
        // with no tail text (produces "thetelemedicine" without this fix).
        private static string GetInnerTextWithSpaces(XmlNode node)
        {
            if (node == null) return "";
            var sb = new System.Text.StringBuilder();
            CollectText(node, sb);
            // Collapse multiple spaces
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @" {2,}", " ").Trim();
        }

        private static void CollectText(XmlNode node, System.Text.StringBuilder sb)
        {
            if (node.NodeType == System.Xml.XmlNodeType.Text ||
                node.NodeType == System.Xml.XmlNodeType.CDATA)
            {
                if (!string.IsNullOrWhiteSpace(node.Value))
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] != ' ')
                        sb.Append(' ');
                    sb.Append(node.Value);
                }
                return;
            }
            foreach (XmlNode child in node.ChildNodes)
                CollectText(child, sb);
        }

        // Clean ECW UCUM unit codes
        private static string CleanUnit(string u) =>
            u.Replace("mm[Hg]", "mmHg").Replace("[lb_av]", "lbs")
             .Replace("[in_i]", "in").Replace("[degF]", "°F")
             .Replace("[degC]", "°C").Trim();

        private static XmlNamespaceManager BuildNs(XmlDocument doc)
        {
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");
            return ns;
        }

        private static bool WithinWindow(DateTime a, DateTime b, int days = 1)
            => Math.Abs((a.Date - b.Date).TotalDays) <= days;
    }
}