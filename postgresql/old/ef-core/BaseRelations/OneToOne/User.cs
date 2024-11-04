using System.Collections.Generic;
using BaseRelations.ManyToMany;

namespace BaseRelations.OneToOne;

public class User
{
    public int Id { get; protected set; }
    public string UserName { get; private set; }
    public Address? Address { get; private set; }
    public ICollection<UserBooks> UserBooks { get; set; }
    
    protected User()
    {
        
    }

    public User(string userName)
    {
        UserName = userName;
    }
}