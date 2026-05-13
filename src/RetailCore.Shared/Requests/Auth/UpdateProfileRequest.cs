using System.ComponentModel.DataAnnotations;

namespace RetailCore.Shared.Requests.Auth;

public class UpdateProfileRequest
{
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(2048)]
    public string? AvatarUrl { get; set; }
}
