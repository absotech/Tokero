namespace Tokero.Models
{
    public class ConfiguredAsset
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal MonthlyInvestment { get; set; }
        public DateTime StartDate { get; set; }
    }
}
