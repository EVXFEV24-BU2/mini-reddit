/*

# Skapa användare
POST /user/register

# Logga in
POST /login

*/

using Microsoft.AspNetCore.Mvc;
namespace MiniReddit;
[ApiController]
[Route("/user")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<UserController> logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await userService.CreateUser(request);
            logger.LogInformation("Created user with id '{}'", user.Id);
            return CreatedAtAction(nameof(CreateUser), user.Id);
        }
        catch (IdentityCoreException exception)
        {
            return BadRequest(exception.Errors);
        }
        catch (Exception exception)
        {
            logger.LogError("Unexpected error when creating user: {}", exception.Message);
            return StatusCode(500, "Unexpected error.");
        }
    }
}

public class CreateUserRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}
