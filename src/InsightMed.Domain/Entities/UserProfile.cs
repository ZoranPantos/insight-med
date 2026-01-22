namespace InsightMed.Domain.Entities;

public class UserProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime? PasswordLastChanged { get; set; }
}