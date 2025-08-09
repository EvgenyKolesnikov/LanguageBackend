using Language.Model;

namespace Language.Dictionary.Responses.Translate;

public class WordPropertiesDto
{
    public int Id { get; set; }
    public string? Translation { get; set;}
    public string? PartOfSpeech { get; set;}
    
    public WordPropertiesDto(){}

    public WordPropertiesDto(WordProperties wordProperties)
    {
        Id = wordProperties.Id;
        Translation = wordProperties.Translation;
        PartOfSpeech = wordProperties.PartOfSpeech;
    }
}