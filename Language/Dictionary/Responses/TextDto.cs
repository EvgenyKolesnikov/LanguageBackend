using Language.Model;

namespace Language.Dictionary.Responses;

public class TextDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public IEnumerable<BaseWordDto> Dictionary { get; set; }

    public TextDto(Text text)
    {
        Id = text.Id;
        Content = text.Content;
        Dictionary = text.Dictionary.Select(i => new BaseWordDto(i)).ToList();
    }
}
