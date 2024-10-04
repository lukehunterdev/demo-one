namespace LhDev.DemoOne.Models;

public class NewCustomer
{
    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public override string ToString() => $"{FirstName} {Surname}";
}

public class Customer : NewCustomer
{
    public int Id { get; set; }
}