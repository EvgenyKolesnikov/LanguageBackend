using Language.Model;

namespace Language.Dictionary.Responses;

public class GetWordsByTextResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public List<BaseWordDto> Words { get; set; } = new();
    
    public GetWordsByTextResponse(){}

    public GetWordsByTextResponse(Text text)
    {
        Id = text.Id;
        Content = text.Content;
        Words = text.Dictionary.Select(i => new BaseWordDto(i)).ToList();
    }
   
}