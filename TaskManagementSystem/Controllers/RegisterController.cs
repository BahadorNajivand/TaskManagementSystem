using Microsoft.AspNetCore.Mvc;
using ModelsAndEnums.Models;
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
        if (!ModelState.IsValid) {
            return View();
        }
        // Check if the username is already taken
        if (_context.Users.Any(u => u.Username == model.Username))
        {
            ModelState.AddModelError("Username", "Username is already taken");
            return View();
        }

        User user = new User();

        user.Username = model.Username;
        // Set the user's password
        user.SetPassword(model.Password);

        // Save the new user to the database
        _context.Users.Add(user);
        _context.SaveChanges();

        // Redirect to the login page or any other desired page
        return RedirectToAction("Index", "Login");
    }
}