namespace Server.Models;

public class Item 
{

    public string Name { get; set; }

    public int Stock { get; set; }

    public string? Buyer { get; set; }

    public Guid Id { get; set; }

    public DateTime Date { get; set; }

    public Item() 
    {
        Date = DateTime.Now;
        Id = Guid.NewGuid();
    }


}
