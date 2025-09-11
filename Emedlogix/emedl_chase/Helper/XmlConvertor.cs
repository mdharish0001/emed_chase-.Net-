using emedl_chase.Model;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

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
                var prepare_file_path = $"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}.xml";
          
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
                // 1. Decode Base64 to byte array
                byte[] xmlBytes = Convert.FromBase64String(base64data);

                // 2. Convert bytes to UTF-8 string
                string xmlContent = Encoding.UTF8.GetString(xmlBytes);

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
                var prepare_file_path = $"D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\Output\\{patient_name}_{dos}_{type}.xml";

                File.WriteAllText(prepare_file_path, xmlContent, Encoding.UTF8);

                Console.WriteLine("✅ XML file saved successfully at: " + Path.GetFullPath(prepare_file_path));
                return prepare_file_path;
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
            //if (root != null)
            //{
            //    foreach (XmlNode node in root.ChildNodes)
            //    {
            //        // Check if it's an element and has inner text
            //        if (node.NodeType == XmlNodeType.Element)
            //        {
            //            Console.WriteLine($"{node.Name}: {node.InnerText}");
            //        }
            //    }
            //}
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

        public static  Tuple<List<string>, string, List<CCDAHL7Model>> ReadCCDAFile(string uplodedXMLFilePath = "")
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

                if (entry_notes != null && entry_notes.Count >0) {
                
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
                txtFilePath = WriteXMLToText(sectionBase_Conditions, fileName );
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
        public static  string WriteXMLToText(List<string> sectionBase_Conditions = null, string fileName = "", IWebHostEnvironment webHostEnvironment = null)
        {
            string textFilePath = "";
            if (sectionBase_Conditions == null || sectionBase_Conditions.Count <= 0)
            { return textFilePath; }
            else
            {
                Random random = new Random();
                int randomNumber = random.Next(1000);
                string newFileName = randomNumber + "_" + fileName;
                var descPath = "D:\\DotnetProjects\\emed_chase-.Net-\\Emedlogix\\emedl_chase\\wwwroot\\files\\Demo";
                string text_filePath = Path.Combine(descPath, newFileName + ".txt");

                CheckDirectory(text_filePath);
                // Writing to the file
                using (StreamWriter writer = new StreamWriter(text_filePath))
                {
                    foreach (string value in sectionBase_Conditions)
                    {
                        writer.WriteLine(value);
                    }
                }
                textFilePath = text_filePath;
            }
            return textFilePath;
        }
    }
}
