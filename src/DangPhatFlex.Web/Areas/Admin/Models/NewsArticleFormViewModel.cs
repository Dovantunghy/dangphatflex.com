using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Areas.Admin.Models;

public class NewsArticleFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Tiêu đề")]
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Đường dẫn (slug)")]
    public string? Slug { get; set; }

    [Display(Name = "Tóm tắt")]
    [Required, MaxLength(400)]
    public string Summary { get; set; } = string.Empty;

    [Display(Name = "Nội dung")]
    [Required]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Ảnh bìa")]
    public IFormFile? CoverImage { get; set; }

    [Display(Name = "Mô tả ảnh bìa (alt text)")]
    public string? CoverImageAlt { get; set; }

    [Display(Name = "Ngày đăng")]
    public DateTime? PublishedAt { get; set; }

    [Display(Name = "Meta title (SEO)")]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta description (SEO)")]
    public string? MetaDescription { get; set; }
}
