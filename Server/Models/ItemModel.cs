namespace Server.Models;

public class Item 
{

    public string Name { get; set; }

    public int Stock { get; set; } 

    public Guid Id { get; set; }

    public float Price { get; set; }

    public DateTime Date { get; set; }

    public string ImageLink { get; set; }

    public Item() 
    {
        Date = DateTime.Now;
        Id = Guid.NewGuid();
    }


}
