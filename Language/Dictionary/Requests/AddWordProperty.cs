using Language.Model;

namespace Language.Dictionary.Requests;

public class AddWordProperty
{
    public int BaseWordId { get; set; }
    public string BaseWord { get; set; }
    public string? Translation { get; set; }
    public string? PartOfSpeech { get; set; }
}