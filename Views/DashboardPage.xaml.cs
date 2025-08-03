using Tokero.ViewModels;

namespace Tokero.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel viewModel)
        {
            viewModel.RefreshDataCommand.Execute(null);
        }
    }
}