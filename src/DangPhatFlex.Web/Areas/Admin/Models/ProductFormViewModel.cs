using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DangPhatFlex.Web.Areas.Admin.Models;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required]
    public int ProductCategoryId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public IFormFile? MainImage { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập alt text cho ảnh")]
    public string MainImageAlt { get; set; } = string.Empty;

    public IFormFile? DatasheetPdf { get; set; }

    public string? InnerDiameter { get; set; }
    public string? OuterDiameter { get; set; }
    public string? HoseType { get; set; }
    public string? MaxTemperature { get; set; }
    public string? MaxPressure { get; set; }
    public string? MinBendRadius { get; set; }
    public string? Standards { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}
