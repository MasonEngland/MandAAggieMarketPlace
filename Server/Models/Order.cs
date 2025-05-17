namespace Server.Models;

public class Order
{
    public Guid Id { get; set; }

    public required Item OrderItem { get; set; }

    public required string OwnerId { get; set; }

    public required string Adress { get; set; } 
    
    public DateTime Date { get; set; }

    public Order() 
    {
        Id = Guid.NewGuid();
        Date = DateTime.Now;
    }
}   
