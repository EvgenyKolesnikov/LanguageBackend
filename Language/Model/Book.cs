namespace Language.Model;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public List<User> Users { get; set; }
}