using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ContactSubmission
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public bool IsProcessed { get; set; }
}
