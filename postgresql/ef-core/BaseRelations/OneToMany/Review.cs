namespace BaseRelations.OneToMany;

public class Review
{
    public int Id { get; protected set; }
    public string Content { get; private set; }
    public int BookId { get; private set; }
    public Book Book { get; private set; }

    protected Review()
    {
        
    }
    
    public Review(Book book, string content)
    {
        Book = book;
        BookId = book.Id;
        Content = content;
    }
}