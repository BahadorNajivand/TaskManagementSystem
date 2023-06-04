using Microsoft.AspNetCore.Mvc;
using ModelsAndEnums.Models;
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
        User user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

        if (user != null && user.VerifyPassword(model.Password))
        {
            // Store the authenticated user details in session
            _httpContextAccessor.HttpContext.Session.SetInt32("UserId", user.UserId);
            _httpContextAccessor.HttpContext.Session.SetString("Username", user.Username);

            // Redirect to the home page or any other desired page
            return RedirectToAction("Index", "Tasks");
        }

        // Invalid credentials, display error message
        ModelState.AddModelError("Authentication", "Invalid username or password");
        return View();
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
