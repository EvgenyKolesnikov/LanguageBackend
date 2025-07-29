namespace Language.Dictionary.Requests;

public class EditBaseWordRequest
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }
}