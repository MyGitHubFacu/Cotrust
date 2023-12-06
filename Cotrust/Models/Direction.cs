using System.ComponentModel.DataAnnotations;

namespace Cotrust.Models
{
    public class Direction
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
    }
}
