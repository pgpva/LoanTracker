using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    /// <summary>
    /// Представляет долг (IOU) между двумя пользователями.
    /// </summary>
    public class Iou
    {
        /// <summary>
        /// Уникальный идентификатор долга.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор кредитора.
        /// </summary>
        public int LenderId { get; set; }

        /// <summary>
        /// Кредитор, которому должен долг.
        /// </summary>
        public User Lender { get; set; }

        /// <summary>
        /// Идентификатор заемщика.
        /// </summary>
        public int BorrowerId { get; set; }

        /// <summary>
        /// Заемщик, который должен вернуть долг.
        /// </summary>
        public User Borrower { get; set; }

        /// <summary>
        /// Сумма долга.
        /// </summary>
        public double Amount { get; set; }
    }
}
