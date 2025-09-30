using System.ComponentModel.DataAnnotations.Schema;

namespace Language.Model;

public class Text
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int WordsCount { get; set; }
    public ICollection<Word> Dictionary { get; set; }
    
}