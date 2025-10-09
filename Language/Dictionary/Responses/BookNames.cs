using Language.Model;

namespace Language.Dictionary.Responses;

public class BookNames
{
    public int Id { get; set; }
    public string Name { get; set; }


    public BookNames(int id, string name)
    {
        Id = id;
        Name = name;
    }
}