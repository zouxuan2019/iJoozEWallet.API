using System.IdentityModel.Tokens.Jwt;

namespace iJoozEWallet.API.Services
{
    public class TokenService
    {
        public string GetUserIdFromToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Subject;
        }
    }
}