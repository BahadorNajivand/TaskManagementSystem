
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModels;

public class RegisterController : Controller
{
    private readonly WebApplicationDbContext _context;

    public RegisterController(WebApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }   

    [HttpPost]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return View(model);
            }

            // Registration logic
            // You can create a new User object based on the register form inputs, set the appropriate properties, and save it to the database.

            var newUser = new User
            {
                Username = model.Username,
                Password = model.Password,
                Role = UserRole.RegularUser
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // You can also perform additional actions after successful registration, such as sending a confirmation email, logging the registration, etc.

            return RedirectToAction("Index", "Login");
        }

        return View(model);
    }
}