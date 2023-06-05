using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsAndEnums.Enums;
using WebAPI.RequestModel;
using WebAPI.RequestModels;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
    {
        var user = new ModelsAndEnums.Models.User();
        user.Username = model.Username;
        user.Password = model.Password;
        user.Role = UserRole.RegularUser;

        var result = await _userService.Register(user);
        if (result)
            return Ok();

        return BadRequest("Username is already taken.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        var token = await _userService.Login(model.Username, model.Password);
        if (token == null)
            return Unauthorized();

        return Ok(new { Token = token });
    }

    // Protected route example
    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok();
    }
}