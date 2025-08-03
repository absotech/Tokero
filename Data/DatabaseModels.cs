using SQLite;
using Tokero.ViewModels;

namespace Tokero.Data
{
    public class UserDb
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class InvestmentPlanDb
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string PlanName { get; set; }
        public AllocationStrategy AllocationStrategy { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ConfiguredAssetDb
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal MonthlyInvestment { get; set; }
        public DateTime StartDate { get; set; }

        [Indexed]
        public string InvestmentPlanId { get; set; }
    }
}
