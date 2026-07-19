namespace RealEstateApp.Core.Models;

public class StaffUpdateResult
{
    public StaffActionStatus Status { get; set; }
    public List<string> Errors { get; set; } = new();
}
