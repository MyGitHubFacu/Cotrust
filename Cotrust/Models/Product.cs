
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cotrust.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Stock { get; set; }
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public TypeOfProduct Kind { get; set; }        
        public string? Image { get; set; }
        
        [NotMapped]
        public IFormFile? File { get; set; }

        public enum TypeOfProduct
        {
            PLC = 100,
            Module = 200,
            HMI = 300,
            Servo = 400,
            Driver = 500,
            Software = 600,
            Kits = 700,
        }
    }
}
