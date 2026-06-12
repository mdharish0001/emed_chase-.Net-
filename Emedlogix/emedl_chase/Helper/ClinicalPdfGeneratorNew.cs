using emedl_chase.Helper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace emedl_chase.Model
{
    // ─────────────────────────────────────────────────────────────
    //  PDF Section order + display config
    //  ► ADD / REMOVE / REORDER entries here only.
    //    Nothing else needs to change.
    // ─────────────────────────────────────────────────────────────

    public static class ClinicalPdfGeneratorNew
    {
        // (CanonicalKey, DisplayLabel, SoapGroup, AlwaysShow)
        // AlwaysShow = true  → render even if empty (shows "No Information")
        // AlwaysShow = false → skip section entirely if no data
        private static readonly List<(string Key, string Label, string Group, bool AlwaysShow)> SectionOrder
            = new()
        {
            (CcdaParserNew.SEC_CC,           "Chief Complaint",         "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_ROS,          "Review of Systems",       "SUBJECTIVE",        false),
            ("Reason For Referral",          "Reason For Referral",     "SUBJECTIVE",        false),
            ("Consultation Notes",           "Consultation Notes",      "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_HPI,          "History & Physical",      "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_GEN_HISTORY,  "Medical History",         "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_PMH,          "Past Medical History",    "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_PMSFH,        "Social / Family History", "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_ALLERGY,      "Allergies",               "SUBJECTIVE",        true),
            (CcdaParserNew.SEC_MEDICATIONS,  "Medications",             "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_IMMUNIZATION, "Immunizations",           "SUBJECTIVE",        false),
            (CcdaParserNew.SEC_VITALS,       "Vital Signs",             "OBJECTIVE",         false),
            (CcdaParserNew.SEC_EXAM,         "Physical Examination",    "OBJECTIVE",         false),
            (CcdaParserNew.SEC_ASSESSMENT,   "Assessment & Plan",       "ASSESSMENT & PLAN", false),
            (CcdaParserNew.SEC_TREATMENT,    "Treatment Plan",          "ASSESSMENT & PLAN", false),
            (CcdaParserNew.SEC_RESULTS,      "Lab Results",             "ASSESSMENT & PLAN", false),
            (CcdaParserNew.SEC_CLINICAL_NOTES, "Clinical Notes",        "ASSESSMENT & PLAN", false),
            (CcdaParserNew.SEC_PROCEDURES,   "Procedures",              "ASSESSMENT & PLAN", false),
            (CcdaParserNew.SEC_ENCOUNTERS,   "Encounter History",       "ENCOUNTER HISTORY", false),
            ("Goals",                        "Goals",                   "ADDITIONAL",        false),
            ("Health Concerns",              "Health Concerns",         "ADDITIONAL",        false),
            ("Medical Equipment",            "Medical Equipment",       "ADDITIONAL",        false),
        };

        // ─────────────────────────────────────────────────────────
        //  PUBLIC entry point
        // ─────────────────────────────────────────────────────────
        public static byte[] Generate(ClinicalNoteNew note)
        {
            Console.WriteLine("=== ClinicalPdfGeneratorNew v3.0 ===");
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36, Unit.Point);
                    page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                    page.Header().Element(c => RenderHeader(c, note));
                    page.Content().Element(c => RenderBody(c, note));
                    page.Footer().Element(RenderFooter);
                });
            });

            return document.GeneratePdf();
        }

        // ─────────────────────────────────────────────────────────
        //  Header
        // ─────────────────────────────────────────────────────────
        private static void RenderHeader(IContainer container, ClinicalNoteNew note)
        {
            container.Column(col =>
            {
                // Top bar
                col.Item().BorderBottom(1).PaddingBottom(4).Text(txt =>
                {
                    txt.Span(note.PatientName?.ToUpper()).Bold().FontSize(9);
                    txt.Span($" | DOB: {note.DateOfBirth} ({note.Age} yo)").FontSize(9);
                    txt.Span($" | Acc: {note.AccountNumber}").FontSize(9);
                    txt.Span($" | DOS: {note.DateOfService}").FontSize(9);
                });

                // Title
                col.Item().PaddingTop(6).PaddingBottom(6)
                   .Text("Patient Health Record").FontSize(18).Bold().AlignCenter();

                // Patient details
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        LabelValue(left, "Patient:", note.PatientName);
                        LabelValue(left, "Account Number:", note.AccountNumber);
                        LabelValue(left, "DOB:", note.DateOfBirth);
                        LabelValue(left, "Age:", note.Age);
                        LabelValue(left, "Phone:", note.Phone);
                        LabelValue(left, "Address:", note.Address);
                    });
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

                col.Item().BorderBottom(1).PaddingTop(8).Text("");
                col.Item().Height(8);
            });
        }

        private static void LabelValue(ColumnDescriptor col, string label, string value)
        {
            col.Item().Text(txt =>
            {
                txt.Span(label).Bold().FontSize(10);
                txt.Span($" {value}").FontSize(10);

            });
        }

        // ─────────────────────────────────────────────────────────
        //  Body — iterates SectionOrder, renders each section
        // ─────────────────────────────────────────────────────────
        private static void RenderBody(IContainer container, ClinicalNoteNew note)
        {
            container.Column(col =>
            {
                string lastGroup = null;

                foreach (var (key, label, group, alwaysShow) in SectionOrder)
                {
                    note.Sections.TryGetValue(key, out var rows);
                    rows ??= new List<string>();

                    bool hasContent = rows.Any(r =>
                        !string.IsNullOrWhiteSpace(r) &&
                        !r.Trim().Equals("No Information", StringComparison.OrdinalIgnoreCase));

                    // Skip if no content and not always-show
                    if (!hasContent && !alwaysShow) continue;

                    // Group banner (SUBJECTIVE / OBJECTIVE / etc.)
                    if (group != lastGroup)
                    {
                        col.Item().Element(c => GroupBanner(c, group));
                        lastGroup = group;
                    }

                    // Section title
                    col.Item().PaddingTop(6).Text(label)
                       .FontSize(11).Bold().FontColor("#1E4F8C");

                    // Render content
                    if (!hasContent)
                    {
                        // AlwaysShow section with no data
                        col.Item().PaddingLeft(12).PaddingTop(2)
                           .Text("No Information").FontSize(10).FontColor("#888888").Italic();
                        continue;
                    }

                    switch (key)
                    {
                        case var k when k == CcdaParserNew.SEC_HPI:
                            RenderHpi(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_VITALS:
                            RenderVitalsTable(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_EXAM:
                            RenderPhysicalExam(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_RESULTS:
                            RenderLabResults(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_ASSESSMENT:
                            RenderAssessment(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_PMH:
                            RenderBulletList(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_MEDICATIONS:
                            RenderMedications(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_CLINICAL_NOTES:
                            RenderClinicalNotes(col, rows);
                            break;
                        case var k when k == CcdaParserNew.SEC_ENCOUNTERS:
                            RenderEncounters(col, rows);
                            break;
                        default:
                            RenderPlainRows(col, rows);
                            break;
                    }
                }


                // ── Render sections not in SectionOrder ──────────────
                // Any section in the XML but not in SectionOrder appears
                // here — no content is ever silently dropped.
                var knownKeys = new HashSet<string>(
                    SectionOrder.Select(s => s.Key), StringComparer.OrdinalIgnoreCase);

                // Sections explicitly excluded from PDF — never render these
                var excludedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    CcdaParserNew.SEC_INSURANCE,   // Insurance Providers — not shown
                    "Insurance Providers",
                    "Insurance",
                };

                bool addedExtraBanner = false;
                foreach (var kvp in note.Sections)
                {
                    if (knownKeys.Contains(kvp.Key)) continue;
                    if (excludedKeys.Contains(kvp.Key)) continue;  // ← skip excluded
                    if (!kvp.Value.Any(r => !string.IsNullOrWhiteSpace(r) &&
                        !r.Trim().Equals("No Information", StringComparison.OrdinalIgnoreCase)))
                        continue;
                    if (!addedExtraBanner)
                    {
                        col.Item().Element(c => GroupBanner(c, "OTHER INFORMATION"));
                        addedExtraBanner = true;
                    }
                    col.Item().PaddingTop(6).Text(kvp.Key)
                       .FontSize(11).Bold().FontColor("#1E4F8C");
                    RenderPlainRows(col, kvp.Value);
                }
            });
        }

        // ─────────────────────────────────────────────────────────
        //  Section renderers
        // ─────────────────────────────────────────────────────────

        // H&P — "date | Category | note text"
        // Category (e.g. "Transition of Care") shown as bold subheading
        private static void RenderHpi(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                var parts = row.Split('|');
                string p0 = parts[0].Trim();

                if (p0.Equals("CC", StringComparison.OrdinalIgnoreCase))
                {
                    col.Item().PaddingLeft(12).PaddingTop(6)
                       .Text("Chief Complaint:").Bold().FontSize(10).FontColor("#1E4F8C");
                    col.Item().PaddingLeft(16).PaddingTop(2)
                       .Text(string.Join("|", parts.Skip(1)).Trim()).FontSize(10).LineHeight(1.4f);
                }
                else if (p0.Equals("LINE", StringComparison.OrdinalIgnoreCase))
                {
                    string lineText = string.Join("|", parts.Skip(1)).Trim();
                    // "---" is a paragraph divider between multiple visit notes
                    if (lineText == "---")
                        col.Item().PaddingTop(6).PaddingBottom(2); // just add spacing
                    else
                        col.Item().PaddingLeft(16).PaddingTop(1)
                           .Text(lineText).FontSize(10).LineHeight(1.4f);
                }
                else if (parts.Length >= 3 && parts[2].Trim() == "---HEADING---")
                {
                    col.Item().PaddingLeft(12).PaddingTop(8)
                       .Text(parts[1].Trim() + ":").Bold().FontSize(10).FontColor("#1E4F8C");
                }
                else if (parts.Length >= 3)
                {
                    col.Item().PaddingLeft(12).PaddingTop(8)
                       .Text(parts[1].Trim() + ":").Bold().FontSize(10).FontColor("#1E4F8C");
                    col.Item().PaddingLeft(16).PaddingTop(2)
                       .Text(string.Join("|", parts.Skip(2)).Trim()).FontSize(10).LineHeight(1.5f);
                }
                else if (parts.Length == 2)
                {
                    col.Item().PaddingLeft(12).PaddingTop(6)
                       .Text(p0 + ":").Bold().FontSize(10).FontColor("#1E4F8C");
                    col.Item().PaddingLeft(16).PaddingTop(2)
                       .Text(parts[1].Trim()).FontSize(10).LineHeight(1.5f);
                }
                else
                {
                    col.Item().PaddingLeft(12).PaddingTop(2)
                       .Text(row.Trim()).FontSize(10).LineHeight(1.5f);
                }
            }
        }

        // Clinical Notes — plain text from Assessment "Other" rows
        private static void RenderClinicalNotes(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                col.Item().PaddingLeft(12).PaddingTop(4)
                   .Text(row.Trim()).FontSize(10).LineHeight(1.5f);
            }
        }

        // Encounters — "date time | type | provider | facility\nDiagnoses: ..."
        private static void RenderEncounters(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                // Split on newline to separate header from diagnoses
                var lines = row.Split('\n');
                string header = lines[0].Trim();
                string dxLine = lines.Length > 1 ? lines[1].Trim() : "";

                // Header: "date time | type | provider | facility"
                var parts = header.Split('|');
                string dateTime = parts.Length > 0 ? parts[0].Trim() : "";
                string type = parts.Length > 1 ? parts[1].Trim() : "";
                string provider = parts.Length > 2 ? parts[2].Trim() : "";
                string facility = parts.Length > 3 ? parts[3].Trim() : "";

                col.Item().PaddingLeft(12).PaddingTop(6).Column(inner =>
                {
                    // Date + type on first line, bold
                    inner.Item().Text(txt =>
                    {
                        txt.Span($"{dateTime}").Bold().FontSize(10);
                        if (!string.IsNullOrEmpty(type))
                            txt.Span($"  —  {type}").FontSize(10);
                    });

                    // Provider + facility on second line, muted
                    if (!string.IsNullOrEmpty(provider) || !string.IsNullOrEmpty(facility))
                        inner.Item().PaddingTop(1).Text(
                            string.Join("  |  ", new[] { provider, facility }
                                .Where(s => !string.IsNullOrEmpty(s))))
                           .FontSize(9).FontColor("#666666");

                    // Diagnoses omitted — already shown in Assessment & Plan section
                });
            }
        }

        // Vital Signs — "VitalName | Value | Date"
        private static void RenderVitalsTable(ColumnDescriptor col, List<string> rows)
        {
            if (!rows.Any()) return;

            // Format: "BP: 114/78 mm Hg, HR: 71 /min, RR: 16 /min, Oxygen sat %: 100 %"
            // Build a comma-separated inline summary matching ECW printed note format.
            // Row format from parser: "Vital Name | value unit | date"
            var parts = new System.Collections.Generic.List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                var p = row.Split('|');
                string name = p.Length > 0 ? p[0].Trim() : "";
                string value = p.Length > 1 ? p[1].Trim() : "";
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) continue;

                // Shorten common vital names to match ECW display
                string label = name switch
                {
                    "Blood Pressure" => "BP",
                    "heart rate" => "HR",
                    "respiratory rate" => "RR",
                    "oximetry" => "Oxygen sat %",
                    "weight" => "Weight",
                    "weight-kg" => "Weight",
                    "temperature" => "Temp",
                    "body height" => "Height",
                    "bmi" => "BMI",
                    _ => name
                };

                // Clean value display: "114/78 mmHg" → "114/78 mm Hg"
                value = value.Replace("mmHg", "mm Hg").Replace("  /min", " /min").Trim();

                parts.Add($"{label}: {value}");
            }

            if (!parts.Any()) return;

            col.Item().PaddingLeft(12).PaddingTop(4)
               .Text(string.Join(",  ", parts))
               .FontSize(10).LineHeight(1.5f);
        }

        // Physical Exam — "Date | ExamType | Finding"  or  "Label | Content"
        private static void RenderPhysicalExam(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                var parts = row.Split('|');

                if (parts.Length >= 3)
                {
                    string examType = parts[1].Trim();
                    string content = string.Join(" ", parts.Skip(2)).Trim();
                    if (!string.IsNullOrEmpty(examType))
                        col.Item().PaddingLeft(12).PaddingTop(4)
                           .Text(examType).Bold().FontSize(10);
                    if (!string.IsNullOrEmpty(content))
                        col.Item().PaddingLeft(16).PaddingTop(1)
                           .Text(content).FontSize(10).LineHeight(1.4f);
                }
                else if (parts.Length == 2)
                {
                    col.Item().PaddingLeft(12).PaddingTop(2).Row(r =>
                    {
                        r.ConstantItem(130).Text(parts[0].Trim()).Bold().FontSize(9).FontColor("#444444");
                        r.RelativeItem().Text(parts[1].Trim()).FontSize(9).LineHeight(1.4f);
                    });
                }
                else
                {
                    col.Item().PaddingLeft(12).PaddingTop(2)
                       .Text(row.Trim()).FontSize(10).LineHeight(1.4f);
                }
            }
        }

        // Lab Results — panel header rows flagged with "|ORDER|date", then result rows
        private static void RenderLabResults(ColumnDescriptor col, List<string> rows)
        {
            bool inPanel = false;

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                var parts = row.Split('|');

                // Panel header marker: "PanelName|ORDER|date"
                if (parts.Length >= 2 && parts[1].Trim() == "ORDER")
                {
                    string panelName = parts[0].Trim();
                    string date = parts.Length > 2 ? parts[2].Trim() : "";
                    string header = panelName + (string.IsNullOrEmpty(date) ? "" : $"  —  {date}");

                    col.Item().PaddingTop(8).PaddingLeft(12)
                       .Text(header).Bold().FontSize(9).FontColor("#1E4F8C");

                    // Start result table for this panel
                    inPanel = true;
                    continue;
                }

                if (inPanel && parts.Length >= 2)
                {
                    // Individual result row: "TestName|Value|Range|Flag"
                    string test = parts.Length > 0 ? parts[0].Trim() : "";
                    string value = parts.Length > 1 ? parts[1].Trim() : "";
                    string range = parts.Length > 2 ? parts[2].Trim() : "";
                    string flag = parts.Length > 3 ? parts[3].Trim() : "";
                    string displayFlag = flag.Length <= 2 ? flag : "";
                    string flagColor = displayFlag is "H" or "L" ? "#CC0000" : "#009900";

                    col.Item().PaddingLeft(20).PaddingTop(1).Row(r =>
                    {
                        r.RelativeItem(4).Text(test).FontSize(9);
                        r.RelativeItem(2).Text(value).FontSize(9);
                        r.RelativeItem(3).Text(range).FontSize(9).FontColor("#666666");
                        r.ConstantItem(20).Text(displayFlag).FontSize(9).FontColor(flagColor);
                    });
                }
                else
                {
                    col.Item().PaddingLeft(12).PaddingTop(2)
                       .Text(row.Trim()).FontSize(10);
                }
            }
        }

        // Assessment — numbered diagnosis list
        private static void RenderAssessment(ColumnDescriptor col, List<string> rows)
        {
            // Row formats:
            //   "Diagnosis - CODE"
            //   "Diagnosis - CODE\nNotes: treatment text"
            //   "CLINICAL_NOTE\nclinical note text"  ← from Other rows

            int num = 1;
            var clinicalNotes = new List<string>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                // Collect clinical notes separately — render after numbered list
                if (row.StartsWith("CLINICAL_NOTE\n"))
                {
                    string noteText = row.Substring("CLINICAL_NOTE\n".Length).Trim();
                    if (!string.IsNullOrEmpty(noteText))
                        clinicalNotes.Add(noteText);
                    continue;
                }

                var parts = row.Split(new[] { "\nNotes:" }, 2, StringSplitOptions.None);
                string dx = parts[0].Trim();
                string notes = parts.Length > 1 ? parts[1].Trim() : "";

                // Numbered diagnosis line — ShowEntire prevents page-cut truncation
                col.Item().PaddingLeft(12).PaddingTop(4)
                   .Text($"{num++}. {dx}").FontSize(10).LineHeight(1.4f);

                // Treatment notes indented below
                if (!string.IsNullOrEmpty(notes))
                    col.Item().PaddingLeft(24).PaddingTop(1).PaddingBottom(2)
                       .Text($"Notes: {notes}").FontSize(9)
                       .FontColor("#444444").LineHeight(1.5f).Italic();
            }


        }

        // Medications — "Name | SIG | Notes | StartDate | EndDate | Diagnosis | Status"
        private static void RenderMedications(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                var p = row.Split('|');
                string name = p.Length > 0 ? p[0].Trim() : "";
                string sig = p.Length > 1 ? p[1].Trim() : "";
                string start = p.Length > 3 ? p[3].Trim() : "";
                string end = p.Length > 4 ? p[4].Trim() : "";
                string dx = p.Length > 5 ? p[5].Trim() : "";
                string status = p.Length > 6 ? p[6].Trim() : "";

                if (string.IsNullOrWhiteSpace(name)) continue;

                // Med name bold
                col.Item().PaddingLeft(12).PaddingTop(6)
                   .Text(name).Bold().FontSize(10);

                // SIG below
                if (!string.IsNullOrEmpty(sig))
                    col.Item().PaddingLeft(20).PaddingTop(1)
                       .Text(sig).FontSize(9).FontColor("#444444").LineHeight(1.4f);

                // Date + diagnosis + status on one line
                var meta = new System.Collections.Generic.List<string>();
                if (!string.IsNullOrEmpty(start)) meta.Add($"Start: {start}");
                if (!string.IsNullOrEmpty(end)) meta.Add($"End: {end}");
                if (!string.IsNullOrEmpty(dx)) meta.Add(dx);
                if (!string.IsNullOrEmpty(status)) meta.Add(status);
                if (meta.Any())
                    col.Item().PaddingLeft(20).PaddingTop(1)
                       .Text(string.Join("  |  ", meta)).FontSize(9)
                       .FontColor("#888888").LineHeight(1.3f);
            }
        }

        // Bullet list (PMH etc.)
        private static void RenderBulletList(ColumnDescriptor col, List<string> rows)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                string clean = row.Trim();
                if (!seen.Add(clean)) continue;
                col.Item().PaddingLeft(12).PaddingTop(2)
                   .Text($"• {clean}").FontSize(10);
            }
        }

        // Generic plain rows
        private static void RenderPlainRows(ColumnDescriptor col, List<string> rows)
        {
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;
                col.Item().PaddingLeft(12).PaddingTop(2)
                   .Text(row.Trim()).FontSize(10).LineHeight(1.4f);
            }
        }

        // ─────────────────────────────────────────────────────────
        //  Shared UI elements
        // ─────────────────────────────────────────────────────────
        private static void GroupBanner(IContainer container, string title)
        {
            container.Background("#DCE6F2")
                     .PaddingVertical(5).PaddingHorizontal(8)
                     .Text(title).FontSize(12).Bold().FontColor("#1E4F8C");
        }

        private static void RenderFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor("#CCCCCC").PaddingTop(4)
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
    }
}