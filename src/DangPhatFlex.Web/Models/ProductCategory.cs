using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ProductCategory
{
    public int Id { get; set; }

    [Display(Name = "Tên danh mục")]
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Đường dẫn (slug)")]
    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Meta title (SEO)")]
    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta description (SEO)")]
    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    public List<Product> Products { get; set; } = new();
}
