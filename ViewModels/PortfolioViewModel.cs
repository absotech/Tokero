using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tokero.Models;
using Tokero.Services;

namespace Tokero.ViewModels;

public class PortfolioViewModel : INotifyPropertyChanged
{
    private readonly DcaCalculatorService dcaCalculator;

    private decimal totalInvested;
    private decimal currentValue;
    private decimal overallRoi;
    private bool isRefreshing;
    private ObservableCollection<InvestmentPlan> allPlans = [];
    private ObservableCollection<Investment> allInvestments = [];

    public decimal TotalInvested { get => totalInvested; set { totalInvested = value; OnPropertyChanged(); } }
    public decimal CurrentValue { get => currentValue; set { currentValue = value; OnPropertyChanged(); } }
    public decimal OverallRoi { get => overallRoi; set { overallRoi = value; OnPropertyChanged(); } }
    public bool IsRefreshing { get => isRefreshing; set { isRefreshing = value; OnPropertyChanged(); } }
    public ObservableCollection<InvestmentPlan> AllPlans
    {
        get => allPlans;
        set
        {
            allPlans = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Investment> AllInvestments 
    {
        get => allInvestments;
        set
        {
            allInvestments = value;
            OnPropertyChanged();
        }
    }

    public ICommand BackToDashboardCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand DeleteCommand { get; }

    public PortfolioViewModel()
    {
        var pricingService = new CoinMarketCapPricingService("03c6621f-5ffd-42a7-928a-9774f973ec85");
        dcaCalculator = new DcaCalculatorService(pricingService);

        BackToDashboardCommand = new Command(async () => await OnBackToDashboard());
        RefreshCommand = new Command(async () => await LoadPortfolioFromDatabaseAsync());
        DeleteCommand = new Command<InvestmentPlan>(async (plan) => await OnDeletePlanAsync(plan));
    }

    private async Task LoadPortfolioFromDatabaseAsync()
    {
        var planList = await DatabaseService.GetAllPlansAsync();
        var combinedHistory = new List<Investment>();
        AllPlans.Clear();
        foreach (var plan in planList)
        {
            AllPlans.Add(plan);
        }
        if (AllPlans.Any())
        {
            decimal portfolioTotalInvested = 0;
            decimal portfolioCurrentValue = 0;
            foreach (var plan in AllPlans)
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
            var sortedHistory = combinedHistory.OrderByDescending(i => i.Date).ToList();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AllInvestments = new ObservableCollection<Investment>(sortedHistory);
            });
        }
        else
        {
            TotalInvested = 0;
            CurrentValue = 0;
            OverallRoi = 0;
            MainThread.BeginInvokeOnMainThread(() => AllInvestments.Clear());
        }
        IsRefreshing = false;
    }
    private async Task OnDeletePlanAsync(InvestmentPlan plan)
    {
        if (plan == null) return;
        var result = await Application.Current.MainPage.DisplayAlert(
            "Delete Plan",
            $"Are you sure you want to delete the plan '{plan.PlanName}'?",
            "Yes", "No");
        if (result)
        {
            await DatabaseService.DeletePlanAsync(plan);
            await LoadPortfolioFromDatabaseAsync();
        }
    }
    private static async Task OnBackToDashboard()
    {
        await Shell.Current.GoToAsync("//DashboardPage");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}