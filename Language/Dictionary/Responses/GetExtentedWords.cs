using Language.Model;

namespace Language.Dictionary.Responses;

public class GetExtentedWords
{
    public int Id { get; set; }
    public string Word { get; set; }
    public int BaseWordId { get; set; }
    public string? Translation { get; set; }

    public GetExtentedWords(){}
    
    public GetExtentedWords(ExtentedWord extendedWord)
    {
        Id = extendedWord.Id;
        Word = extendedWord.Word;
        BaseWordId = extendedWord.BaseWordId;
        Translation = extendedWord.Translation;
    }
}

