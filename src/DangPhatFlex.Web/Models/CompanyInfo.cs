using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class CompanyInfo
{
    public int Id { get; set; }

    [Display(Name = "Tên pháp lý")]
    [Required, MaxLength(300)]
    public string LegalName { get; set; } = string.Empty;

    [Display(Name = "Tên thương hiệu")]
    [Required, MaxLength(150)]
    public string BrandName { get; set; } = string.Empty;

    [Display(Name = "Slogan")]
    [Required, MaxLength(300)]
    public string Tagline { get; set; } = string.Empty;

    [Display(Name = "Nội dung giới thiệu")]
    public string AboutContent { get; set; } = string.Empty;

    [Display(Name = "Sứ mệnh")]
    [MaxLength(500)]
    public string? Mission { get; set; }

    [Display(Name = "Tầm nhìn")]
    [MaxLength(500)]
    public string? Vision { get; set; }

    /// <summary>One advantage per line, rendered as a bulleted list on the About page.</summary>
    [Display(Name = "Lợi thế cạnh tranh (mỗi dòng một mục)")]
    public string? Advantages { get; set; }

    [Display(Name = "Địa chỉ")]
    [Required, MaxLength(400)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Hotline")]
    [Required, MaxLength(50)]
    public string Hotline { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Link nhúng bản đồ Google")]
    [MaxLength(600)]
    public string? MapEmbedUrl { get; set; }

    [Display(Name = "Giá trị cốt lõi: Nhanh nhất")]
    [MaxLength(200)]
    public string? CoreValueFast { get; set; }

    [Display(Name = "Giá trị cốt lõi: Tốt nhất")]
    [MaxLength(200)]
    public string? CoreValueBest { get; set; }

    [Display(Name = "Giá trị cốt lõi: Giá cạnh tranh nhất")]
    [MaxLength(200)]
    public string? CoreValueCompetitivePrice { get; set; }
}
