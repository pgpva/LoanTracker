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

        /// <summary>
        /// Получает всех пользователей.
        /// </summary>
        /// <returns>Список пользователей.</returns>
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

        /// <summary>
        /// Получает информацию о пользователе по имени.
        /// </summary>
        /// <param name="name">Имя пользователя.</param>
        /// <returns>Информация о пользователе.</returns>
        [HttpGet("{name}")]
        public IActionResult GetUser(string name)
        {
            var user = _userService.GetUser(name);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

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

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        /// <param name="request">Запрос с данными для создания пользователя.</param>
        /// <returns>Созданный пользователь.</returns>
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

        /// <summary>
        /// Обновление имени существующего пользователя.
        /// </summary>
        /// <param name="name">Имя пользователя, которое нужно обновить.</param>
        /// <param name="request">Новая информация для пользователя.</param>
        /// <returns>Статус обновления.</returns>
        [HttpPut("{name}")]
        public IActionResult UpdateUser(string name, [FromBody] UpdateUserRequest request)
        {
            var user = _userService.GetUser(name);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            if (string.IsNullOrWhiteSpace(request.NewName))
            {
                return BadRequest(new { Message = "New name cannot be empty or whitespace." });
            }

            user.Name = request.NewName;
            _userService.UpdateUser(user);
            return Ok(new { Message = "User updated successfully." });
        }

        [HttpDelete("{name}")]
        public IActionResult DeleteUser(string name)
        {
            var user = _userService.GetUser(name);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            _userService.DeleteUser(user);
            return Ok(new { Message = "User deleted successfully." });
        }
    }
}

public class CreateUserRequest
{
    public string User { get; set; }
}


public class UpdateUserRequest
{
    public string NewName { get; set; }
}
