using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ProductCategory
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    public List<Product> Products { get; set; } = new();
}
