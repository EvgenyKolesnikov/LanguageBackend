using Language.Model;

namespace Language.Dictionary.Responses;

public class TextDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int WordsCount { get; set; }
    public int WordsProcessed => Dictionary != null ? Dictionary.Count() : 0;
    public IEnumerable<WordDto> Dictionary { get; set; }

    public TextDto (){}
    public TextDto(Text text)
    {
        Id = text.Id;
        WordsCount = text.WordsCount;
        Content = text.Content;
        Dictionary = text.Dictionary.Select(i => new WordDto(i)).ToList();
    }
}
