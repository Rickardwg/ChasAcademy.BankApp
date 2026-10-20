namespace BankApp.Models;

public class User
{
    public bool IsLocked { get; set; } // Based on number of failed log in attempts
    public int FailedLoginCounter { get; set; }
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
}
