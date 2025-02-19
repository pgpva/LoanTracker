using System.Collections.Generic;
using System.Linq;
using LoanTracker.Models;

public class UserService
{
    // Список пользователей
    private readonly List<User> _users = new();

    // Получение всех пользователей
    public List<User> GetAllUsers() => _users;

    // Получение пользователя по имени
    public User GetUser(string name) => _users.SingleOrDefault(u => u.Name == name);

    // Создание нового пользователя
    public User CreateUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Проверка на пустое имя
            throw new ArgumentException("User name cannot be empty or whitespace.", nameof(name));
        }
        if (_users.Any(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            // Проверка на существование пользователя
            throw new InvalidOperationException("User with this name already exists.");
        }
        var user = new User { Name = name };
        _users.Add(user);
        return user;
    }

    // Создание IOU (долга)
    public (User lender, User borrower) CreateIou(string lenderName, string borrowerName, double amount)
    {
        if (amount <= 0)
        {
            // Проверка на положительное значение
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount should be greater than zero.");
        }
        var lender = GetUser(lenderName);
        var borrower = GetUser(borrowerName);

        if (lender == null || borrower == null)
        {
            // Проверка на существование заемщика или кредитора
            throw new KeyNotFoundException("Lender or borrower not found.");
        }

        //Обновление долгов
        lender.Owes[borrowerName] = lender.Owes.GetValueOrDefault(borrowerName) + amount;
        borrower.OwedBy[lenderName] = borrower.OwedBy.GetValueOrDefault(lenderName) + amount;

        return(lender, borrower);
    }
}
