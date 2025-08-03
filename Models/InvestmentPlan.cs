using Tokero.ViewModels;

namespace Tokero.Models
{
    public class InvestmentPlan
    {
        public string Id { get; set; }
        public string PlanName { get; set; }
        public AllocationStrategy AllocationStrategy { get; set; }
        public List<ConfiguredAsset> ConfiguredAssets { get; set; }
        public string Notes { get; set; }
    }
}
