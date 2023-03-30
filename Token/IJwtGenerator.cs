using NetKubernetes.Models;

namespace NetKubernetes.Token;

public interface IJwtGenerator
{
    string CreateToken(User user);
}