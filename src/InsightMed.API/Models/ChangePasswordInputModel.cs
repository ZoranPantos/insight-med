namespace InsightMed.API.Models;

public class ChangePasswordInputModel
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}