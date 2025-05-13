namespace Server.Models;

public class Item 
{

    public required string Name { get; set; }

    public required int Stock { get; set; } 

    public required string Description { get; set; }

    public Guid Id { get; set; }

    public required float Price { get; set; }

    public DateTime Date { get; set; }

    public required string ImageLink { get; set; }

    public required string Category { get; set; }

    public Item() 
    {
        Date = DateTime.Now;
        Id = Guid.NewGuid();
    }


}
