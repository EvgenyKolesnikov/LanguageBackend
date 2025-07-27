using Language.Model;

namespace Language.Dictionary.Responses;

public class BaseWordDto
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string? Translation { get; set; }

    public BaseWordDto(){}
    public BaseWordDto(BaseWord baseWord)
    {
        Id = baseWord.Id;
        Word = baseWord.Word;
        Translation = baseWord.Translation;
    }
}