using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cotrust.Models
{
    public class Buys
    {
        public Buys()
        {
            Products = new List<BuysProduct>();
        }

        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public ICollection<BuysProduct> Products { get; set; }
        public DateTime Date { get; set; }
        public string Direction { get; set; } = null!;
    }
}
