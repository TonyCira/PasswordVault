namespace PasswordVault.Models;

public enum EntryCategory
{
    Website,
    Software,
    PinCode,
    Note
}

public class VaultEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public EntryCategory Category { get; set; }
    public string Title { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Url { get; set; } = "";
    public string LicenseKey { get; set; } = "";
    public string PinCode { get; set; } = "";
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public string CategoryLabel => Category switch
    {
        EntryCategory.Website => "🌐 Website",
        EntryCategory.Software => "💿 Software",
        EntryCategory.PinCode => "🔢 PIN / Bankkaart",
        EntryCategory.Note => "📝 Notitie",
        _ => "?"
    };

    public string CategoryIcon => Category switch
    {
        EntryCategory.Website => "🌐",
        EntryCategory.Software => "💿",
        EntryCategory.PinCode => "🔢",
        EntryCategory.Note => "📝",
        _ => "?"
    };
}
