using emedl_chase.Model;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using static emedl_chase.Model.PatientDTO;

namespace emedl_chase.Helper
{
    public class XmlConvertor
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public XmlConvertor(IWebHostEnvironment webHostEnvironment) {

             _webHostEnvironment = webHostEnvironment;
    }
        public static string xmlconvertor(string base64data, string patient_name, string dos)
        {
            try
            {
                // 1. Decode Base64 to byte array
                byte[] xmlBytes = Convert.FromBase64String(base64data);

                // 2. Convert bytes to UTF-8 string
                string xmlContent = Encoding.UTF8.GetString(xmlBytes);

                // 3. Save to file with UTF-8 encoding
                //var prepare_file_path = $"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}.xml";
                var prepare_file_path = $"D:\\Emed Projects\\EMED Data Interoperability\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}.xml";
          
                File.WriteAllText(prepare_file_path, xmlContent, Encoding.UTF8);

                Console.WriteLine("✅ XML file saved successfully at: " + Path.GetFullPath(prepare_file_path));

                return prepare_file_path;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("❌ Invalid Base64 format: " + ex.Message);
                return "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return "ok";
            }
           
        }

        public static string XmlConvertorUpdated(string base64data, string patient_name, string dos,string type)
        {
            try
            {

                string xmlContent;

                if (base64data.TrimStart().StartsWith("<"))
                {
                    xmlContent = base64data; // already XML
                }
                else
                {
                    byte[] xmlBytes = Convert.FromBase64String(base64data);
                    xmlContent = Encoding.UTF8.GetString(xmlBytes);
                }

                //// 1. Decode Base64 to byte array
                //byte[] xmlBytes = Convert.FromBase64String(base64data);

                //// 2. Convert bytes to UTF-8 string
                //string xmlContent = Encoding.UTF8.GetString(xmlBytes);

                // 3. Clean BOM / whitespace
                xmlContent = xmlContent.Trim('\uFEFF', '\u200B').Trim();

                // 4. Quick check: does it look like XML?
                if (!xmlContent.StartsWith("<"))
                {
                    Console.WriteLine("❌ Decoded content is not starting with '<'. Not valid XML.");
                    Console.WriteLine("Preview: " + xmlContent.Substring(0, Math.Min(200, xmlContent.Length)));
                    return "invalid_xml";
                }

                // 5. Validate well-formedness
                try
                {
                    XDocument.Parse(xmlContent); // throws if invalid
                    Console.WriteLine("✅ XML is well-formed.");
                }
                catch (XmlException ex)
                {
                    Console.WriteLine("❌ XML parse error: " + ex.Message);
                    return "invalid_xml";
                }

                // 6. Save to file


                string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

                // Create folder if it doesn’t exist
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Full file path
                var file = $"{patient_name}_{dos}_{type}.xml";
                string filePath = Path.Combine(uploadPath,file);
                //var prepare_file_path = $"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}_{type}.xml";
                //var prepare_file_path = $"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}_{type}.xml";
                var prepare_file_path = $"D:\\Emed Projects\\EMED Data Interoperability\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}.xml";

                File.WriteAllText(filePath, xmlContent, Encoding.UTF8);

                Console.WriteLine("✅ XML file saved successfully at: " + Path.GetFullPath(filePath));
                return filePath;
            }
            catch (FormatException ex)
            {
                Console.WriteLine("❌ Invalid Base64 format: " + ex.Message);
                return "invalid_base64";
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message);
                return "error";
            }
        }

        //        using SelectPdf;
        //using System;
        //using System.Xml;
        //using System.Text;

        //class Program
        //    {
        //        static void Main()
        //        {
        //            // Load XML from file or string
        //            XmlDocument xmlDoc = new XmlDocument();
        //            xmlDoc.Load("data.xml");

        //            // Create simple HTML string from XML (you can style it better)
        //            StringBuilder htmlBuilder = new StringBuilder();
        //            htmlBuilder.Append("<html><body><h2>XML Content</h2><ul>");

        //            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        //            {
        //                htmlBuilder.Append("<li><b>" + node.Name + ":</b> " + node.InnerText + "</li>");
        //            }

        //            htmlBuilder.Append("</ul></body></html>");

        //            // Convert HTML to PDF
        //            HtmlToPdf converter = new HtmlToPdf();
        //            PdfDocument doc = converter.ConvertHtmlString(htmlBuilder.ToString());

        //            // Save PDF
        //            doc.Save("output.pdf");
        //            doc.Close();

        //            Console.WriteLine("✅ PDF created: output.pdf");
        //        }
        //    }



        public static string CCDAFilread(string ccdafilepath)
        {
            string fileName = Path.GetFileNameWithoutExtension(ccdafilepath);
            // Load the XML document from file
            XmlDocument doc = new XmlDocument();
            doc.Load(ccdafilepath);
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("cda", "urn:hl7-org:v3");
            XmlNode firstRow = doc.SelectSingleNode("//cda:table/cda:row[1]", nsManager);

            XmlNode root = doc.DocumentElement;
            using (StreamWriter writer = new StreamWriter("output_08_13_2025.txt"))
                if (root != null)
            {
                TraverseNodes(root, writer);
            }

            static void TraverseNodes(XmlNode node, StreamWriter writer)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        if (child.HasChildNodes && child.FirstChild.NodeType == XmlNodeType.Text)
                        {
                            // Simple tag with a value
                            if (child.Name != "th")
                            {                                
                                    writer.WriteLine(child.InnerText + "\n");
                                    Console.WriteLine($"{child.Name}: {child.InnerText}");
                            }
                        }
                        else
                        {
                            // Nested tag, recurse into it
                            TraverseNodes(child, writer);
                        }
                    }
                }
            }

            return "Success";
        }

        public static Tuple<List<string>, string, List<CCDAHL7Model>> ReadCCDAFile(string uplodedXMLFilePath = "")
        {
            string txtFilePath = "";
            List<string> sections_CCDAHL7 = new List<string>();
            List<string> entry_data = new List<string>();
            var model_CCDAList = new List<CCDAHL7Model>();
            try
            {

                string fileName = Path.GetFileNameWithoutExtension(uplodedXMLFilePath);
               
                XmlDocument doc = new XmlDocument();
                doc.Load(uplodedXMLFilePath);
                
                // Namespace manager to handle namespaces in XML
                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("cda", "urn:hl7-org:v3");
                XmlNode firstRow = doc.SelectSingleNode("//cda:table/cda:row[1]", nsManager);

                XmlNodeList entry_notes = doc.SelectNodes("//cda:component", nsManager);

                if (entry_notes != null && entry_notes.Count > 0)
                {
                    foreach (XmlNode node in entry_notes)
                    {
                        string sectionTitle = node.SelectSingleNode("//cda:structuredBody/cda:component/cda:section/cda:title", nsManager)?.InnerText.Trim();
                        entry_data.Add(sectionTitle);
                    }
                }


                var patient = new PatientInfoforPDF();

                //patient.Name = doc.SelectSingleNode( "//cda:recordTarget/cda:patientRole/cda:patient/cda:name", nsManager)?.InnerText?.Trim();

                XmlNode nameNode = doc.SelectSingleNode("//cda:recordTarget/cda:patientRole/cda:patient/cda:name", nsManager);

                if (nameNode != null)
                {
                    string givenName = nameNode.SelectSingleNode("cda:given", nsManager)?.InnerText?.Trim() ?? "";
                    string familyName = nameNode.SelectSingleNode("cda:family", nsManager)?.InnerText?.Trim() ?? "";

                    patient.Name = $"{givenName} {familyName}";
                }

                //patient.DOB =doc.SelectSingleNode( "//cda:birthTime", nsManager)?.Attributes?["value"]?.Value;

                //dob
                string dobValue = doc.SelectSingleNode( "//cda:birthTime", nsManager)?.Attributes?["value"]?.Value;
                if (!string.IsNullOrEmpty(dobValue))
                {
                    if (DateTime.TryParseExact( dobValue, "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime dateob))
                    {
                        patient.DOB = dateob.ToString("dd/MM/yyyy");

                        patient.Age =
                            ((int)((DateTime.Now - dateob).TotalDays / 365.25))
                            .ToString();
                    }
                }

                patient.Gender =  doc.SelectSingleNode( "//cda:administrativeGenderCode", nsManager)?.Attributes?["displayName"]?.Value;
                                
                ///Account number
                XmlNodeList ids = doc.SelectNodes( "//cda:patientRole/cda:id", nsManager);

                foreach (XmlNode id in ids)
                {
                    string authority = id.Attributes?["assigningAuthorityName"]?.Value;

                    if (!string.IsNullOrEmpty(authority) && authority.Contains("ACC NO"))
                    {
                        patient.AccountNumber = id.Attributes?["extension"]?.Value;
                        break;
                    }
                }

                //if (DateTime.TryParseExact(patient.DOB, "yyyyMMdd",  null, System.Globalization.DateTimeStyles.None, out DateTime dob))
                //{
                //    patient.Age = ((int)((DateTime.Now - dob).TotalDays / 365.25)).ToString();
                //}

                //Phone Number
                //string phone = doc.SelectSingleNode( "//cda:recordTarget/cda:patientRole/cda:telecom",nsManager)?.Attributes?["value"]?.Value;

                //patient.Phone = phone?.Replace("tel:", "");

                XmlNode phoneNode = doc.SelectSingleNode( "//cda:patientRole/cda:telecom[contains(@value,'tel:')]",  nsManager);

                patient.Phone = phoneNode?.Attributes?["value"]?.Value .Replace("tel:", "");

                //Address
                XmlNode addrNode = doc.SelectSingleNode( "//cda:recordTarget/cda:patientRole/cda:addr",nsManager);

                if (addrNode != null)
                {
                    string street = addrNode.SelectSingleNode("cda:streetAddressLine", nsManager)?.InnerText ?? "";
                    string city = addrNode.SelectSingleNode("cda:city", nsManager)?.InnerText ?? "";
                    string state = addrNode.SelectSingleNode("cda:state", nsManager)?.InnerText ?? "";
                    string zip = addrNode.SelectSingleNode("cda:postalCode", nsManager)?.InnerText ?? "";

                    patient.Address = $"{street}, {city}, {state} {zip}";
                }

                //Provider Name
                XmlNode providerNode = doc.SelectSingleNode("//cda:author/cda:assignedAuthor/cda:assignedPerson/cda:name",nsManager);
                if (providerNode != null)
                {
                    string given = providerNode.SelectSingleNode("cda:given", nsManager)?.InnerText ?? "";
                    string family = providerNode.SelectSingleNode("cda:family", nsManager)?.InnerText ?? "";

                    patient.Provider = $"{given} {family}";
                }


                #region 1. Example: Extract patient name
                XmlNodeList nameNodes = doc.SelectNodes("//cda:recordTarget/cda:patientRole/cda:patient/cda:name", nsManager);
                #endregion

                #region 2. Example: Extract allergies
                XmlNodeList allergyNodes = doc.SelectNodes("//cda:allergySection/cda:entry/cda:act[cda:templateId/@root='2.16.840.1.113883.10.20.22.4.7']", nsManager);
                //XmlNodeList nameNodestables= doc.SelectNodes("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                XmlNode tdNode_table = doc.SelectSingleNode("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                #endregion

                #region 3. Extracting all title nodes
                List<string> sections = new List<string>();

                XmlNodeList titleNodes = doc.SelectNodes("//cda:title", nsManager);

                #endregion


                #region 5. Extract all test based on section wise
                var sectionBase_Conditions = new List<string>();
                var section_value_list = new List<CCDA_ToFileWriteModel>();
                XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", nsManager);

                if (sectionNodes != null && sectionNodes.Count > 0)
                {
                    foreach (XmlNode sectionNode in sectionNodes)
                    {
                        var encounterDTSectionName = "";
                        // Get section title
                        string sectionTitle = sectionNode.SelectSingleNode("cda:title", nsManager)?.InnerText.Trim();
                        Console.WriteLine($"Section: {sectionTitle}");
                        //var stringdata = FileReadHelper.ReplaceWholeWord(sectionTitle, sectionTitle, $"<head>{sectionTitle}<head>", RegexOptions.IgnoreCase);

                        var alreadyExistSections = sectionBase_Conditions.Where(t => t == sectionTitle).ToList();
                        if (alreadyExistSections.Count() <= 0)
                        {
                            sectionBase_Conditions.Add(sectionTitle);
                        }
                        sections_CCDAHL7.Add(sectionTitle);

                        // Get all text content under the section
                        XmlNodeList textNodes = sectionNode.SelectNodes("cda:text//text()", nsManager);
                        //XmlNode sectionNode_tdNode_table = sectionNode.SelectSingleNode("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                        if (textNodes != null && textNodes.Count > 0)
                        {
                            int count = 0;
                            var pageNo = 0;
                            var dos_precedingterm = "";
                            foreach (XmlNode textNode in textNodes)
                            {
                                var model_CCDA = new CCDAHL7Model();
                                #region DOS Extraction
                                var text_Line = textNode.InnerText.Trim();
                                var dos = "";

                                if (text_Line.ToLower().Contains("date of encounter")
                                    || text_Line.ToLower().Contains("encounter date")
                                    || text_Line.ToLower().Contains("encounters"))
                                {
                                    dos_precedingterm = text_Line.ToLower().Contains("date of encounter") ? "date of encounter" : "encounter date";
                                    encounterDTSectionName = sectionTitle;
                                    dos = ExtractDateFromSentence(text_Line);
                                    pageNo = pageNo + 1;
                                    sectionBase_Conditions.Add(dos);

                                }
                                else
                                {
                                    if (encounterDTSectionName == sectionTitle)
                                    {
                                        dos = ExtractDateFromSentence(text_Line);
                                        if (dos != null && dos != "")
                                        {
                                            text_Line = dos_precedingterm + " : " + text_Line;
                                            pageNo = pageNo + 1;
                                            sectionBase_Conditions.Add(dos);
                                        }
                                        else
                                        {
                                            pageNo = pageNo + 1;
                                            sectionBase_Conditions.Add(dos);
                                        }

                                    }
                                }
                                //if (string.IsNullOrEmpty(dos) || string.IsNullOrWhiteSpace(dos))
                                //{
                                //    if (text_Line.ToLower().Contains("encounter date"))
                                //    {

                                //    }
                                //    if (tdNode_table != null)
                                //    {
                                //        dos = tdNode_table.InnerText;
                                //        pageNo = pageNo + 1;
                                //        sectionBase_Conditions.Add(dos);
                                //    }
                                //}

                                #endregion
                                sectionBase_Conditions.Add(sectionTitle + " : " + text_Line);
                                section_value_list.Add(new CCDA_ToFileWriteModel() { Section = sectionTitle, Value = text_Line });
                                model_CCDA.section = sectionTitle;
                                model_CCDA.Sentence = text_Line;
                                model_CCDA.DateOfEncounter = dos;
                                model_CCDA.PageNo = pageNo;

                                model_CCDAList.Add(model_CCDA);
                                //sectionBase_Conditions.Add(textNode.InnerText.Trim());

                                //Console.WriteLine(textNode.InnerText.Trim());
                                Console.WriteLine(dos);
                                count++;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No text content found in this section.");
                        }

                        Console.WriteLine(); // Add a blank line for clarity
                    }

                }
                else
                {
                    Console.WriteLine("No sections found in the CCDA file.");
                }
                #endregion

                

                #region Write XML data in an pdf file                

                string pdfPath = GenerateClinicalPdf(patient, section_value_list, fileName);

                #endregion

                #region Write XML data in an text file
                txtFilePath = WriteXMLToText(sectionBase_Conditions, fileName, null, section_value_list);
                //string pdfPath = WriteXmlToPdf(section_value_list, fileName);
                #endregion

                // int pageNo = 1;
                for (int i = 0; i < model_CCDAList.Count(); i++)
                {
                    if (string.IsNullOrEmpty(model_CCDAList[i].DateOfEncounter) || string.IsNullOrWhiteSpace(model_CCDAList[i].DateOfEncounter))
                    {
                        if (i > 0)
                        {
                            model_CCDAList[i].DateOfEncounter = model_CCDAList[i - 1].DateOfEncounter;
                            model_CCDAList[i].PageNo = model_CCDAList[i - 1].PageNo;
                        }
                        else
                        {
                            model_CCDAList[i].PageNo = 1;
                        }
                    }
                    //if (!(string.IsNullOrEmpty(model_CCDAList[i].DateOfEncounter)) || !(string.IsNullOrWhiteSpace(model_CCDAList[i].DateOfEncounter)))
                    //{
                    //    model_CCDAList[i].PageNo = pageNo;
                    //    pageNo++;
                    //}
                }
                return Tuple.Create(sections_CCDAHL7, txtFilePath, model_CCDAList);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CCDA file: {ex.Message}");
            }
            return Tuple.Create(sections_CCDAHL7, txtFilePath, model_CCDAList);
        }


        public static string GenerateClinicalPdf(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections, string fileName)
        {
            //string pdfPath = Path.Combine(  @"D:\ECWPDF",fileName + ".pdf");

            string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
            string uploadPDFPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ECW_EHR_PatientRecords", folderName);
            if (!Directory.Exists(uploadPDFPath))
            {
                Directory.CreateDirectory(uploadPDFPath);
            }

            string safeFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));

            string PDFfilepath = Path.Combine(uploadPDFPath, safeFileName + ".pdf");


            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);

                    page.Margin(20);

                    // HEADER
                    page.Header().Column(column =>
                    {
                        // Top Header Line
                        column.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));

                            text.Span(patient.Name.ToUpper());

                            text.Span($" DOB: {patient.DOB}");

                            text.Span($" ({patient.Age} yo ");

                            text.Span(
                                patient.Gender == "Female"
                                    ? "F)"
                                    : "M)");

                            text.Span($" Acc No. {patient.AccountNumber}");

                            text.Span($" DOS: {patient.DateOfService}");
                        });

                        column.Item().PaddingTop(5);

                        column.Item().LineHorizontal(1);
                    });
                    //// BODY
                    //page.Content().Column(column =>
                    //{
                    //    // Title
                    //    column.Item()
                    //        .AlignCenter()
                    //        .Text("Progress Notes")
                    //        .FontSize(20)
                    //        .Bold();

                    //    column.Item().PaddingVertical(15);

                    //    // Patient Information Block
                    //    column.Item().Row(row =>
                    //    {
                    //        row.RelativeItem().Column(left =>
                    //        {
                    //            left.Item().Text($"Patient: {patient.Name}");
                    //            left.Item().Text($"Account Number: {patient.AccountNumber}");
                    //            left.Item().Text($"DOB: {patient.DOB}");
                    //            left.Item().Text($"Age: {patient.Age}");
                    //            left.Item().Text($"Sex: {patient.Gender}");
                    //            left.Item().Text($"Phone: {patient.Phone}");
                    //            left.Item().Text($"Address: {patient.Address}");
                    //        });

                    //        row.RelativeItem().AlignRight().Column(right =>
                    //        {
                    //            right.Item().Text($"Provider: {patient.Provider}");
                    //            right.Item().Text($"Date: {patient.DateOfService}");
                    //        });
                    //    });

                    //    column.Item().PaddingTop(10);

                    //    column.Item().LineHorizontal(1);

                    //    column.Item().PaddingTop(20);
                    //});


                    page.Content().Column(column =>
                    {
                        // Title
                        column.Item()
                            .AlignCenter()
                            .Text("Patient Health Record")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingVertical(15);

                        // Patient Information Block
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Patient: {patient.Name}");
                                left.Item().Text($"Account Number: {patient.AccountNumber}");
                                left.Item().Text($"DOB: {patient.DOB}");
                                left.Item().Text($"Age: {patient.Age}");
                                left.Item().Text($"Sex: {patient.Gender}");
                                left.Item().Text($"Phone: {patient.Phone}");
                                left.Item().Text($"Address: {patient.Address}");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item().Text($"Provider: {patient.Provider}");
                                right.Item().Text($"Date: {patient.DateOfService}");
                            });
                        });

                        column.Item().PaddingTop(10);

                        column.Item().LineHorizontal(1);

                        column.Item().PaddingTop(20);


                        foreach (var group in sections.GroupBy(x => x.Section))
                        {
                            column.Item()
                                .PaddingTop(10)
                                .Text(group.Key)
                                .Bold()
                                .FontSize(12);

                            foreach (var item in group)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Value))
                                {
                                    column.Item()
                                        .PaddingLeft(10)
                                        .Text("• " + item.Value);
                                }
                            }
                        }
                    });

                    // FOOTER
                    page.Footer().Column(column =>
                    {
                        column.Item().LineHorizontal(1);

                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            })
            .GeneratePdf(PDFfilepath);

            return PDFfilepath;
        }




        public static  Tuple<List<string>, string, List<CCDAHL7Model>> ReadCCDAFileOld(string uplodedXMLFilePath = "")
        {
            string txtFilePath = "";
            List<string> sections_CCDAHL7 = new List<string>();
            List<string> entry_data = new List<string>();
            var model_CCDAList = new List<CCDAHL7Model>();
            try
            {

                string fileName = Path.GetFileNameWithoutExtension(uplodedXMLFilePath);
                // Load the XML document from file
                XmlDocument doc = new XmlDocument();
                doc.Load(uplodedXMLFilePath);

                // Namespace manager to handle namespaces in XML
                XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("cda", "urn:hl7-org:v3");
                XmlNode firstRow = doc.SelectSingleNode("//cda:table/cda:row[1]", nsManager);

                XmlNodeList entry_notes = doc.SelectNodes("//cda:component", nsManager);

                if (entry_notes != null && entry_notes.Count >0) 
                {
                
                foreach (XmlNode node in entry_notes)
                    {
                        string sectionTitle = node.SelectSingleNode("//cda:structuredBody/cda:component/cda:section/cda:title", nsManager)?.InnerText.Trim();
                        entry_data.Add(sectionTitle);

                    }

                }
                #region 1. Example: Extract patient name
                XmlNodeList nameNodes = doc.SelectNodes("//cda:recordTarget/cda:patientRole/cda:patient/cda:name", nsManager);

                #endregion

                #region 2. Example: Extract allergies
                XmlNodeList allergyNodes = doc.SelectNodes("//cda:allergySection/cda:entry/cda:act[cda:templateId/@root='2.16.840.1.113883.10.20.22.4.7']", nsManager);
                //XmlNodeList nameNodestables= doc.SelectNodes("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                XmlNode tdNode_table = doc.SelectSingleNode("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                #endregion

                #region 3. Extracting all title nodes
                List<string> sections = new List<string>();

                XmlNodeList titleNodes = doc.SelectNodes("//cda:title", nsManager);

                #endregion


                #region 5. Extract all test based on section wise
                var sectionBase_Conditions = new List<string>();
                var section_value_list = new List<CCDA_ToFileWriteModel>();
                XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", nsManager);

                if (sectionNodes != null && sectionNodes.Count > 0)
                {
                    foreach (XmlNode sectionNode in sectionNodes)
                    {
                        var encounterDTSectionName = "";
                        // Get section title
                        string sectionTitle = sectionNode.SelectSingleNode("cda:title", nsManager)?.InnerText.Trim();
                        Console.WriteLine($"Section: {sectionTitle}");
                        //var stringdata = FileReadHelper.ReplaceWholeWord(sectionTitle, sectionTitle, $"<head>{sectionTitle}<head>", RegexOptions.IgnoreCase);

                        var alreadyExistSections = sectionBase_Conditions.Where(t => t == sectionTitle).ToList();
                        if (alreadyExistSections.Count() <= 0)
                        {
                            sectionBase_Conditions.Add(sectionTitle);
                        }
                        sections_CCDAHL7.Add(sectionTitle);

                        // Get all text content under the section
                        XmlNodeList textNodes = sectionNode.SelectNodes("cda:text//text()", nsManager);
                        //XmlNode sectionNode_tdNode_table = sectionNode.SelectSingleNode("//cda:table/cda:tbody/cda:tr/cda:td[@ID='ref1']", nsManager);
                        if (textNodes != null && textNodes.Count > 0)
                        {
                            int count = 0;
                            var pageNo = 0;
                            var dos_precedingterm = "";
                            foreach (XmlNode textNode in textNodes)
                            {
                                var model_CCDA = new CCDAHL7Model();
                                #region DOS Extraction
                                var text_Line = textNode.InnerText.Trim();
                                var dos = "";

                                if (text_Line.ToLower().Contains("date of encounter")
                                    || text_Line.ToLower().Contains("encounter date")
                                    || text_Line.ToLower().Contains("encounters"))
                                {
                                    dos_precedingterm = text_Line.ToLower().Contains("date of encounter") ? "date of encounter" : "encounter date";
                                    encounterDTSectionName = sectionTitle;
                                    dos = ExtractDateFromSentence(text_Line);
                                    pageNo = pageNo + 1;
                                    sectionBase_Conditions.Add(dos);

                                }
                                else
                                {
                                    if (encounterDTSectionName == sectionTitle)
                                    {
                                        dos = ExtractDateFromSentence(text_Line);
                                        if (dos != null && dos != "")
                                        {
                                            text_Line = dos_precedingterm + " : " + text_Line;
                                            pageNo = pageNo + 1;
                                            sectionBase_Conditions.Add(dos);
                                        }
                                        else
                                        {
                                            pageNo = pageNo + 1;
                                            sectionBase_Conditions.Add(dos);
                                        }

                                    }
                                }
                                //if (string.IsNullOrEmpty(dos) || string.IsNullOrWhiteSpace(dos))
                                //{
                                //    if (text_Line.ToLower().Contains("encounter date"))
                                //    {

                                //    }
                                //    if (tdNode_table != null)
                                //    {
                                //        dos = tdNode_table.InnerText;
                                //        pageNo = pageNo + 1;
                                //        sectionBase_Conditions.Add(dos);
                                //    }
                                //}

                                #endregion
                                sectionBase_Conditions.Add(sectionTitle + " : " + text_Line);
                                section_value_list.Add(new CCDA_ToFileWriteModel() { Section = sectionTitle, Value = text_Line });
                                model_CCDA.section = sectionTitle;
                                model_CCDA.Sentence = text_Line;
                                model_CCDA.DateOfEncounter = dos;
                                model_CCDA.PageNo = pageNo;

                                model_CCDAList.Add(model_CCDA);
                                //sectionBase_Conditions.Add(textNode.InnerText.Trim());

                                //Console.WriteLine(textNode.InnerText.Trim());
                                Console.WriteLine(dos);
                                count++;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No text content found in this section.");
                        }

                        Console.WriteLine(); // Add a blank line for clarity
                    }

                }
                else
                {
                    Console.WriteLine("No sections found in the CCDA file.");
                }
                #endregion

                #region Write XML data in an text file
                txtFilePath = WriteXMLToText(sectionBase_Conditions, fileName,null, section_value_list);
                string pdfPath = WriteXmlToPdf(section_value_list, fileName);
                #endregion
                // int pageNo = 1;
                for (int i = 0; i < model_CCDAList.Count(); i++)
                {
                    if (string.IsNullOrEmpty(model_CCDAList[i].DateOfEncounter) || string.IsNullOrWhiteSpace(model_CCDAList[i].DateOfEncounter))
                    {
                        if (i > 0)
                        {
                            model_CCDAList[i].DateOfEncounter = model_CCDAList[i - 1].DateOfEncounter;
                            model_CCDAList[i].PageNo = model_CCDAList[i - 1].PageNo;
                        }
                        else
                        {
                            model_CCDAList[i].PageNo = 1;
                        }
                    }
                    //if (!(string.IsNullOrEmpty(model_CCDAList[i].DateOfEncounter)) || !(string.IsNullOrWhiteSpace(model_CCDAList[i].DateOfEncounter)))
                    //{
                    //    model_CCDAList[i].PageNo = pageNo;
                    //    pageNo++;
                    //}
                }
                return Tuple.Create(sections_CCDAHL7, txtFilePath, model_CCDAList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CCDA file: {ex.Message}");
            }
            return Tuple.Create(sections_CCDAHL7, txtFilePath, model_CCDAList);


        }
        public static  string ExtractDateFromSentence(string text = "")
        {
            string input = text;
            var dateOfEncounter = "";
            // Regex pattern for different date formats
            string pattern = @"(?:\b(?:0[1-9]|[12][0-9]|3[01])[-/](?:0[1-9]|1[0-2])[-/](?:\d{4})\b" + // DD/MM/YYYY
                 @"|\b(?:0[1-9]|1[0-2])[-/](?:0[1-9]|[12][0-9]|3[01])[-/](?:\d{4})\b" +    // MM/DD/YYYY
                 @"|\b(?:\d{4})[-/](?:0[1-9]|1[0-2])[-/](?:0[1-9]|[12][0-9]|3[01])\b" +    // YYYY/MM/DD
                 @"|\b(?:\d{1,2})[ \-\/]?(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|" +
                 @"Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)[ \-\/]?(?:\d{4})\b)";

            //string pattern = @"(?:\b(?:0[1-9]|1[0-9]|2[0-9]|3[01])[\-/](?:0[1-9]|1[0-2])[\-/](?:\d{4})\b|\b(?:\d{4})[\-/](?:0[1-9]|1[0-2])[\-/](?:0[1-9]|[1-3][0-9])\b|\b(?:\d{1,2}[ \-\/](?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[ \-\/](?:\d{4})\b))";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                dateOfEncounter = match.Value;
            }

            return dateOfEncounter;
        }
        public static void CheckDirectory(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            Console.WriteLine("FileDirectory:" + directory);
            if (!Directory.Exists(directory))
            {
                var path = System.IO.Directory.CreateDirectory(directory);
                Console.WriteLine("CreatedDirectory:" + path);
            }
            else
                Console.WriteLine("Directory Exist");

        }
        public static  string WriteXMLToText(List<string> sectionBase_Conditions = null, string fileName = "", IWebHostEnvironment webHostEnvironment = null,List<CCDA_ToFileWriteModel> cCDA_ToFileWrites=null)
        {
            string textFilePath = "";
            if (sectionBase_Conditions == null || sectionBase_Conditions.Count <= 0)
            { return textFilePath; }
            else
            {
                Random random = new Random();
                int randomNumber = random.Next(1000);
                string newFileName = randomNumber + "_" + fileName;
             
                //var descPath = "D:\\Emed Projects\\EMED Data Interoperability\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\files\\Demo";
                //var prepare_file_path = $"D:\\Emed Projects\\EMED Data Interoperability\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}.xml";

                string folderName = DateTime.Now.ToString("dd_MMM_yyyy");                
                string descPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ECW_EHR_PatientRecords", folderName);
                if (!Directory.Exists(descPath))
                {
                    Directory.CreateDirectory(descPath);
                }

                string safeFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));

                

                // Create the folder in the current directory
                string final_path = Path.Combine(descPath, folderName);
                string text_filePath = Path.Combine(final_path, newFileName + ".txt");
                string html_filePath = Path.Combine(final_path, newFileName + ".html");
                string rtf_filePath = Path.Combine(final_path, newFileName + ".rtf");

                CheckDirectory(text_filePath);
                CheckDirectory(html_filePath);
                CheckDirectory(rtf_filePath);


                var grouped_by_section = cCDA_ToFileWrites?.GroupBy(a => a.Section).ToList();
                // Writing to the file
                using (StreamWriter writer = new StreamWriter(text_filePath))
                {
                    foreach (var item in grouped_by_section)
                    {
                        var values = item.ToList();
                        //writer.WriteLine(item.Key);
                        writer.WriteLine("=== " + item.Key.ToUpper() + " ===");
                        for (int i = 0; i < values.Count(); i++)
                        {
                            writer.WriteLine(values[i].Value);
                        }

                        writer.WriteLine(Environment.NewLine);
                    }

                }
                using (StreamWriter writer = new StreamWriter(html_filePath))
                {
                    writer.WriteLine("<html><body>");

                    foreach (var item in grouped_by_section)
                    {
                        writer.WriteLine($"<p><b><u>{item.Key}</u></b></p>");

                        foreach (var val in item)
                        {
                            writer.WriteLine($"<p>{val.Value}</p>");
                        }

                        writer.WriteLine("<br/>");
                    }

                    writer.WriteLine("</body></html>");
                }

                using (StreamWriter writer = new StreamWriter(rtf_filePath))
                {
                    writer.WriteLine(@"{\rtf1\ansi");

                    foreach (var item in grouped_by_section)
                    {
                        writer.WriteLine(@"\b\ul " + item.Key + @"\ul0\b0\par");

                        foreach (var val in item)
                        {
                            writer.WriteLine(val.Value + @"\par");
                        }

                        writer.WriteLine(@"\par"); // Extra line break
                    }

                    writer.WriteLine("}");
                }


                textFilePath = text_filePath;
            }
            return textFilePath;
        }

        public static string WriteXmlToPdf(List<CCDA_ToFileWriteModel> sections, string fileName)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedPdf");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string pdfPath = Path.Combine(folderPath, $"{fileName}.pdf");

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                        .Text("CCDA Clinical Document")
                        .FontSize(18)
                        .Bold();

                    page.Content().Column(column =>
                    {
                        string currentSection = "";

                        //foreach (var item in sections)
                        //{
                        //    if (currentSection != item.Section)
                        //    {
                        //        currentSection = item.Section;

                        //        column.Item()
                        //            .PaddingTop(10)
                        //            .Text(currentSection)
                        //            .FontSize(14)
                        //            .Bold();
                        //    }

                        //    column.Item()
                        //        .PaddingLeft(10)
                        //        .Text(item.Value);
                        //}

                        foreach (var group in sections.GroupBy(x => x.Section))
                        {
                            column.Item()
                                .Text(group.Key)
                                .FontSize(14)
                                .Bold();

                            foreach (var item in group)
                            {
                                column.Item()
                                    .PaddingLeft(15)
                                    .Text("• " + item.Value);
                            }

                            column.Item().PaddingBottom(10);
                        }
                    });

                    //page.Content().Table(table =>
                    //{
                    //    table.ColumnsDefinition(columns =>
                    //    {
                    //        columns.RelativeColumn(2);
                    //        columns.RelativeColumn(8);
                    //    });

                    //    foreach (var item in sections)
                    //    {
                    //        table.Cell().Text(item.Section).Bold();
                    //        table.Cell().Text(item.Value);
                    //    }
                    //});


                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(pdfPath);

            return pdfPath;
        }

        public static string GenerateClinicalPdfOld(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections,  string fileName)
        {
            //string pdfPath = Path.Combine(  @"D:\ECWPDF",fileName + ".pdf");

            string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
            string uploadPDFPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "ECW_EHR_PatientRecords",folderName);
            if (!Directory.Exists(uploadPDFPath))
            {
                Directory.CreateDirectory(uploadPDFPath);
            }
            
            string safeFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));

            string PDFfilepath = Path.Combine(uploadPDFPath, safeFileName + ".pdf");


            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);

                    page.Margin(20);

                    // HEADER
                    page.Header().Column(column =>
                    {
                        // Top Header Line
                        column.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));

                            text.Span(patient.Name.ToUpper());

                            text.Span($" DOB: {patient.DOB}");

                            text.Span($" ({patient.Age} yo ");

                            text.Span(
                                patient.Gender == "Female"
                                    ? "F)"
                                    : "M)");

                            text.Span($" Acc No. {patient.AccountNumber}");

                            text.Span($" DOS: {patient.DateOfService}");
                        });

                        column.Item().PaddingTop(5);

                        column.Item().LineHorizontal(1);
                    });
                    //// BODY
                    //page.Content().Column(column =>
                    //{
                    //    // Title
                    //    column.Item()
                    //        .AlignCenter()
                    //        .Text("Progress Notes")
                    //        .FontSize(20)
                    //        .Bold();

                    //    column.Item().PaddingVertical(15);

                    //    // Patient Information Block
                    //    column.Item().Row(row =>
                    //    {
                    //        row.RelativeItem().Column(left =>
                    //        {
                    //            left.Item().Text($"Patient: {patient.Name}");
                    //            left.Item().Text($"Account Number: {patient.AccountNumber}");
                    //            left.Item().Text($"DOB: {patient.DOB}");
                    //            left.Item().Text($"Age: {patient.Age}");
                    //            left.Item().Text($"Sex: {patient.Gender}");
                    //            left.Item().Text($"Phone: {patient.Phone}");
                    //            left.Item().Text($"Address: {patient.Address}");
                    //        });

                    //        row.RelativeItem().AlignRight().Column(right =>
                    //        {
                    //            right.Item().Text($"Provider: {patient.Provider}");
                    //            right.Item().Text($"Date: {patient.DateOfService}");
                    //        });
                    //    });

                    //    column.Item().PaddingTop(10);

                    //    column.Item().LineHorizontal(1);

                    //    column.Item().PaddingTop(20);
                    //});


                    page.Content().Column(column =>
                    {
                        // Title
                        column.Item()
                            .AlignCenter()
                            .Text("Patient Health Record")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingVertical(15);

                        // Patient Information Block
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Patient: {patient.Name}");
                                left.Item().Text($"Account Number: {patient.AccountNumber}");
                                left.Item().Text($"DOB: {patient.DOB}");
                                left.Item().Text($"Age: {patient.Age}");
                                left.Item().Text($"Sex: {patient.Gender}");
                                left.Item().Text($"Phone: {patient.Phone}");
                                left.Item().Text($"Address: {patient.Address}");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item().Text($"Provider: {patient.Provider}");
                                right.Item().Text($"Date: {patient.DateOfService}");
                            });
                        });

                        column.Item().PaddingTop(10);

                        column.Item().LineHorizontal(1);

                        column.Item().PaddingTop(20);


                        foreach (var group in sections.GroupBy(x => x.Section))
                        {
                            column.Item()
                                .PaddingTop(10)
                                .Text(group.Key)
                                .Bold()
                                .FontSize(12);

                            foreach (var item in group)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Value))
                                {
                                    column.Item()
                                        .PaddingLeft(10)
                                        .Text("• " + item.Value);
                                }
                            }
                        }
                    });

                    // FOOTER
                    page.Footer().Column(column =>
                    {
                        column.Item().LineHorizontal(1);

                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            })
            .GeneratePdf(PDFfilepath);

            return PDFfilepath;
        }

        
        public static string GenerateClinicalPdfOld1(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections, string fileName)
        {
            string pdfPath = Path.Combine(@"D:\ECWPDF", fileName + ".pdf");

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);

                    page.Margin(20);

                    // HEADER
                    page.Header().Column(column =>
                    {
                        // Top Header Line
                        column.Item().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10));

                            text.Span(patient.Name.ToUpper());

                            text.Span($" DOB: {patient.DOB}");

                            text.Span($" ({patient.Age} yo ");

                            text.Span(
                                patient.Gender == "Female"
                                    ? "F)"
                                    : "M)");

                            text.Span($" Acc No. {patient.AccountNumber}");

                            text.Span($" DOS: {patient.DateOfService}");
                        });

                        column.Item().PaddingTop(5);

                        column.Item().LineHorizontal(1);
                    });
                    //// BODY
                    //page.Content().Column(column =>
                    //{
                    //    // Title
                    //    column.Item()
                    //        .AlignCenter()
                    //        .Text("Progress Notes")
                    //        .FontSize(20)
                    //        .Bold();

                    //    column.Item().PaddingVertical(15);

                    //    // Patient Information Block
                    //    column.Item().Row(row =>
                    //    {
                    //        row.RelativeItem().Column(left =>
                    //        {
                    //            left.Item().Text($"Patient: {patient.Name}");
                    //            left.Item().Text($"Account Number: {patient.AccountNumber}");
                    //            left.Item().Text($"DOB: {patient.DOB}");
                    //            left.Item().Text($"Age: {patient.Age}");
                    //            left.Item().Text($"Sex: {patient.Gender}");
                    //            left.Item().Text($"Phone: {patient.Phone}");
                    //            left.Item().Text($"Address: {patient.Address}");
                    //        });

                    //        row.RelativeItem().AlignRight().Column(right =>
                    //        {
                    //            right.Item().Text($"Provider: {patient.Provider}");
                    //            right.Item().Text($"Date: {patient.DateOfService}");
                    //        });
                    //    });

                    //    column.Item().PaddingTop(10);

                    //    column.Item().LineHorizontal(1);

                    //    column.Item().PaddingTop(20);
                    //});


                    page.Content().Column(column =>
                    {
                        // Title
                        column.Item()
                            .AlignCenter()
                            .Text("Patient Health Record")
                            .FontSize(20)
                            .Bold();

                        column.Item().PaddingVertical(15);

                        // Patient Information Block
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Patient: {patient.Name}");
                                left.Item().Text($"Account Number: {patient.AccountNumber}");
                                left.Item().Text($"DOB: {patient.DOB}");
                                left.Item().Text($"Age: {patient.Age}");
                                left.Item().Text($"Sex: {patient.Gender}");
                                left.Item().Text($"Phone: {patient.Phone}");
                                left.Item().Text($"Address: {patient.Address}");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item().Text($"Provider: {patient.Provider}");
                                right.Item().Text($"Date: {patient.DateOfService}");
                            });
                        });

                        column.Item().PaddingTop(10);

                        column.Item().LineHorizontal(1);

                        column.Item().PaddingTop(20);


                        RenderSoapPdf(column, sections);
                    });

                    // FOOTER
                    page.Footer().Column(column =>
                    {
                        column.Item().LineHorizontal(1);

                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            })
            .GeneratePdf(pdfPath);

            return pdfPath;
        }

        private static void RenderSoapPdf( ColumnDescriptor column, List<CCDA_ToFileWriteModel> sections)
        {
            //-------------------------
            // SUBJECTIVE
            //-------------------------

            column.Item()
                .Text("SUBJECTIVE")
                .Bold()
                .FontSize(16);

            column.Item().PaddingBottom(5);

            RenderSection(
                column,
                "Chief Complaints",
                GetSectionContent(
                    sections,
                    "Chief Complaint",
                    "Chief Complaints"));

            RenderSection(
                column,
                "HPI",
                GetSectionContent(
                    sections,
                    "History of Present Illness",
                    "HPI"));

            RenderSection(
                column,
                "ROS",
                GetSectionContent(
                    sections,
                    "Review of Systems",
                    "ROS"));

            RenderSection(
                column,
                "Medical History",
                GetSectionContent(
                    sections,
                    "Past Medical History",
                    "Medical History"));

            RenderSection(
                column,
                "Family History",
                GetSectionContent(
                    sections,
                    "Family History"));

            RenderSection(
                column,
                "Medications",
                GetSectionContent(
                    sections,
                    "Medications",
                    "Medication"));

            //-------------------------
            // OBJECTIVE
            //-------------------------

            column.Item()
                .PaddingTop(15)
                .Text("OBJECTIVE")
                .Bold()
                .FontSize(16);

            RenderSection(
                column,
                "Vitals",
                GetSectionContent(
                    sections,
                    "Vital Signs",
                    "Vitals"));

            RenderSection(
                column,
                "Physical Examination",
                GetSectionContent(
                    sections,
                    "Physical Exam",
                    "Examination"));

            RenderSection(
                column,
                "Laboratory Results",
                GetSectionContent(
                    sections,
                    "Results",
                    "Laboratory Data",
                    "Diagnostic Results"));

            //-------------------------
            // ASSESSMENT
            //-------------------------

            column.Item()
                .PaddingTop(15)
                .Text("ASSESSMENT")
                .Bold()
                .FontSize(16);

            RenderSection(
                column,
                "Assessment",
                GetSectionContent(
                    sections,
                    "Assessment",
                    "Problems",
                    "Problem List"));

            //-------------------------
            // PLAN
            //-------------------------

            column.Item()
                .PaddingTop(15)
                .Text("PLAN")
                .Bold()
                .FontSize(16);

            RenderSection(
                column,
                "Treatment",
                GetSectionContent(
                    sections,
                    "Plan of Treatment",
                    "Treatment Plan",
                    "Plan"));

            RenderSection(
                column,
                "Orders",
                GetSectionContent(
                    sections,
                    "Orders"));

            RenderSection(
                column,
                "Follow Up",
                GetSectionContent(
                    sections,
                    "Follow Up",
                    "Follow-Up"));
        }

        private static void RenderSection( ColumnDescriptor column, string title, List<string> values)
        {
            if (!values.Any())
                return;

            column.Item()
                .PaddingTop(8)
                .Text(title)
                .Bold()
                .FontSize(12);

            foreach (var value in values)
            {
                column.Item()
                    .PaddingLeft(15)
                    .Text(value);
            }
        }
        private static List<string> GetSectionContent( List<CCDA_ToFileWriteModel> sections, params string[] sectionNames)
        {
            return sections
                .Where(x => sectionNames.Any(s =>
                    x.Section.Equals(s,
                    StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }



        /////////////

        
        public static string GenerateClinicalPdfNew(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections, string fileName)
        {
            string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
            string uploadPDFPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ECW_EHR_PatientRecords", folderName);
            Directory.CreateDirectory(uploadPDFPath);

            string safeFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            string PDFfilepath = Path.Combine(uploadPDFPath, safeFileName + ".pdf");

            // Reorder sections
            var orderedSections = OrderSections(sections);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(20);

                    // Header
                    page.Header().Column(column =>
                    {
                        column.Item().Text($"{patient.Name.ToUpper()} | DOB: {patient.DOB} ({patient.Age} yo) | Sex: {patient.Gender} | Acc No: {patient.AccountNumber} | DOS: {patient.DateOfService}")
                            .FontSize(10);
                        column.Item().LineHorizontal(1);
                    });

                    // Body
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Patient Health Record").Bold().FontSize(20).AlignCenter();
                        column.Item().PaddingVertical(10);

                        // Patient Info
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Patient: {patient.Name}");
                                left.Item().Text($"Account Number: {patient.AccountNumber}");
                                left.Item().Text($"DOB: {patient.DOB}");
                                left.Item().Text($"Age: {patient.Age}");
                                left.Item().Text($"Sex: {patient.Gender}");
                                left.Item().Text($"Phone: {patient.Phone}");
                                left.Item().Text($"Address: {patient.Address}");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item().Text($"Provider: {patient.Provider}");
                                right.Item().Text($"Date: {patient.DateOfService}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);

                        // Sections
                        foreach (var group in orderedSections.GroupBy(s => s.Section))
                        {
                            column.Item().PaddingTop(10).Text(group.Key).Bold().FontSize(12);
                            foreach (var item in group)
                            {
                                column.Item().PaddingLeft(10).Text("• " + item.Value);
                            }
                        }

                        //foreach (var item in orderedSections)
                        //{
                        //    column.Item().PaddingTop(10).Text(item.Section).Bold().FontSize(12);
                        //    column.Item().PaddingLeft(10).Text("• " + item.Value);
                        //}
                    });

                    // Footer
                    page.Footer().Column(column =>
                    {
                        column.Item().LineHorizontal(1);
                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            })
            .GeneratePdf(PDFfilepath);

            return PDFfilepath;
        }


        public static string GenerateClinicalPdf_SoapNote(PatientInfoforPDF patient, List<CCDA_ToFileWriteModel> sections, string fileName)
        {
            var soap = GenerateSoapNote(sections);
            string folderName = DateTime.Now.ToString("dd_MMM_yyyy");
            string uploadPDFPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ECW_EHR_PatientRecords", folderName);
            Directory.CreateDirectory(uploadPDFPath);

            string safeFileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            string PDFfilepath = Path.Combine(uploadPDFPath, safeFileName + ".pdf");

            // Reorder sections
            var orderedSections = OrderSections(sections);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(20);

                    // Header
                    page.Header().Column(column =>
                    {
                        column.Item().Text($"{patient.Name.ToUpper()} | DOB: {patient.DOB} ({patient.Age} yo) | Sex: {patient.Gender} | Acc No: {patient.AccountNumber} | DOS: {patient.DateOfService}")
                            .FontSize(10);
                        column.Item().LineHorizontal(1);
                    });

                    // Body
                    page.Content().Column(column =>
                    {
                        column.Item().Text("Patient Health Record").Bold().FontSize(20).AlignCenter();
                        column.Item().PaddingVertical(10);

                        // Patient Info
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Patient: {patient.Name}");
                                left.Item().Text($"Account Number: {patient.AccountNumber}");
                                left.Item().Text($"DOB: {patient.DOB}");
                                left.Item().Text($"Age: {patient.Age}");
                                left.Item().Text($"Sex: {patient.Gender}");
                                left.Item().Text($"Phone: {patient.Phone}");
                                left.Item().Text($"Address: {patient.Address}");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item().Text($"Provider: {patient.Provider}");
                                right.Item().Text($"Date: {patient.DateOfService}");
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1);

                        //// Sections
                        //foreach (var group in orderedSections.GroupBy(s => s.Section))
                        //{
                        //    column.Item().PaddingTop(10).Text(group.Key).Bold().FontSize(12);
                        //    foreach (var item in group)
                        //    {
                        //        column.Item().PaddingLeft(10).Text("• " + item.Value);
                        //    }
                        //}

                        column.Item().PaddingTop(10).Text("SUBJECTIVE").Bold().FontSize(12);
                        column.Item().PaddingLeft(10).Text(soap.Subjective);

                        column.Item().PaddingTop(10).Text("OBJECTIVE").Bold().FontSize(12);
                        column.Item().PaddingLeft(10).Text(soap.Objective);

                        column.Item().PaddingTop(10).Text("ASSESSMENT").Bold().FontSize(12);
                        column.Item().PaddingLeft(10).Text(soap.Assessment);

                        column.Item().PaddingTop(10).Text("PLAN").Bold().FontSize(12);
                        column.Item().PaddingLeft(10).Text(soap.Plan);

                        if (!string.IsNullOrWhiteSpace(soap.SignedBy))
                        {
                            column.Item().PaddingTop(10).Text("ELECTRONICALLY SIGNED BY").Bold();
                            column.Item().PaddingLeft(10).Text(soap.SignedBy);
                        }

                    });

                    // Footer
                    page.Footer().Column(column =>
                    {
                        column.Item().LineHorizontal(1);
                        column.Item().AlignCenter().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" of ");
                            text.TotalPages();
                        });
                    });
                });
            })
            .GeneratePdf(PDFfilepath);

            return PDFfilepath;
        }

        //Added on 07-06-26
        public static SoapNoteModel GenerateSoapNote(List<CCDA_ToFileWriteModel> sections)
        {
            var grouped = sections
                .GroupBy(x => x.Section)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(
                        Environment.NewLine,
                        g.Select(x => x.Value)));

            var subjective = new StringBuilder();
            var objective = new StringBuilder();
            var assessment = new StringBuilder();
            var plan = new StringBuilder();

            // SUBJECTIVE
            AddSection(grouped, subjective, "Chief Complaint");
            AddSection(grouped, subjective, "HPI");
            AddSection(grouped, subjective, "ROS");
            AddSection(grouped, subjective, "PMH");
            AddSection(grouped, subjective, "PMSFH");
            AddSection(grouped, subjective, "Allergy");

            // OBJECTIVE
            AddSection(grouped, objective, "Physical Exam");

            // ASSESSMENT
            AddSection(grouped, assessment, "MDM or Assessment and Plan");

            // PLAN
            AddSection(grouped, plan, "Orders");
            AddSection(grouped, plan, "Treatment Plan");
            AddSection(grouped, plan, "Referrals");

            string signedBy = "";

            if (grouped.ContainsKey("Electronically Signed By"))
            {
                signedBy = grouped["Electronically Signed By"];
            }

            return new SoapNoteModel
            {
                Subjective = subjective.ToString(),
                Objective = objective.ToString(),
                Assessment = assessment.ToString(),
                Plan = plan.ToString(),
                SignedBy = signedBy
            };
        }

        ///Added below method on 07-06-26
        private static void AddSection(Dictionary<string, string> grouped, StringBuilder sb, string sectionName)
        {
            if (grouped.TryGetValue(sectionName, out string value)
                && !string.IsNullOrWhiteSpace(value))
            {
                sb.AppendLine(sectionName);
                sb.AppendLine("------------------------------------");
                sb.AppendLine(value);
                sb.AppendLine();
            }
        }
        public static List<CCDA_ToFileWriteModel> OrderSections(List<CCDA_ToFileWriteModel> sections)
        {
            return sections
                .OrderBy(s =>
                {
                    int idx = StandardHeaderOrder.IndexOf(s.Section);
                    return idx >= 0 ? idx : int.MaxValue; // Sections not in the list go to the end
                }).ToList();
        }


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

        






        public static readonly Dictionary<string, string> HeaderMappingOld = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"Reason for Visit", "Chief Complaint"},
            {"Chief Complaint / Reason for Visit", "Chief Complaint"},
            {"History of Present Illness", "HPI"},
            {"Review of Systems", "ROS"},
            {"Past Medical History", "PMH"},
            {"Past Medical / Surgical / Family History", "PMSFH"},
            {"Allergies", "Allergy"},
            {"Physical Exam Findings", "Physical Exam"},
            {"Assessment and Plan", "MDM or Assessment and Plan"},
            {"Plan of Care", "MDM or Assessment and Plan"},
            {"Electronic Signature", "Electronically Signed By"}
        };
          

        


        //public static List<CCDA_ToFileWriteModel> ParseCCDASections(string ccdaFilePath)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(ccdaFilePath);

        //    XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
        //    ns.AddNamespace("cda", "urn:hl7-org:v3");

        //    List<CCDA_ToFileWriteModel> sections = new List<CCDA_ToFileWriteModel>();

        //    XmlNodeList sectionNodes = doc.SelectNodes("//cda:section", ns);

        //    if (sectionNodes != null)
        //    {
        //        foreach (XmlNode section in sectionNodes)
        //        {
        //            string rawTitle = section.SelectSingleNode("cda:title", ns)?.InnerText?.Trim();
        //            if (string.IsNullOrEmpty(rawTitle))
        //                continue;

        //            // Map alternate titles to standard headers
        //            string header = HeaderMapping.ContainsKey(rawTitle) ? HeaderMapping[rawTitle] : rawTitle;

        //            XmlNodeList textNodes = section.SelectNodes("cda:text//text()", ns);
        //            if (textNodes != null)
        //            {
        //                foreach (XmlNode textNode in textNodes)
        //                {
        //                    string content = textNode.InnerText.Trim();
        //                    if (!string.IsNullOrWhiteSpace(content))
        //                    {
        //                        sections.Add(new CCDA_ToFileWriteModel
        //                        {
        //                            Section = header,
        //                            Value = content
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return sections;
        //}
    }
}
