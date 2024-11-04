namespace BaseRelations.OneToOne;

public class Address
{
    public int UserId { get; protected set; }
    public User User { get; protected set; }
    public string Street { get; protected set; }

    protected Address()
    {
        
    }

    public Address(User user, string street)
    {
        User = user;
        Street = street;
    }
}