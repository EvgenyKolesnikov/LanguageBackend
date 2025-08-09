using Language.Dictionary.Responses.Translate;
using Language.Model;

namespace Language.Dictionary.Responses;

public class BaseWordDto
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }
    public List<WordPropertiesDto> Properties { get; set; }
    public BaseWordDto(){}
    public BaseWordDto(BaseWord baseWord)
    {
        Id = baseWord.Id;
        Word = baseWord.Word;
        Translation = baseWord.Translation;
        Properties = baseWord.Properties.Select(i => new WordPropertiesDto(i)).ToList();
    }
}