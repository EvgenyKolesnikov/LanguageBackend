using Language.Dictionary.Responses.Translate;
using Language.Model;

namespace Language.Dictionary.Responses;

public class WordDto
{
    public int Id { get; set; }
    public string WordText { get; set; }
    public string? Translation { get; set; }
    public int? ParentWordId { get; set; }
    
    public List<WordDto> ChildrenWords { get; set; } = new();
    public List<WordPropertiesDto> Properties { get; set; } = new();
    public WordDto(){}
    public WordDto(Word word)
    {
        Id = word.Id;
        WordText = word.WordText;
        ParentWordId = word.ParentWordId;
        ChildrenWords = word.ChildrenWords.Select(w => new WordDto(w, true)).ToList();
        Translation = string.Join(", ", word.Properties?.Select(i => i.Translation) ?? Array.Empty<string?>());
        Properties = word.Properties.Select(i => new WordPropertiesDto(i)).ToList();
    }
    
    public WordDto(Word word, bool? isChildren)
    {
        Id = word.Id;
        WordText = word.WordText;
        ParentWordId = word.ParentWordId;
        ChildrenWords = word.ChildrenWords.Select(w => new WordDto(w)).ToList();
        Translation = word.Translation;
        Properties = word.Properties.Select(i => new WordPropertiesDto(i)).ToList();
    }
}