using Tokero.ViewModels;

namespace Tokero.Views;

public partial class PortfolioPage : ContentPage
{
	public PortfolioPage(PortfolioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is PortfolioViewModel viewModel)
		{
			viewModel.RefreshCommand.Execute(null);
		}
    }
}