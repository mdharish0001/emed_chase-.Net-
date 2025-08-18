using System.Text;
using System.Xml;

namespace emedl_chase.Helper
{
    public class XmlConvertor
    {
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


}
}
