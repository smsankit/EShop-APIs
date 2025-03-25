using System.ComponentModel.DataAnnotations;

namespace Products.Models
{
    public class Product
    {
        [Key]
        public string Id { get; set; }=Guid.NewGuid().ToString();
        //public string ProductId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Rating Rating { get; set; } = new Rating();

    }

    public class Rating
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Rate { get; set; } = string.Empty;
        public string Count { get; set; } = string.Empty;
    }

}