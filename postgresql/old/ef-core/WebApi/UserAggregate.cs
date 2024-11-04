namespace WebApi;




public record Address(string City, string Street, string Home);

public class UserAggregate
{
    public int Id { get; protected set; }
    public Address Address { get; protected set; }
    public int RowVersion { get; protected set; }

    protected UserAggregate()
    {

    }

    public UserAggregate(Address address)
    {
        Address = address;
    }

    public void ChangeAddress(Address address)
    {
        Address = address;
        RowVersion++;
    }

}