using System.Text.Json.Serialization;

namespace Language.Model;

public class Word
{
    public int Id { get; set; }
    public string WordText { get; set; }
    public string? Translation { get; set; }
    public Word? ParentWord { get; set; }
    public int? ParentWordId { get; set; }
    
    public List<Word> ChildrenWords { get; set; } = new();
    public List<WordProperties> Properties { get; set; } = new();
    public List<User> Users { get; set; }
    public IEnumerable<Text>? Texts { get; set; }
}