using NetKubernetes.Models;

namespace NetKubernetes.Token.Interfaces;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}

