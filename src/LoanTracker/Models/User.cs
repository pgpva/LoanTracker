using System.Collections.Generic;

namespace LoanTracker.Models
{
    public class User
    {
        public string Name { get; set; }
        public Dictionary<string, double> Owes { get; set; } = new();
        public Dictionary<string, double> OwedBy { get; set; } = new();
        public double Balance => CalculateBalance();

        // Метод для вычисления баланса
        private double CalculateBalance()
        {
            double totalOwed = OwedBy.Values.Sum();
            double totalOwes = Owes.Values.Sum();
            return totalOwed - totalOwes;
        }

        // Метод для получения отсортированного списка долгов, которые должны быть возвращены пользователю
        public Dictionary<string, double> GetSortedOwes()
        {
            return Owes.OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Метод для получения отсортированного списка долгов, которые пользователь должен вернуть
        public Dictionary<string, double> GetSortedOwedBy()
        {
            return OwedBy.OrderBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

    }
}
