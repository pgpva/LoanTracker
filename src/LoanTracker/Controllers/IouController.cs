using Microsoft.AspNetCore.Mvc;
using LoanTracker.Models;

[ApiController]
[Route("iou")]
public class IouController : ControllerBase
{
    private readonly UserService _userService;

    public IouController(UserService userService)
    {
        // Инициализация сервиса пользователей
        _userService = userService;
    }

    // Создание нового IOU (долга)
    [HttpPost]
    public IActionResult CreateIou([FromBody] IouRequest request)
    {
        _userService.CreateIou(request.Lender, request.Borrower, request.Amount);
        var lender = _userService.GetUser(request.Lender);
        var borrower = _userService.GetUser(request.Borrower);

        // Получение отсортированных данных
        var updatedUsers = new
        {
            Lender = new
            {
                Name = lender.Name,
                Owes = lender.GetSortedOwes(),
                OwedBy = lender.GetSortedOwedBy(),
                Balance = lender.Balance
            },
            Borrower = new
            {
                Name = borrower.Name,
                Owes = borrower.GetSortedOwes(),
                OwedBy = borrower.GetSortedOwedBy(),
                Balance = borrower.Balance
            },
        };
        return Ok(new { users = updatedUsers });
    }
}

// Запрос для создания IOU
public class IouRequest
{
    public string Lender { get; set; }
    public string Borrower { get; set; }
    public double Amount { get; set; }
}
