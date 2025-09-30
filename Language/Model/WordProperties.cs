

using System.Text.Json.Serialization;

namespace Language.Model;

public class WordProperties
{
    public int Id { get; set; }
    public Word Word { get; set; }
    public string? Translation { get; set;}
    public string? PartOfSpeech { get; set;}
}