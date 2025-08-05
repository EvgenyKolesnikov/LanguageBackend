using Language.Model;

namespace Language.Dictionary;

public class GetWordsByUserResponse
{
    public List<BaseWord> Words { get; set; }

    public GetWordsByUserResponse(List<Model.BaseWord> words)
    {
        Words = words;
    }
}