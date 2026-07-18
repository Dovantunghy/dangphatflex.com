using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required, MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string InletOutlet { get; set; } = string.Empty;

    public int InstallLengthMm { get; set; }

    public int MaxBends90 { get; set; }

    [MaxLength(50)]
    public string? MinBendRadiusIn { get; set; }

    [MaxLength(50)]
    public string? EquivalentSteelPipeLength { get; set; }
}
