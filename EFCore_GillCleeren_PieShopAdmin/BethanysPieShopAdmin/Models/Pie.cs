using System.ComponentModel.DataAnnotations;

namespace BethanysPieShopAdmin.Models;

public class Pie
{
    public int PieId { get; set; }

    [Display(Name = "Name")] [Required] public string Name { get; set; } = string.Empty;

    [StringLength(100)] public string? ShortDescription { get; set; }

    [StringLength(1000)] public string? LongDescription { get; set; }

    [StringLength(1000)] public string? AllergyInformation { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageThumbnailUrl { get; set; }

    public bool IsPieOfTheWeek { get; set; }

    public bool InStock { get; set; }


    public int CategoryId { get; set; } // FK
    public Category? Category { get; set; } // Navigation prop 1 Pies belongs to 1 Category

    public ICollection<Ingredient>? Ingredients { get; set; } // reference type, Navigation prop
}