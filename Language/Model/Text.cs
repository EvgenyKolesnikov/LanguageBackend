namespace Language.Model;

public class Text
{
    public int Id { get; set; }
    public string Content { get; set; }
    public IEnumerable<BaseWord> Dictionary { get; set; } 
}