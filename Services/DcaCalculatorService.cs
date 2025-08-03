using Tokero.Models;

namespace Tokero.Services
{
    public class DcaCalculatorService
    {
        private readonly CoinMarketCapPricingService pricingService;

        public DcaCalculatorService(CoinMarketCapPricingService pricingService)
        {
            this.pricingService = pricingService;
        }

        public async Task<List<Investment>> Calculate(InvestmentPlan plan)
        {
            var results = new List<Investment>();
            if (plan.ConfiguredAssets == null || plan.ConfiguredAssets.Count == 0)
            {
                return results;
            }

            decimal totalInvested = 0m;
            var totalCryptoOwned = plan.ConfiguredAssets.ToDictionary(asset => asset.Symbol, _ => 0m);
            var firstStartDate = plan.ConfiguredAssets.Min(a => a.StartDate);

            var currentDate = firstStartDate;
            decimal previousPortfolioValue = 0m;

            while (currentDate <= DateTime.Now)
            {
                bool investmentMadeThisMonth = false;
                var pricesOnDate = new Dictionary<string, decimal>();
                var symbolsToFetch = plan.ConfiguredAssets
                    .Where(a => currentDate >= a.StartDate)
                    .Select(a => a.Symbol)
                    .Distinct()
                    .ToList();

                foreach (var symbol in symbolsToFetch)
                {
                    pricesOnDate[symbol] = await pricingService.GetHistoricalPriceAsync(symbol, currentDate);
                }

                foreach (var asset in plan.ConfiguredAssets)
                {
                    if (currentDate >= asset.StartDate)
                    {
                        investmentMadeThisMonth = true;
                        totalInvested += asset.MonthlyInvestment;

                        var historicalPrice = pricesOnDate[asset.Symbol];
                        if (historicalPrice > 0)
                        {
                            var cryptoBoughtThisMonth = asset.MonthlyInvestment / historicalPrice;
                            totalCryptoOwned[asset.Symbol] += cryptoBoughtThisMonth;
                        }
                    }
                }

                if (investmentMadeThisMonth)
                {
                    decimal portfolioValueThisMonth = 0m;
                    foreach (var symbol in totalCryptoOwned.Keys)
                    {
                        if (totalCryptoOwned[symbol] > 0)
                        {
                            var priceOnDate = pricesOnDate[symbol];
                            portfolioValueThisMonth += totalCryptoOwned[symbol] * priceOnDate;
                        }
                    }

                    var totalRoi = totalInvested > 0 ? ((portfolioValueThisMonth - totalInvested) / totalInvested) * 100 : 0m;
                    var monthRoi = previousPortfolioValue > 0 ? ((portfolioValueThisMonth - previousPortfolioValue) / previousPortfolioValue) * 100 : 0m;

                    results.Add(new Investment
                    {
                        Date = currentDate,
                        Amount = totalInvested,
                        CryptoSymbol = string.Join(", ", plan.ConfiguredAssets.Select(a => a.Symbol).Distinct().ToList()),
                        CurrentValue = portfolioValueThisMonth,
                        TotalRoi = totalRoi,
                        MonthRoi = monthRoi,
                        TotalRoiColor = GetRoiColor(totalRoi),
                        MonthRoiColor = GetRoiColor(monthRoi),
                        RowBackgroundColor = results.Count % 2 == 0 ? Colors.White : Color.FromArgb("#f7f7f7")
                    });

                    previousPortfolioValue = portfolioValueThisMonth;
                }

                currentDate = currentDate.AddMonths(1);
            }

            if (results.Count != 0)
            {
                decimal finalPortfolioValue = 0m;
                var uniqueSymbols = totalCryptoOwned.Where(kvp => kvp.Value > 0).Select(kvp => kvp.Key).ToList();

                foreach (var symbol in uniqueSymbols)
                {
                    var currentPrice = (decimal)await pricingService.GetCurrentPriceAsync(symbol);
                    finalPortfolioValue += totalCryptoOwned[symbol] * currentPrice;
                }

                var latestResult = results.Last();
                var previousValue = latestResult.CurrentValue;
                latestResult.CurrentValue = finalPortfolioValue;
                latestResult.TotalRoi = totalInvested > 0 ? ((finalPortfolioValue - totalInvested) / totalInvested) * 100 : 0m;
                latestResult.MonthRoi = previousValue > 0 ? ((finalPortfolioValue - previousValue) / previousValue) * 100 : 0m;
                latestResult.TotalRoiColor = GetRoiColor(latestResult.TotalRoi);
                latestResult.MonthRoiColor = GetRoiColor(latestResult.MonthRoi);
            }

            return results.OrderByDescending(r => r.Date).ToList();
        }

        private Color GetRoiColor(decimal roi)
        {
            if (roi > 0)
                return Colors.Green;
            else if (roi < 0)
                return Colors.Red;
            else
                return Colors.Gray;
        }
    }
}