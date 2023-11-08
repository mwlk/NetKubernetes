using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NetKubernetes.Models;
using NetKubernetes.Token.Interfaces;

namespace NetKubernetes.Token.Classes;

public class JwtGenerator : IJwtGenerator
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.UserName!),
            // new Claim("userId", user.Id!)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("My_s3cr3t_p455w03d_f0r_g3n3r4t3_jwt_1234$_p455w03d_3ncrypt3d_$_netcoreapi"));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }
}


