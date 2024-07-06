namespace Server.Models;

public class Order
{
    public Guid Id { get; set; }

    public Item OrderItem { get; set; }

    public String OwnerId { get; set; }

    public String Adress { get; set; } 
    
    public DateTime Date { get; set; }

    public Order() 
    {
        Id = Guid.NewGuid();
        Date = DateTime.Now;
    }
}   
