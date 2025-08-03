using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tokero.Services;

namespace Tokero.ViewModels;

public partial class LoginViewModel : INotifyPropertyChanged
{
    private string username = "";
    private string password = "";

    public string Username
    {
        get => username;
        set
        {
            username = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => password;
        set
        {
            password = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await OnLogin());
    }

    private async Task OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter username and password", "OK");
            return;
        }
        if (await DatabaseService.Login(Username, Password))
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Invalid credentials", "OK");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

