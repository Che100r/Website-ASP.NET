namespace Website.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public Phone Phone { get; set; }
        public Basket Basket { get; set; }
    }
}