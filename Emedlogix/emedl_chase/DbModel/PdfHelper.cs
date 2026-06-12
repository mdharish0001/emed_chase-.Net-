
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace emedl_chase.DbModel
{
    public static class PdfHelper
    {
        public static byte[] CreatePdf(string xml)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                        .Text("ECW Clinical Document")
                        .FontSize(18)
                        .Bold();

                    page.Content()
                        .Text(xml)
                        .FontSize(8);
                });
            })
            .GeneratePdf();
        }
    }
}
