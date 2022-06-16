using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthTest.Manager
{
    public class JwtAuthenticationManager
    {
        //key declaration
        private readonly IConfiguration _configuration;

        private readonly IDictionary<string, string> users = new Dictionary<string, string>
        { {"test", "password"}, {"test1", "password1"}, {"user", "assword"} };

        public JwtAuthenticationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Authenticate(string username, string password)
        {
            //auth failed - creds incorrect
            if (!users.Any(u => u.Key == username && u.Value == password))
            {
                return null;
            }
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Token"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                // Duration of the Token
                // Now the the Duration to 1 Hour
                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature) //setting sha256 algorithm
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
