namespace Language.Dictionary.Responses.Translate;

public class YandexTranslateResponse
{
    public List<YandexTranslations> translations { get; set; }
}

public class YandexTranslations
{
    public string text { get; set; }
    public string detectedLanguageCode { get; set; }
}