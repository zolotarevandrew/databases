using BaseRelations.OneToMany;
using BaseRelations.OneToOne;

namespace BaseRelations.ManyToMany;

public class UserBooks
{
    public int Id { get; protected set;  }
    public int UserId { get; protected set; }
    public User User { get; protected set; }
    public int BookId { get; protected set; }
    public Book Book { get; protected set; }

    protected UserBooks()
    {
        
    }
    public UserBooks(User user, Book book)
    {
        User = user;
        Book = book;
    }
}