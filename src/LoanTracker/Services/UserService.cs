using LoanTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class UserService
{
    private readonly LoanTrackerContext _context;

    /// <summary>
    /// Конструктор сервиса.
    /// </summary>
    public UserService(LoanTrackerContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получает всех пользователей из базы данных.
    /// </summary>
    /// <returns>Список всех пользователей.</returns>
    public List<User> GetAllUsers() => _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).ToList();

    /// <summary>
    /// Получает пользователя по имени.
    /// </summary>
    /// <param name="name">Имя пользователя.</param>
    /// <returns>Пользователь с указанным именем или null, если не найден.</returns>
    public User GetUser(string name) => _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).FirstOrDefault(u => u.Name == name);

    /// <summary>
    /// Создает нового пользователя.
    /// </summary>
    /// <param name="name">Имя нового пользователя.</param>
    /// <returns>Созданный пользователь.</returns>
    public User CreateUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("User name cannot be empty or whitespace.", nameof(name));
        }
        if (_context.Users.Any(u => u.Name.ToLower() == name.ToLower()))
        {
            throw new InvalidOperationException("User with this name already exists.");
        }

        var user = new User { Name = name };
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    /// <summary>
    /// Обновление информации о пользователе.
    /// </summary>
    /// <param name="user">Пользователь с обновленными данными.</param>
    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    /// <summary>
    /// Удаление пользователя и всех связанных с ним долгов.
    /// </summary>
    /// <param name="user">Пользователь, которого нужно удалить.</param>
    public void DeleteUser(User user)
    {
        var ious = _context.Ious.Where(i => i.LenderId == user.Id || i.BorrowerId == user.Id).ToList();
        _context.Ious.RemoveRange(ious);

        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    /// <summary>
    /// Создает новый долг (IOU) между кредитором и заемщиком.
    /// </summary>
    /// <param name="lenderName">Имя кредитора.</param>
    /// <param name="borrowerName">Имя заемщика.</param>
    /// <param name="amount">Сумма долга.</param>
    /// <returns>Кредитор и заемщик.</returns>
    public (User lender, User borrower) CreateIou(string lenderName, string borrowerName, double amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount should be greater than zero.");
        }

        var lender = _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).FirstOrDefault(u => u.Name == lenderName);
        var borrower = _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).FirstOrDefault(u => u.Name == borrowerName);

        if (lender == null || borrower == null)
        {
            throw new KeyNotFoundException("Lender or borrower not found.");
        }

        var existingIou = lender.Owes.FirstOrDefault(i => i.Borrower == borrower);
        if (existingIou != null)
        {
            existingIou.Amount += amount;
            _context.SaveChanges();
            return (lender, borrower);
        }

        var reverseDebt = lender.OwedBy.FirstOrDefault(i => i.Lender == borrower);
        if (reverseDebt != null)
        {
            double reverseAmount = reverseDebt.Amount;
            if (reverseAmount > amount)
            {
                reverseDebt.Amount -= amount;
                _context.SaveChanges();
                return (lender, borrower);
            }
            else if (reverseAmount < amount)
            {
                amount -= reverseAmount;
                _context.Ious.Remove(reverseDebt);
                _context.SaveChanges();
            }
            else
            {
                _context.Ious.Remove(reverseDebt);
                _context.SaveChanges();
                return (lender, borrower);
            }
        }

        var iou = new Iou
        {
            Lender = lender,
            Borrower = borrower,
            Amount = amount
        };

        lender.Owes.Add(iou);
        borrower.OwedBy.Add(iou);

        _context.SaveChanges();

        return (lender, borrower);
    }
}
