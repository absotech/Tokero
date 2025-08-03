using Tokero.ViewModels;

namespace Tokero.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}