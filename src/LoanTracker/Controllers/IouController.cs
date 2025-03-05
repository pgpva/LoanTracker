using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoanTracker.Models;

[ApiController]
[Route("iou")]
[Authorize]
public class IouController : ControllerBase
{
    private readonly UserService _userService;

    public IouController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Создает новый долг (IOU) между кредитором и заемщиком.
    /// </summary>
    /// <param name="request">Запрос с данными долга.</param>
    /// <returns>Обновленные данные пользователей.</returns>
    [HttpPost]
    public IActionResult CreateIou([FromBody] IouRequest request)
    {
        var (lender, borrower) = _userService.CreateIou(request.Lender, request.Borrower, request.Amount);

        var updatedUsers = new
        {
            Lender = new
            {
                Name = lender.Name,
                Owes = lender.Owes?.OrderBy(i => i.Borrower.Name).ToDictionary(i => i.Borrower.Name, i => i.Amount) ?? new Dictionary<string, double>(),
                OwedBy = lender.OwedBy?.OrderBy(i => i.Lender.Name).ToDictionary(i => i.Lender.Name, i => i.Amount) ?? new Dictionary<string, double>(),
                Balance = lender.Balance
            },
            Borrower = new
            {
                Name = borrower.Name,
                Owes = borrower.Owes?.OrderBy(i => i.Borrower.Name).ToDictionary(i => i.Borrower.Name, i => i.Amount) ?? new Dictionary<string, double>(),
                OwedBy = borrower.OwedBy?.OrderBy(i => i.Lender.Name).ToDictionary(i => i.Lender.Name, i => i.Amount) ?? new Dictionary<string, double>(),
                Balance = borrower.Balance
            },
        };

        return Ok(new { users = updatedUsers });
    }
}

public class IouRequest
{
    public string Lender { get; set; }
    public string Borrower { get; set; }
    public double Amount { get; set; }
}
