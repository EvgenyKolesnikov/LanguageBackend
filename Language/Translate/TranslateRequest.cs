namespace Language.Translate;

public class TranslateRequest
{
    public string ClickedWord { get; set; }
    public string PreviousWord { get; set; }
    public string NextWord { get; set; }
    public string Sentence { get; set; }
}