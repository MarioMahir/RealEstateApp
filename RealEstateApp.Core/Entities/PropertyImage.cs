namespace RealEstateApp.Core.Entities;

public class PropertyImage
{
    public int Id { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
}
