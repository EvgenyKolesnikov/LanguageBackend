using Language.Model;

namespace Language.Dictionary.Requests;

public class AddWordProperty
{
    public int WordId { get; set; }
    public string WordText { get; set; }
    public string? Translation { get; set; }
    public string? PartOfSpeech { get; set; }
}