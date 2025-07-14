using Language.Model;

namespace Language.Responses;

public class LoginResponse
{
    public string Token { get; set; }

    public LoginResponse(string token)
    {
        Token = token;
    }
}