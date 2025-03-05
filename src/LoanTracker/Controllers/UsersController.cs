using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanTracker.Models;
using System.Linq;

namespace LoanTracker.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAllUsers();
            var usersDto = users.Select(u => new UserDto
                    {
                    Name = u.Name,
                    Balance = u.Balance,
                    Owes = u.Owes.OrderBy(i => i.Borrower.Name).ToDictionary(i => i.Borrower.Name, i => i.Amount),
                    OwedBy = u.OwedBy.OrderBy(i => i.Lender.Name).ToDictionary(i => i.Lender.Name, i => i.Amount)
                    }).ToList();

            return Ok(new { users = usersDto });
        }

        [HttpGet("{name}")]
        public IActionResult GetUser(string name)
        {
            var user = _userService.GetUser(name);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            // Здесь уже используется база данных для получения задолженностей
            var sortedOwes = user.Owes.OrderBy(i => i.Borrower.Name)
                                      .ToDictionary(i => i.Borrower.Name, i => i.Amount);
            var sortedOwedBy = user.OwedBy.OrderBy(i => i.Lender.Name)
                                           .ToDictionary(i => i.Lender.Name, i => i.Amount);

            return Ok(new
            {
                user = new
                {
                    Name = user.Name,
                    Owes = sortedOwes,
                    OwedBy = sortedOwedBy,
                    Balance = user.Balance
                }
            });
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = _userService.CreateUser(request.User);
                return CreatedAtAction(nameof(GetUser), new { name = user.Name }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}

public class CreateUserRequest
{
    public string User { get; set; }
}
