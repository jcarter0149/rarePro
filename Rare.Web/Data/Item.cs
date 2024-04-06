namespace Rare.Web.Data
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<OrderItem> Order { get; set; } = new List<OrderItem>();
    }
}
