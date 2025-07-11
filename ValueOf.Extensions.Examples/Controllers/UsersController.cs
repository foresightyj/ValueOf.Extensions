using Microsoft.AspNetCore.Mvc;
using ValueOf.Extensions.Examples.Models;

namespace ValueOf.Extensions.Examples.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetUsers")]
    public IList<User> Get()
    {
        return Enumerable.Range(1, 5)
            .Select(idx => new User(UserId.From(idx), EmailAddress.From($"user{idx}@gmail.com")))
            .ToArray();
    }
}