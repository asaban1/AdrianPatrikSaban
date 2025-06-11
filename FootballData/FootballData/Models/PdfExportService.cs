using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace FootballData.DataLayer
{
    public class PdfExportService
    {
        public void ExportToPdf(string filePath, List<string[]> data, string[] headers, string title)
        {
            using (PdfWriter writer = new PdfWriter(filePath))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf, PageSize.A4))
            {
                PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                document.Add(new Paragraph(title).SetFont(fontBold).SetFontSize(16));
                document.Add(new Paragraph($"Date: {System.DateTime.Now:dd.MM.yyyy HH:mm}\n\n"));

                Table table = new Table(UnitValue.CreatePercentArray(headers.Length)).UseAllAvailableWidth();

                foreach (var header in headers)
                {
                    table.AddHeaderCell(new Cell().Add(new Paragraph(header).SetFont(fontBold)));
                }

                foreach (var row in data)
                {
                    foreach (var cell in row)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(cell)));
                    }
                }

                document.Add(table);
            }
        }
    }
}
