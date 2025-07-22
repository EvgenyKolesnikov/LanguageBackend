namespace Language.Model;

public class BaseWord
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }
    public List<User> Users { get; set; }
    
    public IEnumerable<ExtentedWord>? ExtentedWords { get; set; }
    public IEnumerable<Text>? TextsId { get; set; }
}