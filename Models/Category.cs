namespace AzubiLog.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#2F855A";

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
}
