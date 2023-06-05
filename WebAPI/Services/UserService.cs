using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModelsAndEnums.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly WebApplicationDbContext _dbContext;
    private readonly AppSettings _appSettings;

    public UserService(WebApplicationDbContext dbContext, IConfiguration configuration, IOptions<AppSettings> appSettings)
    {
        _dbContext = dbContext;
        _appSettings = appSettings.Value;
    }

    public async Task<bool> Register(User user)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null)
            return false;

        user.SetPassword(user.Password);
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !user.VerifyPassword(password))
            return null;

        var token = GenerateJwtToken(user);
        return token;
    }

    private string GenerateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)}),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public User GetById(int id)
    {
        return _dbContext.Users.FirstOrDefault(x => x.UserId == id);
    }
}