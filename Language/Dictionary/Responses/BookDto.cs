using Language.Model;

namespace Language.Dictionary.Responses;

public class BookDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public List<User> Users { get; set; }


    public BookDto(Book book)
    {
        Id = book.Id;
        Name = book.Name;
        Content = book.Content;
        Users = book.Users;
    }
}