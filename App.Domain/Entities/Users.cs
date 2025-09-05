namespace App.Domain.Entities;

public class Users : BaseEntity
{
    public Users(string email, string firstName, string lastName, string password, int role, int status)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        Role = role;
        Status = status;
    }
    public Users()
    {
    }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Password { get; private set; }
    public int Role { get; private set; }
    public int Status { get; private set; }
    public DateTimeOffset? LastLoggedinDate { get; private set; }
    public string FullName => $"{FirstName} {LastName}";
}