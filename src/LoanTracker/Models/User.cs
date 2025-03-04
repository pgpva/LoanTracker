using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    public class User
    {
        public int Id { get; set; }  // Для Entity Framework необходимо добавить идентификатор
        public string Name { get; set; }
        public List<Iou> Owes { get; set; } = new List<Iou>();
        public List<Iou> OwedBy { get; set; } = new List<Iou>();
        public double Balance => OwedBy.Sum(i => i.Amount) - Owes.Sum(i => i.Amount);
    }
}
