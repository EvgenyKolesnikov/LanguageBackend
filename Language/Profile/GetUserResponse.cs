using Language.Model;

namespace Language.Profile;

public class GetUserResponse
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public GetUserResponse(User? user)
    {
        Name = user?.Name;
        Email = user?.Email;
    }
}