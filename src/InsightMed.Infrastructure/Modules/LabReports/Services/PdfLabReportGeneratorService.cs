using InsightMed.Application.Modules.LabReports.Models;
using InsightMed.Application.Modules.LabReports.Services.Abstactions;
using InsightMed.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InsightMed.Infrastructure.Modules.LabReports.Services;

public sealed class PdfLabReportGeneratorService : IPdfLabReportGeneratorService
{
    public byte[] GenerateLabReportPdf(LabReport report, List<ReportItemDto> parsedContent)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page
                    .Header()
                    .Row(row =>
                    {
                        row
                            .RelativeItem()
                            .Column(column =>
                            {
                                column
                                    .Item()
                                    .Text("Lab Report Details")
                                    .SemiBold()
                                    .FontSize(20)
                                    .FontColor(Colors.Blue.Medium);

                                column
                                    .Item()
                                    .Text($"{report.Patient.FirstName} {report.Patient.LastName}")
                                    .FontSize(14);

                                column
                                    .Item()
                                    .Text($"UID: {report.Patient.Uid}")
                                    .FontColor(Colors.Grey.Medium);
                            });

                        row
                            .ConstantItem(100)
                            .AlignRight()
                            .Text(report.Created.ToString("MMM d, yyyy"));
                    });

                page
                    .Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Parameter
                            columns.RelativeColumn(2); // Range
                            columns.RelativeColumn(1); // Value
                        });

                        table.Header(header =>
                        {
                            header
                                .Cell()
                                .Element(CellStyle)
                                .Text("Parameter");

                            header
                                .Cell()
                                .Element(CellStyle)
                                .Text("Normal Range/Value");

                            header
                                .Cell()
                                .Element(CellStyle)
                                .Text("Measured Value");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .DefaultTextStyle(x => x.SemiBold())
                                    .PaddingVertical(5)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten1);
                            }
                        });

                        // Rows
                        foreach (var item in parsedContent)
                        {
                            bool isAbnormal = CheckIfAbnormal(item);
                            var backgroundColor = isAbnormal ? Colors.Red.Lighten4 : Colors.White;

                            table
                                .Cell()
                                .Background(backgroundColor)
                                .Padding(5)
                                .Text(item.Name);

                            table
                                .Cell()
                                .Background(backgroundColor)
                                .Padding(5)
                                .Text(GetReferenceDisplay(item));

                            table
                                .Cell()
                                .Background(backgroundColor)
                                .Padding(5)
                                .Text(GetMeasurementDisplay(item))
                                .SemiBold();
                        }
                    });

                page
                    .Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();
    }

    private bool CheckIfAbnormal(ReportItemDto item)
    {
        double? val = item.Measurement;
        double? min = item.Reference.MinThreshold;
        double? max = item.Reference.MaxThreshold;

        if (val is not null && min is not null && max is not null)
            return val < min || val > max;

        if (item.IsPositive is not null && item.Reference.Positive is not null)
            return item.IsPositive != item.Reference.Positive;

        return false;
    }

    private string GetReferenceDisplay(ReportItemDto item)
    {
        if (item.Reference.MinThreshold is not null && item.Reference.MaxThreshold is not null)
            return $"{item.Reference.MinThreshold} - {item.Reference.MaxThreshold}";

        if (item.Reference.Positive is not null)
            return item.Reference.Positive.Value ? "Positive" : "Negative";

        return "N/A";
    }

    private string GetMeasurementDisplay(ReportItemDto item)
    {
        if (item.Measurement is not null)
            return item.Measurement.ToString() ?? string.Empty;

        if (item.IsPositive is not null)
            return item.IsPositive.Value ? "Positive" : "Negative";

        return "-";
    }
}