using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Service;

public class JwtService
{
    private readonly JwtOptions _options;
    
    private const string SignatureAlgorithm = SecurityAlgorithms.HmacSha512;
    private readonly string _secret = Environment.GetEnvironmentVariable("JwtSecret")!;

    public JwtService(JwtOptions options)
    {
        _options = options;
    }

    public string IssueToken(SessionData data)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var token = jwtHandler.CreateEncodedJwt(new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Convert.FromBase64String(_secret)),
                SignatureAlgorithm
            ),
            Issuer = _options.Address,
            Audience = _options.Address,
            Expires = DateTime.UtcNow.Add(_options.Lifetime),
            Claims = data.ToDictionary()
        });
        return token;
    }

    public SessionData ValidateAndDecodeToken(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var principal = jwtHandler.ValidateToken(token, new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secret)),
            ValidAlgorithms = new[] { SignatureAlgorithm },

            // Default value is true already.
            // They are just set here to emphasise the importance.
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,

            ValidAudience = _options.Address,
            ValidIssuer = _options.Address,

            // Set to 0 when validating on the same system that created the token
            ClockSkew = TimeSpan.FromSeconds(0)
        }, out var securityToken);
        return SessionData.FromDictionary(new JwtPayload(principal.Claims));
    }
}