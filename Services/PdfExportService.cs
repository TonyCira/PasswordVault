using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PasswordVault.Models;

namespace PasswordVault.Services;

public class PdfExportService
{
    static PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public string ExportToPdf(List<VaultEntry> entries, string outputFolder)
    {
        string fileName = $"PasswordVault_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        string filePath = Path.Combine(outputFolder, fileName);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);

                page.Header().Column(col =>
                {
                    col.Item().Text("🔐 PasswordVault Export")
                        .FontSize(24).Bold().FontColor(Color.FromHex("1a1a2e"));
                    col.Item().Text($"Gegenereerd op {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Color.FromHex("6c63ff"));
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    var grouped = entries.GroupBy(e => e.Category);

                    foreach (var group in grouped)
                    {
                        col.Item().PaddingTop(15).Text(GetCategoryTitle(group.Key))
                            .FontSize(14).Bold().FontColor(Color.FromHex("6c63ff"));

                        foreach (var entry in group)
                        {
                            col.Item().PaddingTop(8).Border(1).BorderColor(Colors.Grey.Lighten2)
                                .Padding(10).Column(entryCol =>
                                {
                                    entryCol.Item().Text(entry.Title).FontSize(12).Bold();

                                    if (!string.IsNullOrEmpty(entry.Username))
                                        entryCol.Item().Text($"Gebruikersnaam: {entry.Username}").FontSize(10);

                                    if (!string.IsNullOrEmpty(entry.Password))
                                        entryCol.Item().Text($"Wachtwoord: {entry.Password}").FontSize(10);

                                    if (!string.IsNullOrEmpty(entry.Url))
                                        entryCol.Item().Text($"URL: {entry.Url}").FontSize(10).FontColor(Colors.Blue.Medium);

                                    if (!string.IsNullOrEmpty(entry.LicenseKey))
                                        entryCol.Item().Text($"Licentie: {entry.LicenseKey}").FontSize(10);

                                    if (!string.IsNullOrEmpty(entry.PinCode))
                                        entryCol.Item().Text($"PIN: {entry.PinCode}").FontSize(10);

                                    if (!string.IsNullOrEmpty(entry.Note))
                                        entryCol.Item().Text($"Notitie: {entry.Note}").FontSize(10).Italic();

                                    entryCol.Item().Text($"Bijgewerkt: {entry.UpdatedAt:dd/MM/yyyy}")
                                        .FontSize(8).FontColor(Colors.Grey.Medium);
                                });
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("PasswordVault — Pagina ").FontSize(9).FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                    x.Span(" van ").FontSize(9).FontColor(Colors.Grey.Medium);
                    x.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf(filePath);

        return filePath;
    }

    private string GetCategoryTitle(EntryCategory cat) => cat switch
    {
        EntryCategory.Website => "🌐 Websites",
        EntryCategory.Software => "💿 Software Licenties",
        EntryCategory.PinCode => "🔢 PIN Codes & Bankkaarten",
        EntryCategory.Note => "📝 Notities",
        _ => "Overige"
    };
}
