using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tokero.Models;
using Tokero.Services;

namespace Tokero.ViewModels;
public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DcaCalculatorService dcaCalculator;


    private decimal totalInvested = 0;
    private decimal currentValue = 0;
    private decimal overallRoi = 0;

    public decimal TotalInvested
    {
        get => totalInvested;
        set
        {
            totalInvested = value;
            OnPropertyChanged();
        }
    }
    public decimal CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            OnPropertyChanged();
        }
    }
    public decimal OverallRoi
    {
        get => overallRoi;
        set
        {
            overallRoi = value;
            OnPropertyChanged();
        }
    }
    public ICommand ViewPortfolioCommand { get; }
    public ICommand CreateInvestmentPlanCommand { get; }
    public ICommand RefreshDataCommand { get; }
    public ICommand LogoutCommand { get; }

    public DashboardViewModel()
    {
        var pricingService = new CoinMarketCapPricingService("03c6621f-5ffd-42a7-928a-9774f973ec85");
        dcaCalculator = new DcaCalculatorService(pricingService);

        ViewPortfolioCommand = new Command(async () => await OnViewPortfolio());
        CreateInvestmentPlanCommand = new Command(async () => await OnCreateInvestmentPlan());
        RefreshDataCommand = new Command(async () => await OnRefreshData());
        LogoutCommand = new Command(async () => await OnLogout());
    }
    private async Task LoadDashboardFromDatabaseAsync()
    {
        var planList = await DatabaseService.GetAllPlansAsync();
        var combinedHistory = new List<Investment>();
        if (planList.Count != 0)
        {
            decimal portfolioTotalInvested = 0;
            decimal portfolioCurrentValue = 0;
            foreach (var plan in planList)
            {
                var planHistory = await dcaCalculator.Calculate(plan);
                if (planHistory != null && planHistory.Count != 0)
                {
                    combinedHistory.AddRange(planHistory);
                    var latestStateForPlan = planHistory.OrderByDescending(i => i.Date).FirstOrDefault();

                    if (latestStateForPlan != null)
                    {
                        portfolioTotalInvested += latestStateForPlan.Amount;
                        portfolioCurrentValue += latestStateForPlan.CurrentValue;
                    }
                }
            }
            TotalInvested = portfolioTotalInvested;
            CurrentValue = portfolioCurrentValue;
            OverallRoi = TotalInvested > 0 ? (CurrentValue - TotalInvested) / TotalInvested : 0;
        }
        else
        {
            TotalInvested = 0;
            CurrentValue = 0;
            OverallRoi = 0;
        }
    }

    private async Task OnViewPortfolio()
    {
        await Shell.Current.GoToAsync("//PortfolioPage");
    }

    private async Task OnCreateInvestmentPlan()
    {
        await Shell.Current.GoToAsync("//InvestmentPlanPage");
    }

    private async Task OnRefreshData()
    {
        LoadDashboardFromDatabaseAsync();
    }

    private async Task OnLogout()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

