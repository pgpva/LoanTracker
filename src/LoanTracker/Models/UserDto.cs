using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LoanTracker.Models
{
    public class UserDto
    {
        public string Name { get; set; }
        public double Balance { get; set; }
        public Dictionary<string, double> Owes { get; set; }
        public Dictionary<string, double> OwedBy { get; set; }
    }
}
