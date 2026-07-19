using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class CompanyInfo
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string LegalName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string BrandName { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Tagline { get; set; } = string.Empty;

    public string AboutContent { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Mission { get; set; }

    [MaxLength(500)]
    public string? Vision { get; set; }

    /// <summary>One advantage per line, rendered as a bulleted list on the About page.</summary>
    public string? Advantages { get; set; }

    [Required, MaxLength(400)]
    public string Address { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Hotline { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(600)]
    public string? MapEmbedUrl { get; set; }

    [MaxLength(200)]
    public string? CoreValueFast { get; set; }

    [MaxLength(200)]
    public string? CoreValueBest { get; set; }

    [MaxLength(200)]
    public string? CoreValueCompetitivePrice { get; set; }
}
