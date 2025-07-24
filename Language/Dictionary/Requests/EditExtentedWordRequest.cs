namespace Language.Dictionary.Requests;

public class EditExtentedWordRequest
{
    public int Id { get; set; }
    public string Word { get; set; }
    public int BaseWordId { get; set; }
}