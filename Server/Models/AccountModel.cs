namespace Server.Models;

public class Account 
{
    public Guid Id { get; set; }

    public required string FirstName {get; set;}

    public required string LastName {get; set;}

    public required string Email {get; set;}

    public string Password {get; set;}

    public float Balance { get; set; }

    public bool Admin { get; set; }
 
    public DateTime Date {get; set;} 


    public Account() 
    {
        this.Id = Guid.NewGuid();
        this.Date = DateTime.Now;
    }

}
