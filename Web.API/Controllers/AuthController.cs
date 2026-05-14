using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Web.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential.Username == "admin" && credential.Password == "admin")
        {
            //Crating the security context
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, credential.Username),
                    new Claim(ClaimTypes.Email,"admin@thisNewSite"),
                    new Claim("Deparment", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager","true"),
                    new Claim("EmploymentDate", "2025-04-01")
                };

           var expiresAt = DateTime.UtcNow.AddMinutes(30);

            
            return Ok(new { access_token = CreateToken(claims, expiresAt), expires_at = expiresAt });
        }

        ModelState.AddModelError("Unauthorized", "You are not authorize to access the endpoint");
        var problemDetails = new ValidationProblemDetails(ModelState)
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized"
        };

        return Unauthorized(problemDetails);
    }

    private string CreateToken(List<Claim> claims, DateTime expiresAt)
    {
        var claimsDic = new Dictionary<string, object>();
        if(claims is not null && claims.Count > 0)
        {
            foreach(var claim in claims)
            {
                claimsDic.Add(claim.Type, claim.Value);
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims= claimsDic,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]??string.Empty)), 
                                                        SecurityAlgorithms.HmacSha256Signature),
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);


    }
}

public class Credential
{
    public string Username { get; set; }= string.Empty;
    public string Password { get; set; } = string.Empty;
}
