using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NetKubernetes.Token.Interfaces;

namespace NetKubernetes.Token.Classes;

public class UserSession : IUserSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSession(IHttpContextAccessor httpContextAccesor)
    {
        _httpContextAccessor = httpContextAccesor;
    }

    public string GetUserSession()
    {
        var username = _httpContextAccessor.HttpContext!.User?.Claims?
            .FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?
            .Value;

        return username!;
    }
}