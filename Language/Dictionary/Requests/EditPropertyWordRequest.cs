namespace Language.Dictionary.Requests;

public class EditPropertyWordRequest
{
    public int BaseWordId { get; set; }
    public int PropertyWordId { get; set; }
    public string? Translation { get; set; }
    public string? PartOfSpeech { get; set; }
}