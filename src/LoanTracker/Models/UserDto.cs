using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    /// <summary>
    /// Представляет данные пользователя для отображения в API.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Баланс пользователя (сумма долгов, которые он должен получить, минус сумма долгов, которые он должен вернуть).
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Долги, которые должен вернуть пользователь (по отношению к другим пользователям).
        /// </summary>
        public Dictionary<string, double> Owes { get; set; }

        /// <summary>
        /// Долги, которые другие пользователи должны вернуть этому пользователю.
        /// </summary>
        public Dictionary<string, double> OwedBy { get; set; }
    }
}
