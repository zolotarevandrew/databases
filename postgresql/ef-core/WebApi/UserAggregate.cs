namespace WebApi;




public record Address(string City, string Street, string Home);

public class UserAggregate
{
    public int Id { get; protected set; }
    public Address Address { get; protected set; }

    protected UserAggregate()
    {
        
    }

    public UserAggregate(Address address)
    {
        Address = address;
    }
}