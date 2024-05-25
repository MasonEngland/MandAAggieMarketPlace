namespace Server.Models;

public class Account 
{
    public Guid Id { get; set; }

    public string FirstName {get; set;}

    public string LastName {get; set;}

    public string Email {get; set;}

    public string Password {get; set;}

    public float Balance { get; set; }
 
    public DateTime Date {get; set;} 


    public Account() 
    {
        this.Id = Guid.NewGuid();
        this.Date = DateTime.Now;
    }

}
