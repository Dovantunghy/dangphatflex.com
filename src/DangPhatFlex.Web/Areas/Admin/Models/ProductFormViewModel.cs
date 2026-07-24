using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Areas.Admin.Models;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Danh mục")]
    [Required]
    public int ProductCategoryId { get; set; }

    [Display(Name = "Tên sản phẩm")]
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Đường dẫn (slug)")]
    public string? Slug { get; set; }

    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Ảnh chính")]
    public IFormFile? MainImage { get; set; }

    [Display(Name = "Mô tả ảnh (alt text)")]
    [Required(ErrorMessage = "Vui lòng nhập alt text cho ảnh")]
    public string MainImageAlt { get; set; } = string.Empty;

    [Display(Name = "File datasheet (PDF)")]
    public IFormFile? DatasheetPdf { get; set; }

    [Display(Name = "Đường kính trong")]
    public string? InnerDiameter { get; set; }

    [Display(Name = "Đường kính ngoài")]
    public string? OuterDiameter { get; set; }

    [Display(Name = "Loại ống")]
    public string? HoseType { get; set; }

    [Display(Name = "Nhiệt độ hoạt động tối đa")]
    public string? MaxTemperature { get; set; }

    [Display(Name = "Áp suất hoạt động tối đa")]
    public string? MaxPressure { get; set; }

    [Display(Name = "Bán kính uốn cong nhỏ nhất")]
    public string? MinBendRadius { get; set; }

    [Display(Name = "Tiêu chuẩn")]
    public string? Standards { get; set; }

    [Display(Name = "Meta title (SEO)")]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta description (SEO)")]
    public string? MetaDescription { get; set; }
}
