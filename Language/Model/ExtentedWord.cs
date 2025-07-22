namespace Language.Model;

public class ExtentedWord
{
    public int Id { get; set; }
    public string Word { get; set; }
    public BaseWord BaseWord { get; set; }
    public int BaseWordId { get; set; }
}