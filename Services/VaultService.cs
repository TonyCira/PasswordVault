using System.IO;
using ClosedXML.Excel;
using PasswordVault.Models;

namespace PasswordVault.Services;

public class VaultService
{
    private readonly string _vaultFolder;
    private readonly string _excelPath;
    private List<VaultEntry> _entries = new();

    public VaultService()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _vaultFolder = Path.Combine(desktop, "PasswordVault");
        _excelPath = Path.Combine(_vaultFolder, "vault.xlsx");

        Directory.CreateDirectory(_vaultFolder);
        LoadFromExcel();
    }

    public string VaultFolder => _vaultFolder;

    public List<VaultEntry> GetAll() => _entries.OrderByDescending(e => e.UpdatedAt).ToList();

    public List<VaultEntry> Search(string query, EntryCategory? category = null)
    {
        var all = GetAll();
        if (category.HasValue)
            all = all.Where(e => e.Category == category.Value).ToList();

        if (string.IsNullOrWhiteSpace(query))
            return all;

        query = query.ToLower();
        return all.Where(e =>
            e.Title.ToLower().Contains(query) ||
            e.Username.ToLower().Contains(query) ||
            e.Url.ToLower().Contains(query) ||
            e.Note.ToLower().Contains(query) ||
            e.LicenseKey.ToLower().Contains(query)
        ).ToList();
    }

    public void Add(VaultEntry entry)
    {
        _entries.Add(entry);
        SaveToExcel();
    }

    public void Update(VaultEntry entry)
    {
        var existing = _entries.FirstOrDefault(e => e.Id == entry.Id);
        if (existing != null)
        {
            _entries.Remove(existing);
            entry.UpdatedAt = DateTime.Now;
            _entries.Add(entry);
            SaveToExcel();
        }
    }

    public void Delete(Guid id)
    {
        _entries.RemoveAll(e => e.Id == id);
        SaveToExcel();
    }

    private void LoadFromExcel()
    {
        if (!File.Exists(_excelPath)) return;

        using var wb = new XLWorkbook(_excelPath);
        var ws = wb.Worksheet(1);

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            try
            {
                _entries.Add(new VaultEntry
                {
                    Id = Guid.TryParse(row.Cell(1).GetString(), out var id) ? id : Guid.NewGuid(),
                    Category = Enum.TryParse<EntryCategory>(row.Cell(2).GetString(), out var cat) ? cat : EntryCategory.Website,
                    Title = row.Cell(3).GetString(),
                    Username = row.Cell(4).GetString(),
                    Password = row.Cell(5).GetString(),
                    Url = row.Cell(6).GetString(),
                    LicenseKey = row.Cell(7).GetString(),
                    PinCode = row.Cell(8).GetString(),
                    Note = row.Cell(9).GetString(),
                    CreatedAt = row.Cell(10).GetDateTime(),
                    UpdatedAt = row.Cell(11).GetDateTime(),
                });
            }
            catch { /* skip malformed rows */ }
        }
    }

    private void SaveToExcel()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Vault");

        // Header
        var headers = new[] { "ID", "Categorie", "Titel", "Gebruikersnaam", "Wachtwoord", "URL", "Licentie", "PIN", "Notitie", "Aangemaakt", "Bijgewerkt" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1a1a2e");
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }

        // Data
        int row = 2;
        foreach (var e in _entries)
        {
            ws.Cell(row, 1).Value = e.Id.ToString();
            ws.Cell(row, 2).Value = e.Category.ToString();
            ws.Cell(row, 3).Value = e.Title;
            ws.Cell(row, 4).Value = e.Username;
            ws.Cell(row, 5).Value = e.Password;
            ws.Cell(row, 6).Value = e.Url;
            ws.Cell(row, 7).Value = e.LicenseKey;
            ws.Cell(row, 8).Value = e.PinCode;
            ws.Cell(row, 9).Value = e.Note;
            ws.Cell(row, 10).Value = e.CreatedAt;
            ws.Cell(row, 11).Value = e.UpdatedAt;
            row++;
        }

        ws.Columns().AdjustToContents();
        wb.SaveAs(_excelPath);
    }
}
