using Tokero.ViewModels;

namespace Tokero.Views;

public partial class InvestmentPlanPage : ContentPage
{
	public InvestmentPlanPage(InvestmentPlanViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}