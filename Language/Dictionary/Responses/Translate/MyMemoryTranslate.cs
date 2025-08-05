namespace Language.Dictionary.Responses.Translate;

public class MyMemoryTranslate
{
    public  ResponseData responseData { get; set; }
    public  List<Match> matches { get; set; }
}

public class ResponseData 
{
    public string translatedText { get; set; }
}

public class Match 
{
    public string translation { get; set; }
}