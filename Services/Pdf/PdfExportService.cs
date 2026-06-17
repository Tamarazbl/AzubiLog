using AzubiLog.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AzubiLog.Services.Pdf;

public static class PdfExportService
{
    public static byte[] GenerateWeeklyReport(WeeklyReport report, ApplicationUser user)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(t => t.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Text("Ausbildungsnachweis (Wochenbericht)")
                        .Bold().FontSize(16);
                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Text($"Name: {user.FirstName} {user.LastName}");
                        row.RelativeItem().Text($"KW {report.CalendarWeek} / {report.Year}");
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Ausbildungsberuf: {user.TrainingOccupation}");
                        row.RelativeItem().Text($"Ausbildungsjahr: {user.TrainingYear}");
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Berufsschule: {user.School}");
                        row.RelativeItem().Text($"Klasse: {user.ClassName}");
                    });
                    col.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(10).Column(col =>
                {
                    if (report.Entries.Count == 0)
                    {
                        col.Item().Text("Keine Einträge für diese Woche.").Italic();
                    }
                    else
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);  // Datum
                                columns.ConstantColumn(80);  // Typ
                                columns.RelativeColumn(2);   // Titel
                                columns.RelativeColumn(3);   // Beschreibung
                                columns.ConstantColumn(45);  // Stunden
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Datum");
                                header.Cell().Element(HeaderCell).Text("Typ");
                                header.Cell().Element(HeaderCell).Text("Tätigkeit");
                                header.Cell().Element(HeaderCell).Text("Beschreibung");
                                header.Cell().Element(HeaderCell).Text("Std.");

                                static IContainer HeaderCell(IContainer container) =>
                                    container.Padding(4)
                                        .Background(Colors.Grey.Lighten3)
                                        .Border(0.5f)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .DefaultTextStyle(t => t.Bold().FontSize(9));
                            });

                            foreach (var entry in report.Entries.OrderBy(e => e.Date))
                            {
                                table.Cell().Element(DataCell).Text(entry.Date.ToString("dd.MM."));
                                table.Cell().Element(DataCell).Text(entry.DayType);
                                table.Cell().Element(DataCell).Text(entry.Title);
                                table.Cell().Element(DataCell).Text(entry.Description);
                                table.Cell().Element(DataCell).Text(entry.Duration.ToString("0.#"));

                                static IContainer DataCell(IContainer container) =>
                                    container.Padding(4)
                                        .Border(0.5f)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .DefaultTextStyle(t => t.FontSize(9));
                            }
                        });

                        col.Item().PaddingTop(8).AlignRight()
                            .Text($"Gesamtstunden: {report.Entries.Sum(e => e.Duration):0.#}")
                            .Bold();
                    }

                    if (!string.IsNullOrEmpty(report.Comment))
                    {
                        col.Item().PaddingTop(12).Text("Bemerkung:").Bold();
                        col.Item().Text(report.Comment);
                    }

                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().LineHorizontal(0.5f);
                            c.Item().Text("Unterschrift Auszubildende/r").FontSize(8);
                        });
                        row.ConstantItem(40);
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().LineHorizontal(0.5f);
                            c.Item().Text("Unterschrift Ausbilder/in").FontSize(8);
                        });
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Seite ").FontSize(8);
                    text.CurrentPageNumber().FontSize(8);
                });
            });
        });

        return document.GeneratePdf();
    }
}
