using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class NewsArticle
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required, MaxLength(400)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(400)]
    public string? CoverImageAlt { get; set; }

    public DateTime PublishedAt { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(300)]
    public string? MetaDescription { get; set; }
}
