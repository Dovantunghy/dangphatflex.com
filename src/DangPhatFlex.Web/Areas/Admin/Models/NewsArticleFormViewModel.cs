using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Areas.Admin.Models;

public class NewsArticleFormViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    [Required, MaxLength(400)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public IFormFile? CoverImage { get; set; }

    public string? CoverImageAlt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
}
