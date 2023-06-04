using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelsAndEnums.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Data;

public class UserService : IUserService
{
    private readonly WebApplicationDbContext _dbContext;
    private readonly string _jwtSecret;

    public UserService(WebApplicationDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _jwtSecret = configuration["JwtSettings:SecretKey"];
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
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}