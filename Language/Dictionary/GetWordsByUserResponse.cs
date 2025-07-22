namespace Language.Dictionary;

public class GetWordsByUserResponse
{
    public Dictionary<string, string> Words { get; set; }

    public GetWordsByUserResponse(List<Model.BaseWord> words)
    {
        Words = words.ToDictionary(i => i.Word, i => i.Translation);
    }
}