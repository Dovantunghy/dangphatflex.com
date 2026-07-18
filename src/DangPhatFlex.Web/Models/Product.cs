using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class Product
{
    public int Id { get; set; }

    public int ProductCategoryId { get; set; }
    public ProductCategory? ProductCategory { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(400)]
    public string? MainImageUrl { get; set; }

    [MaxLength(400)]
    public string? MainImageAlt { get; set; }

    [MaxLength(400)]
    public string? DatasheetPdfUrl { get; set; }

    [MaxLength(100)]
    public string? InnerDiameter { get; set; }

    [MaxLength(100)]
    public string? OuterDiameter { get; set; }

    [MaxLength(100)]
    public string? HoseType { get; set; }

    [MaxLength(50)]
    public string? MaxTemperature { get; set; }

    [MaxLength(100)]
    public string? MaxPressure { get; set; }

    [MaxLength(100)]
    public string? MinBendRadius { get; set; }

    [MaxLength(200)]
    public string? Standards { get; set; }

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(300)]
    public string? MetaDescription { get; set; }

    public List<ProductVariant> Variants { get; set; } = new();
    public List<Accessory> Accessories { get; set; } = new();
}
