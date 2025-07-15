using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ValueOf.Extensions.Examples.Database;
using ValueOf.Extensions.Examples.Models;

namespace ValueOf.Extensions.Examples.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly DemoDbContext _dbContext;

    public UsersController(ILogger<UsersController> logger, DemoDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet("", Name = nameof(ListUsers))]
    public async Task<IList<UserViewModel>> ListUsers()
    {
        var users = await _dbContext.Users.ToListAsync();
        return users.Select(u => new UserViewModel(u.Id, u.Email)).ToArray();
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(UserViewModel), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById([FromRoute] UserId id)
    {
        _logger.LogInformation($"Getting user with id {id}");
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserViewModel(user.Id, user.Email));
    }

    [HttpGet("findByEmail", Name = nameof(FindByEmail))]
    [ProducesResponseType(typeof(UserViewModel), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> FindByEmail([FromQuery] EmailAddress email)
    {
        using (var conn = _dbContext.Database.GetDbConnection())
        {
            var first = await conn.QueryFirstAsync<UserViewModel>(
                "SELECT u.id, u.email FROM users u where u.email = @email",
                new { email });
            if (first != null)
            {
                return Ok(first);
            }

            return NotFound();
        }
    }
}