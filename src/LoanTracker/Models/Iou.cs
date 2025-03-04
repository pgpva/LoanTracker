using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    public class Iou
    {
        public int Id { get; set; }
        public int LenderId { get; set; }
        public User Lender { get; set; }
        public int BorrowerId { get; set; }
        public User Borrower { get; set; }
        public double Amount { get; set; }
    }
}
