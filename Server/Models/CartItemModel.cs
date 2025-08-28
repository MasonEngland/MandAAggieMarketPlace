namespace Server.Models;

public class CartItem
{
    public Guid Id { get; set; }

    public required Item OrderItem { get; set; }

    public Guid OrderItemId { get; set; }

    public required string OwnerId { get; set; }

    public required string Address { get; set; }

    public DateTime Date { get; set; }
    public int Amount { get; set; }

    public CartItem()
    {
        Id = Guid.NewGuid();
        Date = DateTime.Now;
        Amount = 1;
    }
}