using Language.Dictionary.Responses.Translate;
using Language.Model;

namespace Language.Dictionary.Responses;

public class BaseWordDto
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }
    public List<WordPropertiesDto> Properties { get; set; } = new();
    public BaseWordDto(){}
    public BaseWordDto(BaseWord baseWord)
    {
        Id = baseWord.Id;
        Word = baseWord.Word;
        Translation = string.Join(", ", baseWord.Properties?.Select(i => i.Translation) ?? Array.Empty<string?>());
        Properties = baseWord.Properties.Select(i => new WordPropertiesDto(i)).ToList();
    }
}