using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Cotrust.Models
{
    public class User
    {
        public User()
        {
            Directions = new List<Direction>();
            Products = new List<CartProduct>();
            Buys = new List<Buys>();
            Packages = new List<Package>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EmailConfirmed { get; set; } = false;
        public TypeOfUser Type { get; set; }
        public ICollection<Direction> Directions { get; set; }
        public ICollection<CartProduct> Products { get; set; } 
        public ICollection<Buys> Buys { get; set; }
        public ICollection<Package> Packages { get; set; }

        [NotMapped]
        public string? Password2 { get; set; }

        public enum TypeOfUser
        {
            Admin = 100,
            Staff = 200,
            Customer = 300,
        }
    }
}
