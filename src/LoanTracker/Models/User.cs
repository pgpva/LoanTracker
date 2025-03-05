using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    /// <summary>
    /// Представляет пользователя в системе.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список долгов, которые пользователь должен вернуть.
        /// </summary>
        public List<Iou> Owes { get; set; } = new List<Iou>();

        /// <summary>
        /// Список долгов, которые другие пользователи должны вернуть этому пользователю.
        /// </summary>
        public List<Iou> OwedBy { get; set; } = new List<Iou>();

        /// <summary>
        /// Баланс пользователя, вычисляемый как разница между долговыми обязательствами, которые он должен вернуть, и которые он должен получить.
        /// </summary>
        public double Balance => OwedBy.Sum(i => i.Amount) - Owes.Sum(i => i.Amount);
    }
}
