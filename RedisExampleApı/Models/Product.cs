namespace RedisExampleApı.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; } = "unknown";
        public decimal Price{ get; set; }
    }
} 
