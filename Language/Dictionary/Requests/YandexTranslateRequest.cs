namespace Language.Dictionary.Requests;

public class YandexTranslateRequest
{
    public string folderId { get; set; }
    public string texts { get; set; }
    public string targetLanguageCode { get; set; }
}