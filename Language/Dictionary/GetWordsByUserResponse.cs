using Language.Model;

namespace Language.Dictionary;

public class GetWordsByUserResponse
{
    public List<Word> Words { get; set; }

    public GetWordsByUserResponse(List<Model.Word> words)
    {
        Words = words;
    }
}