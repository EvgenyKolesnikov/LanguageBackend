using System.Text.Json.Serialization;

namespace Language.Model;

public class BaseWord
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }
    public List<User> Users { get; set; }
    
    public IEnumerable<ExtentedWord>? ExtentedWords { get; set; }
    public IEnumerable<Text>? Texts { get; set; }
}