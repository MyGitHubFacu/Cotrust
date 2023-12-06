namespace Cotrust.Models
{
    public class Package
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<PackageProduct>? Products { get; set; }
    }
}
