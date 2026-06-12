using emedl_chase.Helper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.RegularExpressions;
using System.Xml;

namespace emedl_chase.Model
{
    public class ClinicalPdfGenerator
    {

        private static readonly HashSet<string> ManuallyRenderedSections = new()
        {
            "History and Physical Notes",
            "Vital Signs",
            "Progress Notes",
            "Results"
        };
        // Keys match EXACTLY what debug output shows

        private static readonly List<(string Key, string Label, string Group)> SectionOrder = new()
{
    ("History and Physical Notes",  "History & Physical",      "SUBJECTIVE"),
    ("Medical (General) History",   "Medical History",         "SUBJECTIVE"),
    ("PMH",                         "Past Medical History",    "SUBJECTIVE"),
    ("PMSFH",                       "Social / Family History", "SUBJECTIVE"),
    ("Allergy",                     "Allergies",               "SUBJECTIVE"),  // matches 'ALLERGY'
    ("Medications",                 "Medications",             "SUBJECTIVE"),
    ("Immunizations",               "Immunizations",           "SUBJECTIVE"),
    ("Vital Signs",                 "Vital Signs",             "OBJECTIVE"),
    ("Progress Notes",              "Physical Examination",    "OBJECTIVE"),
    ("Results",                     "Lab Results",             "OBJECTIVE"),
    ("MDM or Assessment and Plan",  "Assessment",              "ASSESSMENT & PLAN"), // matches 'MDM OR ASSESSMENT AND PLAN'
    ("Treatment Plan",              "Treatment Plan",          "ASSESSMENT & PLAN"), // matches 'TREATMENT PLAN'
    ("Procedures",                  "Procedures",              "ASSESSMENT & PLAN"),
    ("Encounters",                  "Encounter History",       "ENCOUNTER HISTORY"),
    ("Insurance Providers",         "Insurance",               "ADMINISTRATIVE"),
};

        // Remove Allergy from SkipIfNoInfo:
        private static readonly HashSet<string> SkipIfNoInfo = new()
{
    "Medications", "Immunizations",
    "Reason For Referral", "Medical Equipment",
    "Goals Section", "Health Concerns",
    "Social History"
    // "Allergies" removed — always show with fallback
};

        public static byte[] Generate(ClinicalNoteNew note, string dosFilter = null)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36, Unit.Point);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    page.Header().Element(c => ComposeHeader(c, note));
                    page.Content().Element(c => ComposeBody(c, note, dosFilter));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        // ── Header ───────────────────────────────────────────────
        private static void ComposeHeader(IContainer container, ClinicalNoteNew note)
        {
            container.Column(col =>
            {
                // ── Top bar: "HENRYK BLASZCZYK | DOB: ... | DOS: ..." ──
                col.Item()
                   .BorderBottom(1)
                   //.BorderBottom(1).BorderColor("#000000")
                   .PaddingBottom(4)
                   .Text(txt =>
                   {
                       txt.Span($"{note.PatientName?.ToUpper()}").Bold().FontSize(9);
                       txt.Span($" | DOB: {note.DateOfBirth} ({note.Age} yo)").FontSize(9);
                       //txt.Span($" | Sex: {note.Sex}").FontSize(9);
                       txt.Span($" | Acc No: {note.AccountNumber}").FontSize(9);
                       txt.Span($" | DOS: {note.DateOfService}").FontSize(9);
                   });

                // ── Title: "Patient Health Record" ──
                col.Item()
                   .PaddingTop(6).PaddingBottom(6)
                   .Text("Patient Health Record")
                   .FontSize(18).Bold()
                   .AlignCenter();

                // ── Patient details block ──
                col.Item()
                   .PaddingTop(4)
                   .Row(row =>
                   {
                       // Left column
                       row.RelativeItem().Column(left =>
                       {
                           left.Item().Text(txt =>
                           {
                               txt.Span("Patient: ").FontSize(10);
                               txt.Span(note.PatientName).FontSize(10);
                           });
                           left.Item().Text(txt =>
                           {
                               txt.Span("Account Number: ").FontSize(10);
                               txt.Span(note.AccountNumber).FontSize(10);
                           });
                           left.Item().Text(txt =>
                           {
                               txt.Span("DOB: ").FontSize(10);
                               txt.Span(note.DateOfBirth).FontSize(10);
                           });
                           left.Item().Text(txt =>
                           {
                               txt.Span("Age: ").FontSize(10);
                               txt.Span(note.Age).FontSize(10);
                           });
                           //left.Item().Text(txt =>
                           //{
                           //    txt.Span("Sex: ").FontSize(10);
                           //    txt.Span(note.Sex).FontSize(10);
                           //});
                           left.Item().Text(txt =>
                           {
                               txt.Span("Phone: ").FontSize(10);
                               txt.Span(note.Phone).FontSize(10);
                           });
                           left.Item().Text(txt =>
                           {
                               txt.Span("Address: ").FontSize(10);
                               txt.Span(note.Address).FontSize(10);
                           });
                       });

                       // Right column
                       row.RelativeItem().Column(right =>
                       {
                           right.Item().AlignRight().Text(txt =>
                           {
                               txt.Span("Provider: ").FontSize(10);
                               txt.Span(note.Provider).FontSize(10);
                           });
                           right.Item().AlignRight().Text(txt =>
                           {
                               txt.Span("Date: ").FontSize(10);
                               txt.Span(note.DateOfService).FontSize(10);
                           });
                       });
                   });

                // ── Bottom divider ──
                col.Item()
                   .BorderBottom(1)
                   //.BorderBottom(1).BorderColor("#000000")
                   .PaddingTop(8)
                   .Text("");

                col.Item().Height(8);
            });
        }

        // ── Body ─────────────────────────────────────────────────
        //private static void ComposeBody(IContainer container, ClinicalNoteNew note, string dosFilter)
        //{
        //    // Case-insensitive lookup
        //    var lookup = note.Sections
        //        .ToDictionary(k => k.Key.ToUpperInvariant(), v => v.Value);

        //    container.Column(col =>
        //    {
        //        string lastGroup = null;

        //        foreach (var (key, label, group) in SectionOrder)
        //        {
        //            if (!lookup.TryGetValue(key.ToUpperInvariant(), out var rows))
        //                continue;

        //            if (ManuallyRenderedSections.Contains(key)) continue;

        //            // Filter "No Information" rows for noisy sections
        //            if (SkipIfNoInfo.Contains(key) &&
        //                rows.All(r => r.Trim().Equals("No Information", StringComparison.OrdinalIgnoreCase)))
        //                continue;

        //            // For Progress Notes: only show rows matching the DOS date
        //            //List<string> filteredRows = rows;
        //            //if (key == "Progress Notes" && !string.IsNullOrEmpty(dosFilter))
        //            //{
        //            //    // Each progress note block starts with a date like "06/01/2026 | ..."
        //            //    // Group rows into blocks by date-header rows, keep only the DOS block
        //            //    filteredRows = FilterProgressNotesByDos(rows, dosFilter);
        //            //    if (!filteredRows.Any()) continue;
        //            //}




        //            // NEW — filters both Progress Notes AND History and Physical Notes:
        //            List<string> filteredRows = rows;
        //            if (DosFilteredSections.Contains(key) && !string.IsNullOrEmpty(dosFilter))
        //            {
        //                filteredRows = FilterProgressNotesByDos(rows, dosFilter);
        //                if (!filteredRows.Any()) continue;
        //            }

        //            // SOAP group banner
        //            if (group != lastGroup)
        //            {
        //                col.Item().Element(c => GroupBanner(c, group));
        //                lastGroup = group;
        //            }

        //            // Section title
        //            col.Item().PaddingTop(6).Text(label)
        //               .FontSize(11).Bold().FontColor("#1E4F8C");

        //            // Render rows

        //            if (key == "History and Physical Notes")
        //            {
        //                List<string> filtered = string.IsNullOrEmpty(dosFilter)
        //                    ? filteredRows
        //                    : FilterHpiByDos(filteredRows, dosFilter);

        //                if (!filtered.Any()) continue;

        //                if (group != lastGroup)
        //                {
        //                    col.Item().Element(c => GroupBanner(c, group));
        //                    lastGroup = group;
        //                }
        //                col.Item().PaddingTop(6).Text(label)
        //                   .FontSize(11).Bold().FontColor("#1E4F8C");
        //                RenderHpiNotes(col, filtered);
        //                continue; // skip everything else below
        //            }

        //            else if (key == "Vital Signs")
        //            {
        //                if (string.IsNullOrEmpty(dosFilter))
        //                {
        //                    RenderVitalsTable(col, filteredRows);
        //                }
        //                else
        //                {
        //                    // Only show vitals recorded ON the DOS date
        //                    var dosVitals = filteredRows.Where(r =>
        //                    {
        //                        var parts = r.Split('|');
        //                        if (parts.Length < 3) return false;
        //                        string rowDate = parts[2].Trim(); // "06/01/2026"
        //                        if (!DateTime.TryParse(rowDate, out var vDate)) return false;
        //                        if (!DateTime.TryParse(dosFilter, out var dos)) return false;
        //                        return vDate.Date == dos.Date;
        //                    }).ToList();

        //                    // Only render section if vitals exist for this DOS
        //                    if (dosVitals.Any())
        //                    {
        //                        if (group != lastGroup)
        //                        {
        //                            col.Item().Element(c => GroupBanner(c, group));
        //                            lastGroup = group;
        //                        }
        //                        col.Item().PaddingTop(6).Text(label)
        //                           .FontSize(11).Bold().FontColor("#1E4F8C");
        //                        RenderVitalsTable(col, dosVitals);
        //                    }
        //                }
        //                continue; // skip generic renderer
        //            }
        //            else if (key == "Results")
        //                RenderResultsTable(col, filteredRows, dosFilter);
        //            else if (key == "PMH")
        //                RenderPmhList(col, filteredRows);
        //            //else if (key == "History and Physical Notes")
        //            //    RenderHpiNotes(col, filteredRows);

        //            if (key == "Progress Notes")
        //            {
        //                List<string> filtered = string.IsNullOrEmpty(dosFilter)
        //                    ? filteredRows
        //                    : FilterProgressNotesByDos(filteredRows, dosFilter);

        //                // Skip entirely if nothing matched for this DOS
        //                if (!filtered.Any()) continue;

        //                if (group != lastGroup)
        //                {
        //                    col.Item().Element(c => GroupBanner(c, group));
        //                    lastGroup = group;
        //                }
        //                col.Item().PaddingTop(6).Text(label)
        //                   .FontSize(11).Bold().FontColor("#1E4F8C");
        //                RenderPlainRows(col, filtered);
        //                continue;
        //            }
        //            else
        //                RenderPlainRows(col, filteredRows);
        //        }
        //    });
        //}

        private static void ComposeBody(IContainer container, ClinicalNoteNew note, string dosFilter)
        {
            var lookup = note.Sections.ToDictionary(k => k.Key.ToUpperInvariant(), v => v.Value);

            // DEBUG — print all keys in lookup vs SectionOrder keys
            Console.WriteLine("=== LOOKUP KEYS IN NOTE.SECTIONS ===");
            foreach (var k in lookup.Keys)
                Console.WriteLine($"  LOOKUP: '{k}'");

            Console.WriteLine("=== SECTIONORDER KEYS ===");
            foreach (var (key, label, group) in SectionOrder)
                Console.WriteLine($"  ORDER KEY: '{key.ToUpperInvariant()}' → match={lookup.ContainsKey(key.ToUpperInvariant())}");

            container.Column(col =>
            {
                string lastGroup = null;

                foreach (var (key, label, group) in SectionOrder)
                {
                    if (!lookup.TryGetValue(key.ToUpperInvariant(), out var rows)) continue;

                    // ── Skip "No Information" sections ──────────────
                    if (SkipIfNoInfo.Contains(key) &&
                        rows.All(r => r.Trim().Equals("No Information",
                            StringComparison.OrdinalIgnoreCase))) continue;

                    // ── Get filtered rows based on section type ──────
                    List<string> displayRows;

                    // Filter switch — remove Assessments/Plan Of Treatment, keep only matching keys:
                    switch (key)
                    {
                        case "History and Physical Notes":
                            displayRows = FilterHpiByDos(rows, dosFilter);
                            break;
                        case "Progress Notes":
                        case "Encounters":
                            displayRows = FilterProgressNotesByDos(rows, dosFilter);
                            break;
                        case "Vital Signs":
                            displayRows = FilterVitalsByDos(rows, dosFilter);
                            break;
                        case "MDM or Assessment and Plan":
                            displayRows = rows; // already filtered by ParseAssessmentFromXml
                            break;

                        case "Treatment Plan":
                            displayRows = FilterTreatmentPlanByDos(rows, dosFilter); // NEW
                            break;
                        default:
                            displayRows = rows;
                            break;
                    }

                    //if (!displayRows.Any()) continue;

                    //if (!displayRows.Any() || !HasRealContent(displayRows)) continue;

                    if (key != "Allergy" && (!displayRows.Any() || !HasRealContent(displayRows))) continue;

                    // ── Group banner ─────────────────────────────────
                    if (group != lastGroup)
                    {
                        col.Item().Element(c => GroupBanner(c, group));
                        lastGroup = group;
                    }

                    // ── Section title (only once) ────────────────────
                    col.Item().PaddingTop(6).Text(label)
                       .FontSize(11).Bold().FontColor("#1E4F8C");

                    // Special case: Allergy always shows, with fallback if no real content
                    // In ComposeBody, Allergy special case — handle both possible key names:
                    if (key == "Allergies" || key == "Allergy")
                    {
                        bool hasRealAllergy = HasRealContent(displayRows) &&
                            !displayRows.All(r => r.Trim().Equals("No Information",
                                StringComparison.OrdinalIgnoreCase));

                        if (hasRealAllergy)
                            RenderPlainRows(col, displayRows);
                        else
                            col.Item().PaddingLeft(12).PaddingTop(2)
                               .Text("No Information").FontSize(10)
                               .FontColor("#666666").Italic();

                        continue;
                    }

                    if (key != "Allergies" && key != "Allergy" &&
                    (!displayRows.Any() || !HasRealContent(displayRows))) continue;

                    // ── Render content ───────────────────────────────
                    // Render switch — remove Assessments/Plan Of Treatment duplicates:
                    switch (key)
                    {
                        case "History and Physical Notes":
                            RenderHpiNotes(col, displayRows);
                            break;
                        case "Vital Signs":
                            RenderVitalsTable(col, displayRows);
                            break;
                        case "Results":
                            RenderResultsTable(col, displayRows, dosFilter);
                            break;
                        case "PMH":
                            RenderPmhList(col, displayRows);
                            break;
                        case "Progress Notes":
                            RenderProgressNotes(col, displayRows);
                            break;
                        case "MDM or Assessment and Plan":
                        case "Treatment Plan":
                            RenderAssessment(col, displayRows);
                            break;
                        default:
                            RenderPlainRows(col, displayRows);
                            break;
                    }
                }
            });
        }

        private static List<string> FilterTreatmentPlanByDos(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dos)) return rows;
            string dosFormatted = dos.ToString("MM/dd/yyyy");

            // Skip known noise rows entirely
            var noiseRows = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Pending Test", "Future Test", "Next Appt"
    };

            var result = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                string trimmed = row.Trim();

                // Skip noise
                if (noiseRows.Contains(trimmed)) continue;
                if (trimmed.StartsWith("Test NameOrder", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("DetailsProvider", StringComparison.OrdinalIgnoreCase)) continue;

                // Check if row contains any embedded dates
                var dateMatches = Regex.Matches(trimmed, @"\d{2}/\d{2}/\d{4}");
                if (dateMatches.Count == 0)
                {
                    result.Add(trimmed); // no dates — plain text, include as-is
                    continue;
                }

                // Only include rows where at least one date matches DOS
                bool hasDosDate = dateMatches.Cast<Match>()
                    .Any(m => m.Value == dosFormatted);

                if (!hasDosDate) continue;

                // Extract only the DOS-relevant segment if multiple dates are embedded
                // e.g. "Venogram06/01/2026" — extract "Venogram"
                var dosSegments = new List<string>();
                var allMatches = Regex.Matches(trimmed, @"([A-Za-z\s:/,()]+)("
                    + Regex.Escape(dosFormatted) + @")");

                foreach (Match m in allMatches)
                {
                    string label = m.Groups[1].Value.Trim().TrimEnd('/');
                    if (!string.IsNullOrWhiteSpace(label))
                        dosSegments.Add(label);
                }

                if (dosSegments.Any())
                    result.Add(string.Join(", ", dosSegments));
                else
                    result.Add(trimmed); // fallback: include whole row
            }

            return result.Any() ? result : new List<string>();
        }
        private static List<string> FilterAssessmentByDos(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dos)) return rows;
            string dosFormatted = dos.ToString("MM/dd/yyyy"); // "06/02/2026"

                var noiseRows = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Pending Test", "Future Test", "Next Appt"
                };

            var result = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                string trimmed = row.Trim();

                // Skip noise rows
                if (noiseRows.Contains(trimmed)) continue;
                if (trimmed.StartsWith("Test NameOrder", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("DetailsProvider", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("Encounter Date", StringComparison.OrdinalIgnoreCase)) continue;

                // Check for embedded dates — format is "MM/dd/yyyy" directly concatenated
                // e.g. "06/02/2026Pain in leg, unspecified (ICD-10 - M79.606)06/02/2026SOB..."
                var allDateMatches = System.Text.RegularExpressions.Regex.Matches(
                    trimmed, @"\d{2}/\d{2}/\d{4}");

                if (allDateMatches.Count == 0)
                {
                    // No dates at all — plain text, include as-is
                    result.Add(trimmed);
                    continue;
                }

                // Find all positions of the DOS date in this string
                var dosPositions = new List<int>();
                int searchFrom = 0;
                while (true)
                {
                    int idx = trimmed.IndexOf(dosFormatted, searchFrom, StringComparison.Ordinal);
                    if (idx < 0) break;
                    dosPositions.Add(idx);
                    searchFrom = idx + 1;
                }

                if (!dosPositions.Any()) continue;

                // For each DOS occurrence, extract content between this date and the next date
                foreach (int startIdx in dosPositions)
                {
                    int contentStart = startIdx + dosFormatted.Length; // skip past the date
                    if (contentStart >= trimmed.Length) continue;

                    // Find the next date pattern after contentStart
                    var nextDateMatch = System.Text.RegularExpressions.Regex.Match(trimmed.Substring(contentStart), @"\d{2}/\d{2}/\d{4}");

                    int contentEnd = nextDateMatch.Success
                        ? contentStart + nextDateMatch.Index
                        : trimmed.Length;

                    //int contentEnd = nextDateMatch.Success ? nextDateMatch.Index : trimmed.Length;
                    string content = trimmed.Substring(contentStart, contentEnd - contentStart).Trim();

                    if (string.IsNullOrWhiteSpace(content)) continue;

                    // Strip "Other" prefix ECW prepends to note paragraphs
                    if (content.StartsWith("Other", StringComparison.OrdinalIgnoreCase))
                        content = content.Substring(5).Trim();

                    // Clean ICD format: "(ICD-10 - M79.606)" → "- M79.606"
                    content = System.Text.RegularExpressions.Regex.Replace(
                        content, @"\(ICD-10\s*-\s*([A-Z0-9.]+)\)", "- $1");

                    if (!string.IsNullOrWhiteSpace(content))
                        result.Add(content);
                }
            }

            // Deduplicate while preserving order
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return result.Where(r => seen.Add(r)).ToList();
        }
        private static List<string> FilterAssessmentByDosOld(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dos)) return rows;
            string dosFormatted = dos.ToString("MM/dd/yyyy");

            var result = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                // Skip noise header rows
                if (row.StartsWith("Encounter Date", StringComparison.OrdinalIgnoreCase) ||
                    row.StartsWith("Diagnosis (ICD", StringComparison.OrdinalIgnoreCase) ||
                    row.Equals("Pending Test", StringComparison.OrdinalIgnoreCase) ||
                    row.Equals("Future Test", StringComparison.OrdinalIgnoreCase) ||
                    row.Equals("Next Appt", StringComparison.OrdinalIgnoreCase) ||
                    row.StartsWith("Test NameOrder Date", StringComparison.OrdinalIgnoreCase) ||
                    row.StartsWith("DetailsProvider", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Check if row contains embedded dates (the big concatenated string)
                bool hasEmbeddedDates = System.Text.RegularExpressions.Regex.IsMatch(
                    row, @"\d{2}/\d{2}/\d{4}");

                if (!hasEmbeddedDates)
                {
                    // Clean plain text — include as-is
                    result.Add(row);
                    continue;
                }

                // ── Parse the giant concatenated Assessment string ──
                // Format: "date+diagnosis+date+diagnosis..." mixed with notes text
                // Strategy: split on date pattern, pair each date with following content
                var segments = System.Text.RegularExpressions.Regex.Split(
                    row, @"(?=\d{2}/\d{2}/\d{4})");

                foreach (var seg in segments)
                {
                    if (seg.Length < 12) continue;

                    // Extract the date at start of segment
                    var dateMatch = System.Text.RegularExpressions.Regex.Match(
                        seg, @"^(\d{2}/\d{2}/\d{4})(.*)$",
                        System.Text.RegularExpressions.RegexOptions.Singleline);

                    if (!dateMatch.Success) continue;

                    string segDate = dateMatch.Groups[1].Value;
                    string segContent = dateMatch.Groups[2].Value.Trim();

                    if (segDate != dosFormatted) continue;
                    if (string.IsNullOrWhiteSpace(segContent)) continue;

                    // Skip "Other" prefix that ECW adds before notes
                    if (segContent.StartsWith("Other", StringComparison.OrdinalIgnoreCase))
                        segContent = segContent.Substring(5).Trim();

                    // Clean up ICD-10 diagnosis format:
                    // "Pain in leg, unspecified (ICD-10 - M79.606)"
                    // → "Pain in leg, unspecified - M79.606"
                    segContent = System.Text.RegularExpressions.Regex.Replace(
                        segContent,
                        @"\(ICD-10\s*-\s*([A-Z0-9.]+)\)",
                        "- $1");

                    if (!string.IsNullOrWhiteSpace(segContent))
                        result.Add(segContent);
                }
            }

            return result;
        }

        //private static List<string> FilterAssessmentByDos(List<string> rows, string dosFilter)
        //{
        //    if (string.IsNullOrEmpty(dosFilter)) return rows;
        //    if (!DateTime.TryParse(dosFilter, out var dos)) return rows;

        //    string dosFormatted = dos.ToString("MM/dd/yyyy");
        //    var result = new List<string>();

        //    foreach (var row in rows)
        //    {
        //        if (string.IsNullOrWhiteSpace(row)) continue;

        //        // Skip column header rows
        //        if (row.StartsWith("Encounter Date", StringComparison.OrdinalIgnoreCase) ||
        //            row.StartsWith("Diagnosis (ICD", StringComparison.OrdinalIgnoreCase))
        //            continue;

        //        // Check if this row contains embedded dates (old concatenated format)
        //        bool hasEmbeddedDates = System.Text.RegularExpressions.Regex.IsMatch(
        //            row, @"\d{2}/\d{2}/\d{4}");

        //        if (hasEmbeddedDates)
        //        {
        //            // Split by date and extract only DOS segments
        //            var segments = System.Text.RegularExpressions.Regex.Split(
        //                row, @"(?=\d{2}/\d{2}/\d{4})");

        //            foreach (var seg in segments)
        //            {
        //                if (seg.Length < 10) continue;
        //                if (!seg.StartsWith(dosFormatted)) continue;

        //                string diagnosis = seg.Substring(dosFormatted.Length).Trim();
        //                if (!string.IsNullOrEmpty(diagnosis))
        //                    result.Add(diagnosis);
        //            }
        //        }
        //        else
        //        {
        //            // Clean format — already plain text like:
        //            // "1. Pain in leg, unspecified - M79.606 (Primary)"
        //            // "Pending Test" / "Next Appt" etc.
        //            result.Add(row);
        //        }
        //    }

        //    // Safety: if nothing collected, return original rows
        //    return result.Any() ? result : rows
        //        .Where(r => !string.IsNullOrWhiteSpace(r) &&
        //                    !r.StartsWith("Encounter Date", StringComparison.OrdinalIgnoreCase) &&
        //                    !r.StartsWith("Diagnosis (ICD", StringComparison.OrdinalIgnoreCase))
        //        .ToList();
        //}

        private static void RenderAssessment(ColumnDescriptor col, List<string> rows)
        {
            if (!rows.Any()) return;

            var noiseRows = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Pending Test", "Future Test", "Next Appt"
    };

            int diagnosisNum = 1;

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                string text = row.Trim();

                if (noiseRows.Contains(text)) continue;
                if (text.StartsWith("Test NameOrder", StringComparison.OrdinalIgnoreCase)) continue;
                if (text.StartsWith("DetailsProvider", StringComparison.OrdinalIgnoreCase)) continue;

                // ── THIS WAS MISSING — the actual render call ──
                col.Item()
                   .PaddingLeft(12).PaddingTop(3)
                   .Text($"{diagnosisNum++}. {text}")
                   .FontSize(10).LineHeight(1.4f);
            }
        }
        private static void RenderProgressNotes(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                var parts = row.Split('|');

                // Date header row: "06/01/2026 | General Examination | General appearance:"
                if (parts.Length >= 2 &&
                    System.Text.RegularExpressions.Regex.IsMatch(parts[0].Trim(), @"^\d{2}/\d{2}/\d{4}$"))
                {
                    // Skip — date already shown in header; just show the exam type
                    string examType = parts.Length > 1 ? parts[1].Trim() : "";
                    if (!string.IsNullOrEmpty(examType))
                    {
                        col.Item().PaddingLeft(12).PaddingTop(4)
                           .Text(examType).Bold().FontSize(10);
                    }
                    // Show rest of the content after the second pipe
                    if (parts.Length > 2)
                    {
                        string content = string.Join(" | ", parts.Skip(2)).Trim();
                        if (!string.IsNullOrEmpty(content))
                            col.Item().PaddingLeft(16).PaddingTop(1)
                               .Text(content).FontSize(10);
                    }
                }
                else
                {
                    // Sub-rows: "Head: | normo normocephalic..."
                    string label = parts[0].Trim();
                    string content = parts.Length > 1
                        ? string.Join(" ", parts.Skip(1)).Trim().TrimStart('|').Trim()
                        : "";

                    col.Item().PaddingLeft(16).PaddingTop(2).Row(r =>
                    {
                        r.ConstantItem(110)
                         .Text(label).Bold().FontSize(9).FontColor("#444444");
                        r.RelativeItem()
                         .Text(content).FontSize(9).LineHeight(1.4f);
                    });
                }
            }
        }
        private static List<string> FilterVitalsByDos(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dos)) return rows;

            return rows.Where(r =>
            {
                var parts = r.Split('|');
                if (parts.Length < 3) return false;
                if (!DateTime.TryParse(parts[2].Trim(), out var vDate)) return false;
                return vDate.Date == dos.Date;
            }).ToList();
        }
        private static List<string> FilterHpiByDos(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dosDate)) return rows;

            string dosFormatted = dosDate.ToString("MM/dd/yyyy");
            var result = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;

                // Case 1: single row containing multiple concatenated notes
                if (IsMultiNoteConcatenated(row))
                {
                    string extracted = ExtractDosSegmentFromConcatenated(row, dosFormatted);
                    if (!string.IsNullOrEmpty(extracted))
                        result.Add(extracted);
                    continue;
                }

                // Case 2: single row that starts with the DOS date directly
                // e.g. "06/02/2026 | Transition of Care | This is a..."
                if (row.StartsWith(dosFormatted))
                {
                    result.Add(row);
                    continue;
                }

                // Case 3: single row with NO date prefix at all — only one note exists
                // Check if it has any date header pattern at all
                bool hasAnyDateHeader = System.Text.RegularExpressions.Regex.IsMatch(
                    row, @"^\d{2}/\d{2}/\d{4} \|");

                if (!hasAnyDateHeader)
                {
                    // No date prefix — include it (only one note, assume it's current)
                    result.Add(row);
                }
            }

            // Safety fallback — if still empty, return all rows
            return result.Any() ? result : rows;
        }

        private static readonly HashSet<string> DosFilteredSections = new()
        {
            "Progress Notes",
            //"History and Physical Notes",
            "Encounters"   //  encounters to current DOS only
        };

        private static List<string> FilterProgressNotesByDos(List<string> rows, string dosFilter)
        {
            if (string.IsNullOrEmpty(dosFilter)) return rows;
            if (!DateTime.TryParse(dosFilter, out var dosDate)) return rows;

            string dosFormatted = dosDate.ToString("MM/dd/yyyy");

            var result = new List<string>();
            bool inBlock = false;

            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;

                if (IsMultiNoteConcatenated(row))
                {
                    string extracted = ExtractDosSegmentFromConcatenated(row, dosFormatted);
                    if (!string.IsNullOrEmpty(extracted))
                        result.Add(extracted);
                    continue;
                }

                bool isDateHeader = row.Length >= 12 &&
                                    char.IsDigit(row[0]) && row[2] == '/' &&
                                    char.IsDigit(row[3]) && row[5] == '/' &&
                                    row.Contains(" | ");

                if (isDateHeader)
                    inBlock = row.StartsWith(dosFormatted);

                if (inBlock)
                    result.Add(row);
            }

            // STRICT: return empty if no match — do NOT fall back to all rows
            return result;
        }

        // Detects if a single string contains multiple date-prefixed notes concatenated
        private static bool IsMultiNoteConcatenated(string row)
        {
            // Count how many date patterns "MM/dd/yyyy |" appear in the string
            var matches = System.Text.RegularExpressions.Regex.Matches(
                row, @"\d{2}/\d{2}/\d{4} \|");
            return matches.Count > 1;
        }

        // Extracts only the segment starting with dosFormatted date


        // Additional fallback: cut at "This is a XX-year-old" that isn't the first one
        private static string ExtractDosSegmentFromConcatenated(string row, string dosFormatted)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(
                row, @"\d{2}/\d{2}/\d{4} \|");

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (!row.Substring(match.Index).StartsWith(dosFormatted)) continue;

                int start = match.Index;
                int end;

                if (i + 1 < matches.Count)
                {
                    end = matches[i + 1].Index;
                }
                else
                {
                    end = row.Length;

                    // Cut at "agrees to the plan of care." 
                    var planEnd = System.Text.RegularExpressions.Regex.Match(
                        row.Substring(start),
                        @"agrees to the plan of care\.",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    if (planEnd.Success)
                    {
                        end = start + planEnd.Index + planEnd.Length;
                    }
                    else
                    {
                        // Fallback: cut at second occurrence of "This is a XX-year-old"
                        // which signals a new patient note starting
                        var newPatient = System.Text.RegularExpressions.Regex.Matches(
                            row.Substring(start),
                            @"This is a \d+.year.old",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        if (newPatient.Count > 1)
                            end = start + newPatient[1].Index;
                    }
                }

                return row.Substring(start, end - start).Trim();
            }

            return null;
        }

        //private static string ExtractDosSegmentFromConcatenated(string row, string dosFormatted)
        //{
        //    // Find all date positions in the string
        //    var matches = System.Text.RegularExpressions.Regex.Matches(
        //        row, @"\d{2}/\d{2}/\d{4} \|");

        //    for (int i = 0; i < matches.Count; i++)
        //    {
        //        var match = matches[i];

        //        // Check if this date matches the DOS
        //        if (!row.Substring(match.Index).StartsWith(dosFormatted))
        //            continue;

        //        // Extract from this match to the start of the next date (or end of string)
        //        int start = match.Index;
        //        int end = i + 1 < matches.Count
        //            ? matches[i + 1].Index
        //            : row.Length;

        //        return row.Substring(start, end - start).Trim();
        //    }

        //    return null; // DOS date not found in this concatenated string
        //}

        //private static List<string> FilterProgressNotesByDos(List<string> rows, string dosFilter)
        //{

        //    // If no filter provided, return all rows as-is
        //    if (string.IsNullOrEmpty(dosFilter)) return rows;

        //    //if (!DateTime.TryParse(dosFilter, out var dosDate)) return rows;

        //    if (!DateTime.TryParse(dosFilter, out var dosDate))
        //    {
        //        Console.WriteLine($"[FILTER DEBUG] Failed to parse dosFilter: '{dosFilter}'");
        //        return rows; // fallback: show all
        //    }
        //    string dosFormatted = dosDate.ToString("MM/dd/yyyy");
        //    Console.WriteLine($"[FILTER DEBUG] Looking for date: '{dosFormatted}'");



        //    var result = new List<string>();
        //    bool inBlock = false;

        //    foreach (var row in rows)
        //    {
        //        if (string.IsNullOrEmpty(row)) continue;

        //        // Detect date-header rows: start with "MM/dd/yyyy | "
        //        bool isDateHeader = row.Length >= 12 &&
        //                            char.IsDigit(row[0]) && row[2] == '/' &&
        //                            char.IsDigit(row[3]) && row[5] == '/' &&
        //                            row.Contains(" | ");

        //        if (isDateHeader)
        //            inBlock = row.StartsWith(dosFormatted);

        //        if (inBlock)
        //            result.Add(row);
        //    }

        //    // SAFETY: if nothing matched, return ALL rows rather than blank section
        //    return result.Any() ? result : rows;
        //}

        // ── Section renderers ─────────────────────────────────────

        private static void RenderHpiNotes(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                string text = row.Trim();

                // Strip leading date + section label prefix: "06/02/2026 | Transition of Care | actual content"
                // Keep only the last segment after the second pipe
                var parts = text.Split('|');
                if (parts.Length >= 3)
                {
                    // "date | label | content" → show "label:\ncontent"
                    string sectionLabel = parts[1].Trim();
                    string content = string.Join("|", parts.Skip(2)).Trim();

                    if (!string.IsNullOrEmpty(sectionLabel))
                    {
                        col.Item().PaddingLeft(12).PaddingTop(4)
                           .Text(sectionLabel).Bold().FontSize(10).FontColor("#1E4F8C");
                    }
                    col.Item().PaddingLeft(12).PaddingTop(2)
                       .Text(content).FontSize(10).LineHeight(1.5f);
                }
                else
                {
                    col.Item().PaddingLeft(12).PaddingTop(2)
                       .Text(text).FontSize(10).LineHeight(1.5f);
                }
            }
        }
        private static void RenderPlainRows(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                col.Item()
                   .PaddingLeft(12).PaddingTop(2)
                   .Text(row.Trim())
                   .FontSize(10).LineHeight(1.4f);
            }
        }

        private static void RenderPmhList(ColumnDescriptor col, List<string> rows)
        {
            var rendered = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                var parts = row.Split('|');

                // Skip raw "Problem | ..." pipe-delimited rows entirely
                // These are duplicate structured entries — we already have clean rows
                if (parts[0].Trim().Equals("Problem", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Skip "Added On:..." metadata rows
                if (row.TrimStart().StartsWith("Added On:", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Clean condition: remove SNOMED numeric codes e.g. "(32914008)"
                // Keep ICD codes e.g. "(E78.5)" — alphanumeric
                string condition = row.Trim();
                condition = System.Text.RegularExpressions.Regex.Replace(
                    condition, @"\(\d+\)", "").Trim();

                // Remove trailing " | Active | confirmed" etc.
                if (condition.Contains(" | "))
                    condition = condition.Split('|')[0].Trim();

                if (string.IsNullOrEmpty(condition)) continue;

                // Deduplicate
                if (!rendered.Add(condition)) continue;

                col.Item().PaddingLeft(12).PaddingTop(2)
                   .Text($"• {condition}").FontSize(10);
            }
        }
        private static void RenderVitalsTable(ColumnDescriptor col, List<string> rows)
        {
            // Format: "Heart Rate | 67 /min | 06/01/2026"
            col.Item().PaddingLeft(12).PaddingTop(4).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(2);
                });

                // Header
                table.Header(h =>
                {
                    foreach (var hdr in new[] { "Vital", "Value", "Date" })
                        h.Cell().Background("#DCE6F2").Padding(4)
                         .Text(hdr).Bold().FontSize(9);
                });

                foreach (var row in rows)
                {
                    var parts = row.Split('|');
                    string vital = parts.Length > 0 ? parts[0].Trim() : "";
                    string value = parts.Length > 1 ? parts[1].Trim() : "";
                    string date = parts.Length > 2 ? parts[2].Trim() : "";

                    table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                         .Text(vital).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                         .Text(value).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                         .Text(date).FontSize(9);
                }
            });
        }

        private static void RenderResultsTable(ColumnDescriptor col, List<string> rows, string dosFilter)
        {
            // Group rows into panels. A panel starts when a row has no '|' separator
            // (it's a header) or contains "Order date:"
            var panels = new List<(string Header, string OrderDate, List<string> Results)>();
            string currentHeader = null;
            string currentOrderDate = null;
            var currentResults = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                var parts = row.Split('|');
                bool isResultRow = parts.Length >= 2 &&
                                   !row.Contains("Order date:", StringComparison.OrdinalIgnoreCase);

                if (!isResultRow)
                {
                    // Save previous panel
                    if (currentHeader != null)
                        panels.Add((currentHeader, currentOrderDate, new List<string>(currentResults)));

                    // Extract clean panel name and order date from header
                    // e.g. "LIPID PANEL WITH REFLEX TO DIRECT LDL (14852)Order date: 10/14/2024Reviewed..."
                    currentHeader = CleanPanelHeader(row, out currentOrderDate);
                    currentResults = new List<string>();
                }
                else
                {
                    currentResults.Add(row);
                }
            }
            // Save last panel
            if (currentHeader != null)
                panels.Add((currentHeader, currentOrderDate, new List<string>(currentResults)));

            // Filter panels by DOS date
            foreach (var (header, orderDate, results) in panels)
            {
                if (!string.IsNullOrEmpty(dosFilter) && !string.IsNullOrEmpty(orderDate))
                {
                    if (!DateTime.TryParse(orderDate, out var od)) continue;
                    if (!DateTime.TryParse(dosFilter, out var dos)) continue;
                    if (od.Date != dos.Date) continue; // skip panels not from this DOS
                }

                if (!results.Any()) continue;

                // Panel header
                col.Item().PaddingTop(8).PaddingLeft(12)
                   .Text($"{header}" + (string.IsNullOrEmpty(orderDate) ? "" : $"  |  Order date: {orderDate}"))
                   .Bold().FontSize(9).FontColor("#1E4F8C");

                // Result rows as table
                col.Item().PaddingLeft(16).PaddingTop(2).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(4); // Test name
                        c.RelativeColumn(2); // Value
                        c.RelativeColumn(3); // Range
                        c.ConstantColumn(24); // Flag
                    });

                    // Header row
                    table.Header(h =>
                    {
                        foreach (var hdr in new[] { "Test", "Value", "Reference Range", "Flag" })
                            h.Cell().Background("#DCE6F2").Padding(3)
                             .Text(hdr).Bold().FontSize(8);
                    });

                    foreach (var row in results)
                    {
                        var p = row.Split('|');
                        string test = p.Length > 0 ? p[0].Trim() : "";
                        string value = p.Length > 1 ? p[1].Trim() : "";
                        string range = p.Length > 2 ? p[2].Trim() : "";
                        string flag = p.Length > 3 ? p[3].Trim() : "";

                        // Only show single-letter flags (N/H/L), ignore long notes
                        string displayFlag = flag.Length <= 2 ? flag : "";
                        var flagColor = displayFlag == "H" || displayFlag == "L"
                            ? "#CC0000" : "#009900";

                        table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                             .Text(test).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                             .Text(value).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                             .Text(range).FontSize(9).FontColor("#666666");
                        table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(3)
                             .Text(displayFlag).FontSize(9).FontColor(flagColor);
                    }
                });
            }
        }

        // Extracts clean panel name and order date from messy header string
        private static string CleanPanelHeader(string raw, out string orderDate)
        {
            orderDate = null;

            // Extract order date: "Order date: 10/14/2024"
            var orderMatch = System.Text.RegularExpressions.Regex.Match(
                raw, @"Order date:\s*(\d{1,2}/\d{1,2}/\d{4})",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (orderMatch.Success)
                orderDate = orderMatch.Groups[1].Value;

            // Clean panel name: everything before "Order date:" or "(" code
            string name = raw;

            int orderIdx = name.IndexOf("Order date:", StringComparison.OrdinalIgnoreCase);
            if (orderIdx > 0)
                name = name[..orderIdx].Trim();

            // Remove trailing test code like "(14852)"
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\(\d+\)\s*$", "").Trim();

            return name;
        }

        // ── Group banner ──────────────────────────────────────────
        private static void GroupBanner(IContainer container, string title)
        {
            container
                .Background("#DCE6F2")
                .PaddingVertical(5).PaddingHorizontal(8)
                .Text(title).FontSize(12).Bold().FontColor("#1E4F8C");
        }

        // ── Footer ────────────────────────────────────────────────
        private static void ComposeFooter(IContainer container)
        {
            container
                .BorderTop(1).BorderColor("#CCCCCC")
                .PaddingTop(4)
                .Row(row =>
                {
                    row.RelativeItem()
                       .Text("Confidential — For authorized use only")
                       .FontSize(8).Italic().FontColor(Colors.Grey.Medium);

                    row.RelativeItem().AlignRight().Text(txt =>
                    {
                        txt.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                        txt.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                        txt.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                        txt.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
        }

        private static bool HasRealContent(List<string> rows)
        {
            if (rows == null || !rows.Any()) return false;
            return rows.Any(r =>
                !string.IsNullOrWhiteSpace(r) &&
                !r.Trim().Equals("No Information", StringComparison.OrdinalIgnoreCase) &&
                !r.Trim().StartsWith("Encounter Date", StringComparison.OrdinalIgnoreCase) &&
                !r.Trim().StartsWith("Diagnosis (ICD", StringComparison.OrdinalIgnoreCase) &&
                r.Trim().Length > 3);
        }


        public static List<CCDA_ToFileWriteModel> ParseAssessmentFromXml(string ccda, string dosFilter)
        {
            var result = new List<CCDA_ToFileWriteModel>();
            if (string.IsNullOrWhiteSpace(ccda)) return result;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(ccda);
                var ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace("cda", "urn:hl7-org:v3");

                // Parse the DOS filter date once
                DateTime? dosDate = null;
                if (!string.IsNullOrEmpty(dosFilter) &&
                    DateTime.TryParse(dosFilter, out var parsedDos))
                    dosDate = parsedDos;

                // ── Strategy 1: Look for the Problems / Assessment section by LOINC code ──
                // LOINC 11450-4 = Problem list, 51847-2 = Assessment+Plan, 42348-3 = Advance directives
                string[] assessmentCodes = { "51847-2", "11450-4", "10157-6", "42349-1" };

                XmlNode assessSection = null;
                foreach (var code in assessmentCodes)
                {
                    assessSection = doc.SelectSingleNode(
                        $"//cda:section[cda:code[@code='{code}']]", ns);
                    if (assessSection != null) break;
                }

                // Fallback: find by title keywords
                if (assessSection == null)
                {
                    foreach (XmlNode sec in doc.SelectNodes("//cda:section", ns))
                    {
                        string title = sec.SelectSingleNode("cda:title", ns)?.InnerText ?? "";
                        if (title.IndexOf("Assessment", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            title.IndexOf("Problem", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            title.IndexOf("Diagnosis", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            assessSection = sec;
                            break;
                        }
                    }
                }

                if (assessSection == null) return result;

                // ── Parse structured entries (observation/act with codes) ──
                var entries = assessSection.SelectNodes(".//cda:observation | .//cda:act", ns);

                foreach (XmlNode entry in entries)
                {
                    // Get the diagnosis code and display name
                    var valueNode = entry.SelectSingleNode("cda:value", ns);
                    var codeNode = entry.SelectSingleNode("cda:code", ns);

                    string displayName = valueNode?.Attributes?["displayName"]?.Value
                                      ?? codeNode?.Attributes?["displayName"]?.Value;
                    string icdCode = valueNode?.Attributes?["code"]?.Value
                                      ?? codeNode?.Attributes?["code"]?.Value;
                    string codeSystem = valueNode?.Attributes?["codeSystem"]?.Value
                                      ?? codeNode?.Attributes?["codeSystem"]?.Value;

                    // Only ICD-10 codes (codeSystem 2.16.840.1.113883.6.90) or SNOMED
                    bool isIcd = codeSystem == "2.16.840.1.113883.6.90";
                    bool isSnomed = codeSystem == "2.16.840.1.113883.6.96";
                    if (!isIcd && !isSnomed && string.IsNullOrEmpty(displayName)) continue;

                    // Try to get effective date from this entry
                    string effectiveDate = null;
                    var effTime = entry.SelectSingleNode("cda:effectiveTime", ns)
                               ?? entry.SelectSingleNode("cda:effectiveTime/cda:low", ns);
                    string rawDate = effTime?.Attributes?["value"]?.Value
                                  ?? effTime?.Attributes?["low"]?.Value;

                    if (!string.IsNullOrEmpty(rawDate) && rawDate.Length >= 8)
                    {
                        // CCDA dates are yyyyMMdd or yyyyMMddHHmmss
                        string dateStr = rawDate.Substring(0, 8);
                        if (DateTime.TryParseExact(dateStr, "yyyyMMdd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out var entryDate))
                        {
                            effectiveDate = entryDate.ToString("MM/dd/yyyy");

                            // DOS filter: skip entries not matching the visit date
                            if (dosDate.HasValue && entryDate.Date != dosDate.Value.Date)
                                continue;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(displayName)) continue;

                    // Format: "Diagnosis Name - ICD10Code"
                    string formatted = isIcd && !string.IsNullOrEmpty(icdCode)
                        ? $"{displayName} - {icdCode}"
                        : displayName;

                    result.Add(new CCDA_ToFileWriteModel
                    {
                        Section = "MDM or Assessment and Plan",
                        Value = formatted
                    });
                }

                // ── Fallback: if no structured entries found, try the narrative text ──
                if (!result.Any())
                {
                    var textNode = assessSection.SelectSingleNode("cda:text", ns);
                    if (textNode != null)
                    {
                        // Parse <item> or <td> elements inside the narrative
                        var items = textNode.SelectNodes(".//cda:item | .//cda:td", ns);
                        foreach (XmlNode item in items)
                        {
                            string text = item.InnerText?.Trim();
                            if (!string.IsNullOrWhiteSpace(text) && text.Length > 3)
                                result.Add(new CCDA_ToFileWriteModel
                                {
                                    Section = "MDM or Assessment and Plan",
                                    Value = text
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ParseAssessmentFromXml ERROR] {ex.Message}");
            }

            return result;
        }

    }
}
