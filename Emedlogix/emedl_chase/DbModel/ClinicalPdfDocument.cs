using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace emedl_chase.DbModel
{
    public class ClinicalPdfDocument:IDocument
    {
        private readonly ClinicalSummary _data;

        public ClinicalPdfDocument(ClinicalSummary data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata()  => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);

                //page.Header()
                //    .Text("PATIENT HEALTH RECORD")
                //    .FontSize(20)
                //    .Bold();

                page.Header().Column(column =>
                {
                    column.Item().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(10));

                        text.Span(_data.Patient.Name.ToUpper());

                        text.Span($" DOB: {_data.Patient.DOB}");

                        text.Span($" ({_data.Patient.Age} yo ");

                        text.Span(
                            _data.Patient.Gender == "Female"
                                ? "F)"
                                : "M)");

                        text.Span($" Acc No. {_data.Patient.AccountNumber}");

                        text.Span($" DOS: {_data.Patient.DateOfService}");
                    });

                    column.Item().PaddingTop(5);

                    column.Item().LineHorizontal(1);
                });

                page.Content().Column(col =>
                {
                    AddPatientSection(col);

                    //AddAllergySection(col);

                    AddProblemSection(col);

                    //AddMedicationSection(col);

                    AddLabResultsSection(col);

                    //AddEncounterSection(col);
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem()
                       .Text($"Provider: {_data.Patient.ProviderName}");

                    row.RelativeItem()
                       .AlignRight()
                       .Text($"Date: {_data.Patient.DateOfService}");
                });
            });
        }

        private void AddPatientSection(ColumnDescriptor col)
        {
            col.Item()
               .AlignCenter()
               .Text("Progress Notes")
               .FontSize(18)
               .Bold();

            col.Item().PaddingTop(15);

            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text($"Patient: {_data.Patient.Name}");
                    left.Item().Text($"Account Number: {_data.Patient.AccountNumber}");
                    left.Item().Text($"DOB: {_data.Patient.DOB}");
                    left.Item().Text($"Age: {_data.Patient.Age}");
                    left.Item().Text($"Sex: {_data.Patient.Gender}");
                    left.Item().Text($"Phone: {_data.Patient.Phone}");
                    left.Item().Text($"Address: {_data.Patient.Address}");
                });

                row.RelativeItem().Column(right =>
                {
                    right.Item().AlignRight()
                                .Text($"Provider: {_data.Patient.ProviderName}");

                    right.Item().AlignRight()
                                .Text($"Date: {_data.Patient.DateOfService}");
                });
            });

            col.Item().PaddingTop(10);

            col.Item().LineHorizontal(1);
        }
        private void AddLabResultsSection(ColumnDescriptor col)
        {
            col.Item().PaddingTop(20);

            col.Item()
               .Text("LAB RESULTS")
               .Bold()
               .FontSize(14);

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Test");
                    header.Cell().Text("Value");
                    header.Cell().Text("Range");
                });

                foreach (var lab in _data.LabResults)
                {
                    table.Cell().Text(lab.TestName);

                    table.Cell().Text(lab.Value);

                    table.Cell().Text(lab.ReferenceRange);
                }
            });
        }
        private void AddProblemSection(ColumnDescriptor col)
        {
            if (!_data.Problems.Any())
                return;

            col.Item().PaddingTop(20);

            col.Item()
                .Text("PROBLEMS / DIAGNOSES")
                .Bold();

            foreach (var item in _data.Problems)
            {
                col.Item().Text($"• {item.Diagnosis}");
            }
        }


    }
}
