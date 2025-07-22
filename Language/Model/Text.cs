namespace Language.Model;

public class Text
{
    public int Id { get; set; }
    public string Content { get; set; }
    public List<BaseWord> Dictionary { get; set; }
}