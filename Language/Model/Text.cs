namespace Language.Model;

public class Text
{
    public int Id { get; set; }
    public string Content { get; set; }
    public ICollection<BaseWord> Dictionary { get; set; } 
}