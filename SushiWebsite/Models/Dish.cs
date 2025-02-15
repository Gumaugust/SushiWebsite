namespace SushiWebsite.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public decimal Price { get; set; }  
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; } // Danh mục
                                           
        public List<PriceOption>? PriceOptions { get; set; }
    }
    public class PriceOption
    {
        public string Size { get; set; } // Ví dụ: "0.2l", "0.4l"
        public decimal Price { get; set; }
    }
    public class CartItem
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
        public string? Note { get; set; }
    }
}
