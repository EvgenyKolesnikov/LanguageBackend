namespace Language.Model;

public class Dictionary
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string Translation { get; set; }
    public List<User> Users { get; set; }
}