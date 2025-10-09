namespace Language.Model;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
    public List<Word>  Dictionary { get; set; }
    public List<Book> Books { get; set; }
}