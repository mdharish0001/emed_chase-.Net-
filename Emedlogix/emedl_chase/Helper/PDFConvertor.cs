using emedl_chase.Model;
using Hl7.FhirPath.Sprache;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace emedl_chase.Helper
{
    public class PDFConvertor
    {

        public static readonly Dictionary<string, string> HeaderMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    // ── ECW nested caption names ──────────────────────────────────────
    { "Chief Complaints",                       "Chief Complaint"  },
    { "Chief Complaint",                        "Chief Complaint"  },
    { "Reason for Visit",                       "Chief Complaint"  },
    { "Chief Complaint / Reason for Visit",     "Chief Complaint"  },
    { "Presenting Problem",                     "Chief Complaint"  },

    { "HPI",                                    "HPI"              },
    { "History of Present Illness",             "HPI"              },

    { "ROS",                                    "ROS"              },
    { "Review of Systems",                      "ROS"              },

    { "Examination",                            "Physical Exam"    },  // ← ECW uses this
    { "Physical Exam",                          "Physical Exam"    },
    { "Physical Exam Findings",                 "Physical Exam"    },
    { "Physical Examination",                   "Physical Exam"    },

    { "Subjective",                             "Chief Complaint"  },  // parent section
    { "Objective",                              "Physical Exam"    },  // parent section

    { "Past Medical History",                   "PMH"              },
    { "Medical History",                        "PMH"              },
    { "PMH",                                    "PMH"              },

    { "Past Medical / Surgical / Family History","PMSFH"           },
    { "Social History",                         "PMSFH"            },
    { "Family History",                         "PMSFH"            },

    { "Allergies",                              "Allergy"          },
    { "Allergies and Adverse Reactions",        "Allergy"          },
    { "No Known Allergies",                     "Allergy"          },

    { "Assessment and Plan",                    "MDM or Assessment and Plan" },
    { "Assessment",                             "MDM or Assessment and Plan" },
    { "Plan",                                   "MDM or Assessment and Plan" },
    { "Plan of Care",                           "MDM or Assessment and Plan" },

    { "Transition of Care",                     "Treatment Plan"   },  // ← ECW uses this
    { "Treatment Plan",                         "Treatment Plan"   },

    { "Electronic Signature",                   "Electronically Signed By" },
    { "Signatures",                             "Electronically Signed By" },
};
        public static readonly Dictionary<string, string> LoincToHeaderMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "46239-0", "Chief Complaint" },   // Chief complaint+Reason for visit
            { "10154-3", "Chief Complaint" },   // Chief complaint Narrative
            { "29299-5", "Chief Complaint" },   // Reason for visit
            { "11348-0", "PMH"             },   // History of Past Illness
            { "10164-2", "HPI"             },   // History of Present Illness
            { "10187-3", "ROS"             },   // Review of Systems
            { "47420-5", "Physical Exam"   },   // Physical findings
            { "11450-4", "PMH"             },   // Problem list
            { "48765-2", "Allergy"         },   // Allergies
            { "18776-5", "Treatment Plan"  },   // Plan of care
            { "51848-0", "MDM or Assessment and Plan" }, // Assessment note
            { "42348-3", "MDM or Assessment and Plan" }, // Advance directives
            { "29545-1", "Physical Exam"   },   // Physical findings
            { "55110-1", "MDM or Assessment and Plan" }, // Conclusions
        };

        public static readonly List<string> StandardHeaderOrder = new List<string>
        {
            "Chief Complaint",
            "Reason for Visit",
            "HPI",
            "ROS",
            "PMH",
            "PMSFH",
            "Allergy",
            "Physical Exam",
            "MDM or Assessment and Plan",
            "Orders",
            "Treatment Plan",
            "Referrals",
            "CC to Allergy - Subjective",
            "PE - Objective",
            "Format 1 - History and Physical / Consult Notes / Progress Notes / Visit Notes",
            "Format 2 - SOAP Notes",
            "Electronically Signed By"
        };


         //   public static List<CCDA_ToFileWriteModel> ParseCCDASectionsFromString(
     //string ccdaXml,
     //string dos = null)
     //   {
     //       XmlDocument doc = new XmlDocument();
     //       doc.LoadXml(ccdaXml);
     //       XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
     //       ns.AddNamespace("cda", "urn:hl7-org:v3");

     //       DateTime? targetDate = null;
     //       if (!string.IsNullOrEmpty(dos) &&
     //           DateTime.TryParseExact(dos, "yyyy-MM-dd",
     //               CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime td))
     //           targetDate = td;

     //       // ── STEP 1: Check document-level effectiveTime ────────────────────────
     //       // If this CCDA document itself is for a different date, return empty
     //       if (targetDate.HasValue)
     //       {
     //           string docEffectiveTime = doc
     //               .SelectSingleNode("//cda:ClinicalDocument/cda:effectiveTime", ns)
     //               ?.Attributes?["value"]?.Value;

     //           Console.WriteLine($"CCDA document effectiveTime: {docEffectiveTime}");

     //           if (!string.IsNullOrEmpty(docEffectiveTime))
     //           {
     //               string docDatePart = docEffectiveTime.Length >= 8
     //                   ? docEffectiveTime.Substring(0, 8) : docEffectiveTime;

     //               if (DateTime.TryParseExact(docDatePart, "yyyyMMdd",
     //                   CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime docDate))
     //               {
     //                   if (docDate.Date != targetDate.Value.Date)
     //                   {
     //                       Console.WriteLine($"CCDA document date {docDate:yyyy-MM-dd} " +
     //                                         $"does not match DOS {dos} — returning empty");
     //                       return new List<CCDA_ToFileWriteModel>();
     //                   }
     //               }
     //           }
     //       }

     //       var sectionDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
     //       XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", ns);
     //       if (sectionNodes == null) return new List<CCDA_ToFileWriteModel>();

     //       foreach (XmlNode section in sectionNodes)
     //       {
     //           string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();
     //           string loincCode = section.SelectSingleNode("cda:code", ns)
     //                                     ?.Attributes?["code"]?.Value?.Trim();
     //           string header = ResolveHeader(rawTitle, loincCode) ?? rawTitle;

     //           if (string.IsNullOrEmpty(header)) continue;

     //           bool isDateSensitive = IsDateSensitiveSection(header);
     //           XmlNode textNode = section.SelectSingleNode("cda:text", ns);
     //           if (textNode == null) continue;

     //           var lines = new List<string>();

     //           // ── Parse <paragraph><caption> subsections (ECW format) ───────────
     //           XmlNodeList paragraphs = textNode.SelectNodes(".//cda:paragraph", ns);
     //           bool hasSubSections = false;

     //           if (paragraphs != null && paragraphs.Count > 0)
     //           {
     //               foreach (XmlNode para in paragraphs)
     //               {
     //                   string caption = para.SelectSingleNode("cda:caption", ns)?.InnerText?.Trim();
     //                   string paraHeader = ResolveHeader(caption, null) ?? caption ?? header;
     //                   var paraLines = new List<string>();

     //                   // <list><item> numbered items
     //                   XmlNodeList listItems = para.SelectNodes(".//cda:list/cda:item", ns);
     //                   if (listItems != null && listItems.Count > 0)
     //                   {
     //                       int i = 1;
     //                       foreach (XmlNode li in listItems)
     //                       {
     //                           string val = li.InnerText?.Trim();
     //                           if (!string.IsNullOrWhiteSpace(val))
     //                               paraLines.Add($"{i++}. {val}");
     //                       }
     //                   }

     //                   // <content> tags
     //                   XmlNodeList contents = para.SelectNodes("cda:content", ns);
     //                   if (contents != null)
     //                   {
     //                       foreach (XmlNode cn in contents)
     //                       {
     //                           string val = cn.InnerText?.Trim();
     //                           if (!string.IsNullOrWhiteSpace(val) && !paraLines.Contains(val))
     //                               paraLines.Add(val);
     //                       }
     //                   }

     //                   // Plain text fallback
     //                   if (paraLines.Count == 0)
     //                   {
     //                       string plain = para.InnerText?.Trim();
     //                       if (!string.IsNullOrWhiteSpace(plain) && plain != caption)
     //                           paraLines.Add(plain);
     //                   }

     //                   if (paraLines.Count > 0)
     //                   {
     //                       if (!sectionDict.ContainsKey(paraHeader))
     //                           sectionDict[paraHeader] = new List<string>();
     //                       sectionDict[paraHeader].AddRange(paraLines);
     //                       hasSubSections = true;
     //                   }
     //               }
     //           }

     //           // ── For date-sensitive sections: filter <entry> by effectiveTime ──
     //           if (isDateSensitive && targetDate.HasValue)
     //           {
     //               XmlNodeList entries = section.SelectNodes("cda:entry", ns);
     //               if (entries != null && entries.Count > 0)
     //               {
     //                   foreach (XmlNode entry in entries)
     //                   {
     //                       // Check if this entry matches the DOS
     //                       bool matches = EntryMatchesDate(entry, ns, targetDate.Value);
     //                       Console.WriteLine($"Entry in '{header}' — matchesDate={matches}");

     //                       if (!matches) continue;

     //                       var entryLines = ExtractTextFromNode(entry, ns);
     //                       if (entryLines.Count > 0)
     //                       {
     //                           if (!sectionDict.ContainsKey(header))
     //                               sectionDict[header] = new List<string>();
     //                           sectionDict[header].AddRange(entryLines);
     //                           hasSubSections = true;
     //                       }
     //                   }
     //               }
     //           }

     //           // ── Table rows ────────────────────────────────────────────────────
     //           if (!hasSubSections)
     //           {
     //               XmlNodeList tableRows = textNode.SelectNodes(".//cda:tbody/cda:tr", ns);
     //               if (tableRows != null && tableRows.Count > 0)
     //               {
     //                   foreach (XmlNode row in tableRows)
     //                   {
     //                       // Per-row date check for date-sensitive sections
     //                       if (isDateSensitive && targetDate.HasValue)
     //                       {
     //                           // Date is usually in first <td>
     //                           string firstCell = row.SelectSingleNode(".//cda:td[1]", ns)
     //                                                ?.InnerText?.Trim();
     //                           if (!string.IsNullOrEmpty(firstCell) &&
     //                               !RowDateMatches(firstCell, targetDate.Value))
     //                           {
     //                               Console.WriteLine($"Skipping row — date '{firstCell}' " +
     //                                                 $"doesn't match DOS {dos}");
     //                               continue;
     //                           }
     //                       }

     //                       var cells = row.SelectNodes(".//cda:td", ns);
     //                       if (cells == null) continue;
     //                       var parts = new List<string>();
     //                       foreach (XmlNode cell in cells)
     //                       {
     //                           string val = cell.InnerText?.Trim();
     //                           if (!string.IsNullOrWhiteSpace(val)) parts.Add(val);
     //                       }
     //                       if (parts.Count > 0)
     //                       {
     //                           if (!sectionDict.ContainsKey(header))
     //                               sectionDict[header] = new List<string>();
     //                           sectionDict[header].Add(string.Join(" | ", parts));
     //                           hasSubSections = true;
     //                       }
     //                   }
     //               }
     //           }

     //           // ── Plain text fallback ───────────────────────────────────────────
     //           if (!hasSubSections)
     //           {
     //               foreach (XmlNode tn in textNode.SelectNodes(".//text()", ns)
     //                                              ?? new XmlDocument().ChildNodes)
     //               {
     //                   string val = tn.InnerText?.Trim();
     //                   if (!string.IsNullOrWhiteSpace(val))
     //                   {
     //                       if (!sectionDict.ContainsKey(header))
     //                           sectionDict[header] = new List<string>();
     //                       sectionDict[header].Add(val);
     //                   }
     //               }
     //           }
     //       }

     //       // ── Emit in StandardHeaderOrder only, skip empty ──────────────────────
     //       var result = new List<CCDA_ToFileWriteModel>();
     //       foreach (string orderedHeader in StandardHeaderOrder)
     //       {
     //           if (!sectionDict.TryGetValue(orderedHeader, out var values)) continue;
     //           var nonEmpty = values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();
     //           foreach (var val in nonEmpty)
     //               result.Add(new CCDA_ToFileWriteModel { Section = orderedHeader, Value = val });
     //       }

     //       return result;
     //   }

        // ── Check if a table row's date cell matches DOS ──────────────────────────────
        private static bool RowDateMatches(string cellText, DateTime targetDate)
        {
            // Handles formats: "03/15/2024", "2024-03-15", "Mar 15, 2024"
            string[] formats = { "MM/dd/yyyy", "yyyy-MM-dd", "MMM dd, yyyy",
                         "MM/dd/yy",   "dd/MM/yyyy", "MMMM dd, yyyy" };

            foreach (var fmt in formats)
            {
                if (DateTime.TryParseExact(cellText.Trim(), fmt,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                {
                    return parsed.Date == targetDate.Date;
                }
            }

            // If we can't parse the date, include the row (don't filter it out)
            return true;
        }

        // ── Resolves raw XML title/LOINC → standard display header ───────────────────
        private static string ResolveHeader(string rawTitle, string loincCode)
        {
            if (!string.IsNullOrEmpty(rawTitle) &&
                HeaderMapping.TryGetValue(rawTitle, out string mapped))
                return mapped;

            if (!string.IsNullOrEmpty(rawTitle) &&
                StandardHeaderOrder.Contains(rawTitle, StringComparer.OrdinalIgnoreCase))
                return rawTitle;

            if (!string.IsNullOrEmpty(loincCode) &&
                LoincToHeaderMapping.TryGetValue(loincCode, out string loincMapped))
                return loincMapped;

            return null;
        }

        private static bool IsDateSensitiveSection(string header)
        {
            var dateSensitive = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "MDM or Assessment and Plan",
        "Treatment Plan",
        "Orders",
        "Referrals",
        "HPI",
        "ROS",
        "Physical Exam",
        "Chief Complaint",
        "Reason for Visit",
        "Electronically Signed By",
        "Format 1 - History and Physical / Consult Notes / Progress Notes / Visit Notes",
        "Format 2 - SOAP Notes"
    };
            return dateSensitive.Contains(header);
            // NOT date-sensitive (always show): PMH, PMSFH, Allergy
        }

        // ── Helper: check if an <entry> node has effectiveTime matching the DOS ───────
        private static bool EntryMatchesDate(XmlNode entry, XmlNamespaceManager ns, DateTime targetDate)
        {
            // Try all effectiveTime nodes within this entry
            XmlNodeList effectiveTimes = entry.SelectNodes(".//cda:effectiveTime", ns);
            if (effectiveTimes == null || effectiveTimes.Count == 0)
                return true; // No date on this entry — include it (conservative)

            foreach (XmlNode et in effectiveTimes)
            {
                // Check @value attribute  e.g. value="20240315"
                string val = et.Attributes?["value"]?.Value;
                if (TryParseEcwDate(val, out DateTime parsed) && parsed.Date == targetDate.Date)
                    return true;

                // Check <low value="..."/>
                string low = et.SelectSingleNode("cda:low", ns)?.Attributes?["value"]?.Value;
                if (TryParseEcwDate(low, out DateTime parsedLow) && parsedLow.Date == targetDate.Date)
                    return true;
            }
            return false;
        }

        // ── Helper: check section-level effectiveTime ─────────────────────────────────
        private static bool SectionEffectiveTimeMatchesDate(
            XmlNode section, XmlNamespaceManager ns, DateTime targetDate)
        {
            string val = section.SelectSingleNode("cda:effectiveTime", ns)
                                ?.Attributes?["value"]?.Value;
            if (TryParseEcwDate(val, out DateTime parsed))
                return parsed.Date == targetDate.Date;
            return true; // No date — include by default
        }

        // ── Helper: parse ECW date formats (yyyyMMdd or yyyyMMddHHmmss+offset) ────────
        private static bool TryParseEcwDate(string raw, out DateTime result)
        {
            result = default;
            if (string.IsNullOrEmpty(raw)) return false;

            // Trim to first 8 chars (yyyyMMdd) — handles yyyyMMddHHmmss+0000
            string datePart = raw.Length >= 8 ? raw.Substring(0, 8) : raw;

            return DateTime.TryParseExact(datePart, "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        // ── Helper: extract text lines from a section's <text> block ─────────────────
        private static List<string> ExtractSectionText(XmlNode section, XmlNamespaceManager ns)
        {
            var lines = new List<string>();
            XmlNodeList tableRows = section.SelectNodes(".//cda:text//cda:tr", ns);
            if (tableRows != null && tableRows.Count > 0)
            {
                foreach (XmlNode row in tableRows)
                {
                    var cells = row.SelectNodes(".//cda:td", ns);
                    if (cells == null) continue;
                    var parts = new List<string>();
                    foreach (XmlNode cell in cells)
                    {
                        string t = cell.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(t)) parts.Add(t);
                    }
                    if (parts.Count > 0) lines.Add(string.Join(" | ", parts));
                }
            }
            else
            {
                foreach (XmlNode textNode in section.SelectNodes("cda:text//text()", ns) ??
                                                     new XmlDocument().ChildNodes)
                {
                    string t = textNode.InnerText.Trim();
                    if (!string.IsNullOrWhiteSpace(t)) lines.Add(t);
                }
            }
            return lines;
        }

        
        private static List<string> ExtractTextFromNodeOld(XmlNode entry, XmlNamespaceManager ns)
        {
            var lines = new List<string>();
            // Get display names and values from coded elements
            foreach (XmlNode node in entry.SelectNodes(".//*[@displayName]", ns) ??
                                           new XmlDocument().ChildNodes)
            {
                string display = node.Attributes?["displayName"]?.Value?.Trim();
                if (!string.IsNullOrWhiteSpace(display)) lines.Add(display);
            }
            // Also grab any text() nodes
            foreach (XmlNode node in entry.SelectNodes(".//text()", ns) ??
                                           new XmlDocument().ChildNodes)
            {
                string t = node.InnerText.Trim();
                if (!string.IsNullOrWhiteSpace(t) && !lines.Contains(t)) lines.Add(t);
            }
            return lines;
        }

        private static List<string> ExtractTextFromNode(XmlNode node, XmlNamespaceManager ns)
        {
            var results = new List<string>();

            if (node == null) return results;

            foreach (XmlNode child in node.ChildNodes)
            {
                
                if (child.Name == "author" ||
                    child.Name == "assignedAuthor" ||
                    child.Name == "representedOrganization" ||
                    child.Name == "assignedPerson" ||
                    child.Name == "name")
                {
                    continue;
                }
               
                if (child.Name == "text" ||
                    child.Name == "value" ||
                    child.Name == "content" ||
                    child.Name == "table")
                {
                    results.Add(child.InnerText.Trim());
                }
                else
                {
                    results.AddRange(ExtractTextFromNode(child, ns));
                }
            }

            return results;
        }
        public static List<CCDA_ToFileWriteModel> ParseCCDASectionsFromString(string ccdaXml, string dos = null)  // <-- dos format: "yyyy-MM-dd"
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ccdaXml);
            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");

            // Parse the target date once
            DateTime? targetDate = null;
            if (!string.IsNullOrEmpty(dos) && DateTime.TryParseExact(dos, "yyyy-MM-dd",  CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime td))
            {
                targetDate = td;
            }

            var sectionDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", ns);
            if (sectionNodes == null) return new List<CCDA_ToFileWriteModel>();

            foreach (XmlNode section in sectionNodes)
            {
                /// ── Resolve header (same logic as before) ────────────────────────
                //string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();

                string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim() ?? "";

                string normalized = NormalizeHeader(rawTitle);

                //string loincCode = section.SelectSingleNode("cda:code", ns) ?.Attributes?["code"]?.Value?.Trim();
                string loincCode = section.SelectSingleNode(".//cda:code", ns) ?.Attributes?["code"]?.Value?.Trim();

                string header = null;

                //if (!string.IsNullOrEmpty(rawTitle))
                //{
                //    if (HeaderMapping.TryGetValue(rawTitle, out string mapped))
                //        header = mapped;
                //    else if (StandardHeaderOrder.Contains(rawTitle, StringComparer.OrdinalIgnoreCase))
                //        header = rawTitle;
                //}

                //if (!string.IsNullOrEmpty(normalized))
                //{
                //    if (NormalizedHeaderMapping.TryGetValue(normalized, out string mapped))
                //        header = mapped;
                //    else if (StandardHeaderOrder.Contains(rawTitle, StringComparer.OrdinalIgnoreCase))
                //        header = rawTitle?.Trim();
                //}

                // 1. LOINC FIRST (MOST RELIABLE)
                if (!string.IsNullOrEmpty(loincCode) &&
                    LoincToHeaderMapping.TryGetValue(loincCode, out string loincMapped))
                {
                    header = loincMapped;
                }
                // 2. THEN TITLE MAPPING
                else if (!string.IsNullOrEmpty(normalized) &&
                         NormalizedHeaderMapping.TryGetValue(normalized, out string mapped))
                {
                    header = mapped;
                }
                // 3. FALLBACK
                else
                {
                    header = rawTitle;
                }


                if (header == null && !string.IsNullOrEmpty(loincCode))
                    LoincToHeaderMapping.TryGetValue(loincCode, out header);
                if (header == null)
                    header = rawTitle ?? loincCode;
                if (string.IsNullOrEmpty(header))
                    continue;

                // ── DATE FILTERING ────────────────────────────────────────────────
                // For date-sensitive sections, only include entries matching DOS
                bool isDateSensitiveSection = IsDateSensitiveSection(header);

                var lines = new List<string>();

                if (isDateSensitiveSection && targetDate.HasValue)
                {

                    //XmlNodeList entries = section.SelectNodes("cda:entry", ns);
                    //if (entries != null && entries.Count > 0)
                    //{
                    //    foreach (XmlNode entry in entries)
                    //    {
                    //        // Check effectiveTime at any depth inside the entry
                    //        bool matchesDate = EntryMatchesDate(entry, ns, targetDate.Value);
                    //        if (!matchesDate) continue;

                    //        // Extract text from this entry only
                    //        var entryText = ExtractTextFromNode(entry, ns);
                    //        lines.AddRange(entryText);
                    //    }

                    //    // If entry-level filtering found nothing, fall back to full section text
                    //    // (some sections don't have per-entry effectiveTime)
                    //    if (lines.Count == 0)
                    //        lines.AddRange(ExtractSectionText(section, ns));
                    //}
                    //else
                    //{
                    //    // No <entry> nodes — check section-level effectiveTime
                    //    bool sectionMatchesDate = SectionEffectiveTimeMatchesDate(section, ns, targetDate.Value);
                    //    if (sectionMatchesDate || !targetDate.HasValue)
                    //        lines.AddRange(ExtractSectionText(section, ns));
                    //}

                    XmlNode textNode = section.SelectSingleNode("cda:text", ns);

                    if (textNode != null)
                    {
                        lines.AddRange(ExtractTextFromNode(textNode, ns));
                    }
                    else
                    {
                        // fallback ONLY if no text exists
                        XmlNodeList entries = section.SelectNodes("cda:entry", ns);

                        if (entries != null)
                        {
                            foreach (XmlNode entry in entries)
                            {
                                bool matchesDate = !targetDate.HasValue || EntryMatchesDate(entry, ns, targetDate.Value);
                                if (!matchesDate) continue;

                                lines.AddRange(ExtractTextFromNode(entry, ns));
                            }
                        }
                    }
                }
                else
                {
                    // Non-date-sensitive sections (Demographics, Allergies, PMH etc.) — always include
                    lines.AddRange(ExtractSectionText(section, ns));
                }

                if (lines.Count == 0) continue;

                if (!sectionDict.ContainsKey(header))
                    sectionDict[header] = new List<string>();
                sectionDict[header].AddRange(lines);
            }

            // ── Emit in StandardHeaderOrder, skip missing ─────────────────────────
            var result = new List<CCDA_ToFileWriteModel>();
            foreach (string orderedHeader in StandardHeaderOrder)
            {
                if (!sectionDict.TryGetValue(orderedHeader, out var values)) continue;
                foreach (var val in values)
                    if (!string.IsNullOrWhiteSpace(val))
                        result.Add(new CCDA_ToFileWriteModel { Section = orderedHeader, Value = val });
            }
            // Append anything not in standard order
            foreach (var kvp in sectionDict)
            {
                if (StandardHeaderOrder.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase)) continue;
                foreach (var val in kvp.Value)
                    if (!string.IsNullOrWhiteSpace(val))
                        result.Add(new CCDA_ToFileWriteModel { Section = kvp.Key, Value = val });
            }

            return result;
        }

        private static string NormalizeHeader(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input
                .Trim()
                .Replace(":", "")
                .Replace("-", "")
                .Replace("/", " ")
                .Replace("_", " ")
                .Replace("  ", " ")
                .ToLowerInvariant();
        }

        public static readonly Dictionary<string, string> NormalizedHeaderMapping =
         new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    { NormalizeHeader("chief complaint"), "Chief Complaint" },
    { NormalizeHeader("chief complaints"), "Chief Complaint" },
    { NormalizeHeader("reason for visit"), "Chief Complaint" },
    { NormalizeHeader("presenting problem"), "Chief Complaint" },

    { NormalizeHeader("hpi"), "HPI" },
    //{ NormalizeHeader("History and Physical Notes"), "HPI" },
    { NormalizeHeader("history of present illness"), "HPI" },
    { NormalizeHeader("hx of present illness"), "HPI" },

    { NormalizeHeader("ros"), "ROS" },
    { NormalizeHeader("review of systems"), "ROS" },

    { NormalizeHeader("physical exam"), "Physical Exam" },
    { NormalizeHeader("physical examination"), "Physical Exam" },
    { NormalizeHeader("examination"), "Physical Exam" },

    { NormalizeHeader("pmh"), "PMH" },
    { NormalizeHeader("past medical history"), "PMH" },

    { NormalizeHeader("pmsfh"), "PMSFH" },
    { NormalizeHeader("social history"), "PMSFH" },
    { NormalizeHeader("family history"), "PMSFH" },

    { NormalizeHeader("allergy"), "Allergy" },
    { NormalizeHeader("allergies"), "Allergy" },

    { NormalizeHeader("assessment"), "MDM or Assessment and Plan" },
    { NormalizeHeader("assessment and plan"), "MDM or Assessment and Plan" },
    { NormalizeHeader("plan of care"), "Treatment Plan" },

    //{ NormalizeHeader("plan of treatment"), "Plan Of Treatment" },
    { NormalizeHeader("treatment plan"), "Treatment Plan" },

    { NormalizeHeader("signature"), "Electronically Signed By" }
};

        /// ///////////////////////////////////////////////////////////////////////


        //public static List<CCDA_ToFileWriteModel> ParseCCDASectionsFromString(string ccdaXml)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(ccdaXml);

        //    XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
        //    ns.AddNamespace("cda", "urn:hl7-org:v3");

        //    // Use a dictionary to MERGE entries under the same header
        //    // (handles multiple XML sections mapping to same display header)
        //    var sectionDict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        //    XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", ns);
        //    if (sectionNodes == null) return new List<CCDA_ToFileWriteModel>();

        //    foreach (XmlNode section in sectionNodes)
        //    {
        //        // ── Step 1: try <title> text ──────────────────────────────────────
        //        string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();

        //        // ── Step 2: fallback to <code> LOINC if title missing/unmapped ────
        //        string loincCode = section.SelectSingleNode("cda:code", ns)
        //                                  ?.Attributes?["code"]?.Value?.Trim();

        //        // Resolve to display header
        //        string header = null;

        //        if (!string.IsNullOrEmpty(rawTitle))
        //        {
        //            if (HeaderMapping.TryGetValue(rawTitle, out string mapped))
        //                header = mapped;
        //            else if (StandardHeaderOrder.Contains(rawTitle, StringComparer.OrdinalIgnoreCase))
        //                header = rawTitle;   // already a canonical name
        //        }

        //        if (header == null && !string.IsNullOrEmpty(loincCode))
        //        {
        //            LoincToHeaderMapping.TryGetValue(loincCode, out header);
        //        }

        //        // Still nothing? Use raw title as-is so we don't lose data
        //        if (header == null)
        //            header = rawTitle ?? loincCode;

        //        if (string.IsNullOrEmpty(header))
        //            continue;

        //        // ── Step 3: collect text content ──────────────────────────────────
        //        var lines = new List<string>();

        //        // Try structured <entry> elements first (medications, allergies, etc.)
        //        // Then fall back to free-text <text> block
        //        XmlNodeList tableRows = section.SelectNodes(".//cda:text//cda:tr", ns);
        //        if (tableRows != null && tableRows.Count > 0)
        //        {
        //            foreach (XmlNode row in tableRows)
        //            {
        //                var cells = row.SelectNodes(".//cda:td", ns);
        //                if (cells == null) continue;
        //                var parts = new List<string>();
        //                foreach (XmlNode cell in cells)
        //                {
        //                    string cellText = cell.InnerText.Trim();
        //                    if (!string.IsNullOrWhiteSpace(cellText))
        //                        parts.Add(cellText);
        //                }
        //                if (parts.Count > 0)
        //                    lines.Add(string.Join(" | ", parts));
        //            }
        //        }
        //        else
        //        {
        //            // Plain text nodes
        //            XmlNodeList textNodes = section.SelectNodes("cda:text//text()", ns);
        //            if (textNodes != null)
        //            {
        //                foreach (XmlNode textNode in textNodes)
        //                {
        //                    string content = textNode.InnerText.Trim();
        //                    if (!string.IsNullOrWhiteSpace(content))
        //                        lines.Add(content);
        //                }
        //            }
        //        }

        //        if (lines.Count == 0) continue;

        //        // Merge into dictionary
        //        if (!sectionDict.ContainsKey(header))
        //            sectionDict[header] = new List<string>();

        //        sectionDict[header].AddRange(lines);
        //    }

        //    // ── Step 4: emit in StandardHeaderOrder, skip missing sections ────────
        //    var result = new List<CCDA_ToFileWriteModel>();

        //    foreach (string orderedHeader in StandardHeaderOrder)
        //    {
        //        if (!sectionDict.TryGetValue(orderedHeader, out var values)) continue;
        //        foreach (var val in values)
        //        {
        //            if (!string.IsNullOrWhiteSpace(val))
        //                result.Add(new CCDA_ToFileWriteModel
        //                {
        //                    Section = orderedHeader,
        //                    Value = val
        //                });
        //        }
        //    }

        //    // Append any sections NOT in StandardHeaderOrder (don't silently drop them)
        //    foreach (var kvp in sectionDict)
        //    {
        //        if (StandardHeaderOrder.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase))
        //            continue;
        //        foreach (var val in kvp.Value)
        //        {
        //            if (!string.IsNullOrWhiteSpace(val))
        //                result.Add(new CCDA_ToFileWriteModel
        //                {
        //                    Section = kvp.Key,
        //                    Value = val
        //                });
        //        }
        //    }

        //    return result;
        //}

        public static string GenerateClinicalPdfNew(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections, string fileNameWithoutExtension)
        {
            // Required for QuestPDF community license
            QuestPDF.Settings.License = LicenseType.Community;

            string outputPath = Path.Combine(Path.GetTempPath(), $"{fileNameWithoutExtension}.pdf");

            // Group & order sections — skip empty ones
            var grouped = sections
                .Where(s => !string.IsNullOrWhiteSpace(s.Value))
                .GroupBy(s => s.Section)
                .OrderBy(g =>
                {
                    int idx = StandardHeaderOrder.IndexOf(g.Key);
                    return idx == -1 ? int.MaxValue : idx;
                })
                .ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30, Unit.Point);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    // ── HEADER ────────────────────────────────────────────────────
                    page.Header().Column(col =>
                    {
                        // Title bar
                        col.Item()
                           .Background("#003366")
                           .Padding(10)
                           .Text("Clinical Visit Summary")
                           .FontSize(16)
                           .Bold()
                           .FontColor(Colors.White);

                        col.Item().Height(6);

                        // Patient info grid (2-column table)
                        col.Item()
                           .Border(1)
                           .BorderColor("#CCCCCC")
                           .Table(table =>
                           {
                               table.ColumnsDefinition(cols =>
                               {
                                   cols.RelativeColumn();
                                   cols.RelativeColumn();
                                   cols.RelativeColumn();
                                   cols.RelativeColumn();
                               });

                               void AddCell(string label, string value)
                               {
                                   table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#EEEEEE")
                                        .Padding(5)
                                        .Column(c =>
                                        {
                                            c.Item().Text(label)
                                                    .FontSize(7)
                                                    .Bold()
                                                    .FontColor("#555555");
                                            c.Item().Text(value ?? "—")
                                                    .FontSize(9)
                                                    .FontColor("#000000");
                                        });
                               }

                               AddCell("Patient", patient.Name);
                               AddCell("DOB", patient.DOB);
                               AddCell("Age / Gender", $"{patient.Age} / {patient.Gender}");
                               AddCell("Date of Service", patient.DateOfService);
                               AddCell("Phone", patient.Phone);
                               AddCell("Address", patient.Address);
                               AddCell("Provider", patient.Provider);
                               AddCell("", "");  // filler cell
                           });

                        col.Item().Height(8);
                    });

                    // ── CONTENT ───────────────────────────────────────────────────
                    page.Content().Column(col =>
                    {
                        foreach (var group in grouped)
                        {
                            // Section heading
                            col.Item()
                               .PaddingTop(10)
                               .BorderBottom(1.5f)
                               .BorderColor("#004080")
                               .PaddingBottom(2)
                               .Text(group.Key)
                               .FontSize(11)
                               .Bold()
                               .FontColor("#004080");

                            // Section lines
                            foreach (var item in group)
                            {
                                if (string.IsNullOrWhiteSpace(item.Value)) continue;

                                col.Item()
                                   .PaddingLeft(8)
                                   .PaddingTop(2)
                                   .Text(item.Value)
                                   .FontSize(9)
                                   .FontColor("#111111");
                            }
                        }
                    });

                    // ── FOOTER ────────────────────────────────────────────────────
                    page.Footer()
                        .BorderTop(1)
                        .BorderColor("#CCCCCC")
                        .PaddingTop(4)
                        .Row(row =>
                        {
                            row.RelativeItem()
                               .Text($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}")
                               .FontSize(7)
                               .FontColor("#888888");

                            row.RelativeItem()
                               .AlignRight()
                               .Text(x =>
                               {
                                   x.Span("Page ").FontSize(7).FontColor("#888888");
                                   x.CurrentPageNumber().FontSize(7).FontColor("#888888");
                                   x.Span(" of ").FontSize(7).FontColor("#888888");
                                   x.TotalPages().FontSize(7).FontColor("#888888");
                               });
                        });
                });
            })
            .GeneratePdf(outputPath);

            return outputPath;
        }

        public static PatientInfoforPDF ExtractPatientFromCCDA(string ccdaXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ccdaXml);

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");

            var patient = new PatientInfoforPDF();

            // Name
            XmlNode nameNode = doc.SelectSingleNode("//cda:patient/cda:name", ns);
            if (nameNode != null)
            {
                string given = nameNode.SelectSingleNode("cda:given", ns)?.InnerText ?? "";
                string family = nameNode.SelectSingleNode("cda:family", ns)?.InnerText ?? "";
                patient.Name = $"{given} {family}".Trim();
            }

            // DOB + Age
            string dobValue = doc.SelectSingleNode("//cda:birthTime", ns)?.Attributes?["value"]?.Value;
            if (DateTime.TryParseExact(dobValue, "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime dob))
            {
                patient.DOB = dob.ToString("dd/MM/yyyy");
                patient.Age = ((int)((DateTime.Now - dob).TotalDays / 365.25)).ToString();
            }

            // Gender
            patient.Gender = doc.SelectSingleNode("//cda:administrativeGenderCode", ns)?
                .Attributes?["displayName"]?.Value;

            // Phone
            var phoneNode = doc.SelectSingleNode("//cda:telecom[contains(@value,'tel:')]", ns);
            patient.Phone = phoneNode?.Attributes?["value"]?.Value?.Replace("tel:", "");

            // Address
            XmlNode addr = doc.SelectSingleNode("//cda:addr", ns);
            if (addr != null)
            {
                string street = addr.SelectSingleNode("cda:streetAddressLine", ns)?.InnerText;
                string city = addr.SelectSingleNode("cda:city", ns)?.InnerText;
                string state = addr.SelectSingleNode("cda:state", ns)?.InnerText;
                string zip = addr.SelectSingleNode("cda:postalCode", ns)?.InnerText;

                patient.Address = $"{street}, {city}, {state} {zip}";
            }

            // Provider
            XmlNode provider = doc.SelectSingleNode("//cda:assignedAuthor/cda:assignedPerson/cda:name", ns);
            if (provider != null)
            {
                string given = provider.SelectSingleNode("cda:given", ns)?.InnerText ?? "";
                string family = provider.SelectSingleNode("cda:family", ns)?.InnerText ?? "";
                patient.Provider = $"{given} {family}".Trim();
            }

            return patient;
        }



        public static List<CCDA_ToFileWriteModel> ParseCCDASectionsFromStringOld(string ccdaXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ccdaXml); // <-- Load from string

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");

            List<CCDA_ToFileWriteModel> sections = new List<CCDA_ToFileWriteModel>();

            XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", ns);

            if (sectionNodes != null)
            {
                foreach (XmlNode section in sectionNodes)
                {
                    //string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();

                    string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim() ?? section.SelectSingleNode(".//cda:text", ns)?.InnerText?.Trim();


                    if (string.IsNullOrEmpty(rawTitle))
                        continue;

                    string header = HeaderMapping.ContainsKey(rawTitle) ? HeaderMapping[rawTitle] : rawTitle;

                    XmlNodeList textNodes = section.SelectNodes("cda:text//text()", ns);
                    if (textNodes != null)
                    {
                        foreach (XmlNode textNode in textNodes)
                        {
                            string content = textNode.InnerText.Trim();
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                sections.Add(new CCDA_ToFileWriteModel
                                {
                                    Section = header,
                                    Value = content
                                });
                            }
                        }
                    }
                }
            }

            return sections;
        }


        public static List<CCDA_ToFileWriteModel> ParseAssessmentFromXml(string ccdaXml, string dos)
        {
            var result = new List<CCDA_ToFileWriteModel>();
            if (string.IsNullOrEmpty(ccdaXml)) return result;

            if (!DateTime.TryParse(dos, out var dosDate)) return result;
            string dosFormatted = dosDate.ToString("MM/dd/yyyy"); // "06/02/2026"

            var doc = new XmlDocument();
            doc.LoadXml(ccdaXml);
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("cda", "urn:hl7-org:v3");

            // Find the Assessments section by LOINC code 51848-0
            XmlNode assessSection = doc.SelectSingleNode(
                "//cda:section[cda:code[@code='51848-0']]", ns);

            if (assessSection == null) return result;

            // Find all <tr> rows in the table (skip header row)
            XmlNodeList rows = assessSection.SelectNodes(".//cda:text//tr[position()>1]", ns)
                ?? assessSection.SelectNodes(".//tr[position()>1]");

            // Fallback without namespace if needed
            if (rows == null || rows.Count == 0)
                rows = assessSection.SelectNodes(".//tr");

            if (rows == null) return result;

            bool firstRow = true;
            foreach (XmlNode row in rows)
            {
                // Skip the header <tr> which contains <th> elements
                if (firstRow && row.SelectNodes(".//th")?.Count > 0)
                {
                    firstRow = false;
                    continue;
                }
                firstRow = false;

                XmlNodeList cells = row.SelectNodes(".//td");
                if (cells == null || cells.Count < 2) continue;

                string rowDate = cells[0].InnerText?.Trim();
                string diagnosis = cells[1].InnerText?.Trim();
                string assessNote = cells.Count > 2 ? cells[2].InnerText?.Trim() : "";
                string treatNote = cells.Count > 3 ? cells[3].InnerText?.Trim() : "";
                string sectionNote = cells.Count > 4 ? cells[4].InnerText?.Trim() : "";

                // Filter by DOS
                if (rowDate != dosFormatted) continue;
                if (string.IsNullOrWhiteSpace(diagnosis)) continue;

                // Clean ICD format: "(ICD-10 - M79.606)" → "- M79.606"
                string cleanDiagnosis = System.Text.RegularExpressions.Regex.Replace(
                    diagnosis, @"\(ICD-10\s*-\s*([A-Z0-9.]+)\)", "- $1").Trim();

                result.Add(new CCDA_ToFileWriteModel
                {
                    Section = "MDM or Assessment and Plan",
                    Value = cleanDiagnosis
                });

                // Add assessment note if present
                if (!string.IsNullOrWhiteSpace(assessNote))
                    result.Add(new CCDA_ToFileWriteModel
                    {
                        Section = "MDM or Assessment and Plan",
                        Value = $"Notes: {assessNote}"
                    });

                // Add treatment note if present  
                if (!string.IsNullOrWhiteSpace(treatNote))
                    result.Add(new CCDA_ToFileWriteModel
                    {
                        Section = "MDM or Assessment and Plan",
                        Value = $"Notes: {treatNote}"
                    });

                // Add section note if present
                if (!string.IsNullOrWhiteSpace(sectionNote))
                    result.Add(new CCDA_ToFileWriteModel
                    {
                        Section = "MDM or Assessment and Plan",
                        Value = $"Notes: {sectionNote}"
                    });
            }

            return result;
        }


    }
}
