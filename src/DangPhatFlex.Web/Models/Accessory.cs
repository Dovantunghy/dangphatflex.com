using System.ComponentModel.DataAnnotations;

namespace DangPhatFlex.Web.Models;

public class Accessory
{
    public int Id { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? ImageUrl { get; set; }

    [MaxLength(400)]
    public string? ImageAlt { get; set; }

    public int DefaultQuantity { get; set; } = 1;
}
