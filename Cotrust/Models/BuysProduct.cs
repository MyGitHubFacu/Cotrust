using System.ComponentModel.DataAnnotations.Schema;

namespace Cotrust.Models
{
    public class BuysProduct
    {
        public int Id { get; set; }
        public int BuysId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
