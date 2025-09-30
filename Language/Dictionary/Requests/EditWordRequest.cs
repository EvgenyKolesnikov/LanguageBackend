namespace Language.Dictionary.Requests;

public class EditWordRequest
{
    public int Id { get; set; }
    public string WordText { get; set; }
    public string? ParentWord { get; set; }
    public int? ParentWordId { get; set; }
    public string? Translation { get; set; }
}