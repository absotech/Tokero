using Microsoft.Extensions.Logging;
using Tokero.ViewModels;
using Tokero.Views;

namespace Tokero
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<PortfolioPage>();
            builder.Services.AddTransient<PortfolioViewModel>();
            builder.Services.AddTransient<InvestmentPlanPage>();
            builder.Services.AddTransient<InvestmentPlanViewModel>();


            return builder.Build();
        }
    }
}
