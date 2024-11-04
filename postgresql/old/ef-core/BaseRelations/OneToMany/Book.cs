using System.Collections.Generic;
using BaseRelations.ManyToMany;

namespace BaseRelations.OneToMany;

public class Book
{
    public int Id { get; protected set; }
    public string Title { get; private set; }
    public ICollection<Review> Reviews { get; private set; }
    public ICollection<UserBooks> UserBooks { get; set; }

    protected Book()
    {
        
    }
    public Book(string title)
    {
        Title = title;
    }
}