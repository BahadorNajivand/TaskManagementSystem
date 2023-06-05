using Microsoft.AspNetCore.Identity;
using ModelsAndEnums.Models;

public interface IUserService
{
    Task<bool> Register(User user);
    Task<string> Login(string username, string password);
    User GetById(int id);
}
