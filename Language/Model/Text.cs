namespace Language.Model;

public class Text
{
    public int Id { get; set; }
    public string Content { get; set; }
    public List<Dictionary> Dictionary { get; set; }
}