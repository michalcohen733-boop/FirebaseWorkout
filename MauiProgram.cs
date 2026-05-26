using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using FirebaseWorkout.Service;
using FirebaseWorkout.Service.DBService;
using FirebaseWorkout.Service.DBService.Firebase;
using Microsoft.Extensions.Logging;
using BarcodeScanning;

namespace FirebaseWorkout
{
    // נקודת הכניסה של האפליקציה - הגדרת MAUI, DI, פונטים וסריקת ברקודים
    public static class MauiProgram
    {
        // בניית האפליקציה - הגדרת כל הרכיבים
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // הפעלת ספריית סריקת ברקודים (BarcodeScanning.Native.Maui)
                .UseBarcodeScanning()
                // רישום פונטים - OpenSans + Material Icons לאייקונים
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
				});

            // רישום כל הרכיבים ב-DI (Dependency Injection)
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

        // רישום כל ה-Views כ-Transient (נוצרים מחדש בכל פעם)
        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
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
        // רישום כל ה-ViewModels כ-Transient
        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
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
        // רישום שירותים - Singleton לשירותים משותפים, Transient ל-Repositories
        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            // שירותים משותפים (Singleton - מופע אחד לכל האפליקציה)
            builder.Services.AddSingleton<IAppLogger,LogService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();
            builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
            // Repositories (Transient - מופע חדש בכל בקשה)
            builder.Services.AddTransient<IAppUserRepository, FirebaseUsersRepository>();
            builder.Services.AddTransient<IComputerRepository, FirebaseComputersRepository>();
            builder.Services.AddTransient<IReportRepository, FirebaseReportsRepository>();
            return builder;
        }
    }
}
