using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace FirebaseWorkout
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); 
					fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
				});

            #region Dependency Injection for Views, ViewModels and Services
            builder.RegisterViews()
                   .RegisterViewModels()
                   .RegisterServices();
			#endregion

#if DEBUG
			builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            // Register ViewModels for Dependency Injection
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<Views.HomePageView>();
			builder.Services.AddTransient<Views.SignInView>();
            builder.Services.AddTransient<Views.SignUpView>();
            builder.Services.AddTransient<Views.MainPageView>();
			builder.Services.AddTransient<Views.AdminView>();
			builder.Services.AddTransient<Views.UsersListView>();
            builder.Services.AddTransient<Views.AccountView>();
            builder.Services.AddTransient<Views.ReportPageView>();
            builder.Services.AddTransient<Views.ServicePageView>();
            builder.Services.AddTransient<Views.UpdateUserView>();
			return builder;
        }
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            // Register ViewModels for Dependency Injection
            builder.Services.AddTransient<ViewModels.AppShellViewModel>();
            builder.Services.AddTransient<ViewModels.HomePageViewModel>();
            builder.Services.AddTransient<ViewModels.SignInViewModel>();
            builder.Services.AddTransient<ViewModels.SignUpViewModel>();
            builder.Services.AddTransient<ViewModels.MainPageViewModel>();
            builder.Services.AddTransient<ViewModels.AdminViewModel>();
			builder.Services.AddTransient<ViewModels.UsersListViewModel>();
            builder.Services.AddTransient<ViewModels.AccountViewModel>();
            builder.Services.AddTransient<ViewModels.ReportPageViewModel>();
            builder.Services.AddTransient<ViewModels.ServicePageViewModel>();
            builder.Services.AddTransient<ViewModels.UpdateUserViewModel>();
			return builder;
        }
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IAppLogger,LogService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
            builder.Services.AddTransient<IAppUserRepository, FirebaseUsersRepository>();
            builder.Services.AddTransient<IComputerRepository, FirebaseComputersRepository>();
            builder.Services.AddTransient<IReportRepository, FirebaseReportsRepository>();
            return builder;
        }
    }
}
