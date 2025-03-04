using LoanTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class UserService
{
    private readonly LoanTrackerContext _context;

    public UserService(LoanTrackerContext context)
    {
        _context = context;
    }

    public List<User> GetAllUsers() => _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).ToList();

    public User GetUser(string name) => _context.Users.Include(u => u.Owes).ThenInclude(i => i.Borrower).Include(u => u.OwedBy).ThenInclude(i => i.Lender).FirstOrDefault(u => u.Name == name);

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
