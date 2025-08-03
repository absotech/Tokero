using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tokero.Extensions;
using Tokero.Models;
using Tokero.Services;

namespace Tokero.ViewModels;

public class InvestmentPlanViewModel : INotifyPropertyChanged
{
    private DateTime _startDate = DateTime.Now;
    private decimal investmentAmount = 100;
    private AllocationStrategy selectedAllocationStrategy = AllocationStrategy.EqualDistribution;
    private string notes = "";
    private string planName = "New Investment Plan";
    public ObservableCollection<ConfiguredAssetViewModel> ConfiguredAssets { get; } = [];

    public DateTime StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }

    public decimal InvestmentAmount
    {
        get => investmentAmount;
        set { investmentAmount = value; OnPropertyChanged(); }
    }

    public AllocationStrategy SelectedAllocationStrategy
    {
        get => selectedAllocationStrategy;
        set { selectedAllocationStrategy = value; OnPropertyChanged(); CheckDistribution(); }
    }
    public string Notes
    {
        get => notes;
        set { notes = value; OnPropertyChanged(); }
    }
    public string PlanName
    {
        get => planName;
        set { planName = value; OnPropertyChanged(); }
    }

    public ObservableCollection<int> DaysOfMonth { get; }
    public List<string> AllocationStrategies { get => [.. Enum.GetNames(typeof(AllocationStrategy)).Select(a => a.SplitCamelCase())]; }
    public ObservableCollection<CryptoCurrency> AvailableCryptos { get; }

    public ICommand CreatePlanCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ToggleSelectionCommand { get; }

    public InvestmentPlanViewModel()
    {
        DaysOfMonth = new ObservableCollection<int>(Enumerable.Range(1, 28));
        AvailableCryptos =
        [
            new() { Symbol = "BTC", Name = "Bitcoin" },
            new() { Symbol = "ETH", Name = "Ethereum" },
            new() { Symbol = "SOL", Name = "Solana" },
            new() { Symbol = "XRP", Name = "Ripple" },
            new() { Symbol = "ADA", Name = "Cardano" },
            new() { Symbol = "DOT", Name = "Polkadot" }
        ];

        CreatePlanCommand = new Command(async () => await OnCreatePlan());
        CancelCommand = new Command(async () => await OnCancel());
        ToggleSelectionCommand = new Command<CryptoCurrency>(OnToggleSelection);
    }

    private void OnToggleSelection(CryptoCurrency crypto)
    {
        var existingAsset = ConfiguredAssets.FirstOrDefault(a => a.Symbol == crypto.Symbol);
        if (existingAsset != null)
        {
            ConfiguredAssets.Remove(existingAsset);
            crypto.IsSelected = false;
        }
        else
        {
            ConfiguredAssets.Add(new ConfiguredAssetViewModel
            {
                Symbol = crypto.Symbol,
                Name = crypto.Name,
                StartDate = this.StartDate
            });
            crypto.IsSelected = true;

        }
        CheckDistribution();
    }

    private void CheckDistribution()
    {
        if (SelectedAllocationStrategy == AllocationStrategy.EqualDistribution && ConfiguredAssets.Count != 0)
        {
            var share = Math.Floor(InvestmentAmount / ConfiguredAssets.Count);
            foreach (var asset in ConfiguredAssets)
            {
                asset.MonthlyInvestment = share;
            }
        }
    }

    private async Task OnCreatePlan()
    {
        if (ConfiguredAssets.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please select and configure at least one cryptocurrency.", "OK");
            return;
        }

        var newPlan = new InvestmentPlan
        {
            PlanName = PlanName,
            AllocationStrategy = this.SelectedAllocationStrategy,
            Notes = this.Notes,
            ConfiguredAssets = [.. ConfiguredAssets.Select(vm => new ConfiguredAsset
            {
                Symbol = vm.Symbol,
                Name = vm.Name,
                MonthlyInvestment = vm.MonthlyInvestment,
                StartDate = vm.StartDate
            })]
        };
        if (newPlan.AllocationStrategy == AllocationStrategy.EqualDistribution)
        {
            var share = InvestmentAmount / newPlan.ConfiguredAssets.Count;
            foreach (var asset in newPlan.ConfiguredAssets)
            {
                asset.MonthlyInvestment = share;
            }
        }
        await DatabaseService.SavePlanAsync(newPlan);

        await Shell.Current.GoToAsync("//PortfolioPage");
    }

    private async Task OnCancel()
    {
        await Shell.Current.GoToAsync("//DashboardPage");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
public class ConfiguredAssetViewModel : INotifyPropertyChanged
{
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    private decimal _monthlyInvestment;
    private DateTime _startDate;

    public decimal MonthlyInvestment
    {
        get => _monthlyInvestment;
        set { _monthlyInvestment = value; OnPropertyChanged(); }
    }
    public DateTime StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
public enum AllocationStrategy
{
    Custom,
    EqualDistribution
}