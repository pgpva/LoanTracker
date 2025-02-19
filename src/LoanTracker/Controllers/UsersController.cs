using Microsoft.AspNetCore.Mvc;
using LoanTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace LoanTracker.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            // Инициализация сервиса пользователей
            _userService = userService;
        }

        // Получение списка всех пользователей
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(new { users });
        }

        // Получение информации о пользователе по имени
        [HttpGet("{name}")]
        public IActionResult GetUser(string name)
        {
            var user = _userService.GetUser(name);
            if (user == null)
            {
                // Обработка случая, когда пользователь не найден
                return NotFound(new { Message = "User not found." });
            }

            // Получение отсортированных данных
            var sortedOwes = user.GetSortedOwes();
            var sortedOwedBy = user.GetSortedOwedBy();
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

        // Создание нового пользователя
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
                // Обработка конфликта
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}

// Запрос для создания пользователя
public class CreateUserRequest
{
    public string User { get; set; }
}
