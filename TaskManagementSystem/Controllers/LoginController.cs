using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModels;


public class LoginController : Controller
{
    private readonly WebApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController(WebApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user != null)
            {
                // Authentication logic
                // Store the authenticated user's details in a session

                var session = _httpContextAccessor.HttpContext.Session;
                session.SetInt32("UserId", user.UserId);
                session.SetString("Username", user.Username);
                session.SetString("UserRole", user.Role.ToString());

                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Logout()
    {
        // Clear session and sign out the user
        _httpContextAccessor.HttpContext.Session.Clear();
        // Redirect to home page or any other desired page
        return RedirectToAction("Index", "Home");
    }
}
