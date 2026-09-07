using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeCanvas.Core;
using TimeCanvas.Core.Data;
using TimeCanvas.Core.Data.CompiledModels;
using TimeCanvas.Core.Services;
using TimeCanvas.ViewModels;

namespace TimeCanvas;

internal abstract class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            LogCrash(e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString()));

        try
        {
            Services = BuildServices();

            // Apply any pending migrations — creates the schema on a brand-new
            // install, and brings an older install forward after an update.
            using (var scope = Services.CreateScope())
            {
                var contextFactory = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<TimeCanvasDbContext>>();
                using var db = contextFactory.CreateDbContext();
                db.Database.Migrate();
            }

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogCrash(ex);
            throw;
        }
    }

    private static void LogCrash(Exception ex)
    {
        try
        {
            File.AppendAllText(AppPaths.CrashLogFile, $"{DateTime.Now:O}\n{ex}\n\n");
        }
        catch
        {
            // Best-effort — don't let logging itself crash the crash handler.
        }
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();

    private static IServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<TimeCanvasDbContext>(options =>
            options
                .UseSqlite($"Data Source={AppPaths.DatabaseFile}")
                .UseModel(TimeCanvasDbContextModel.Instance));

        services.AddScoped<StatsService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITemplateService, TemplateService>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<Views.MainWindow>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<Views.SettingsWindow>();
        services.AddTransient<TrendViewModel>();
        services.AddTransient<Views.TrendWindow>();
        services.AddTransient<TemplateEditorViewModel>();
        services.AddTransient<Views.TemplateEditorWindow>();

        services.AddSingleton<SettingsService>();

        return services.BuildServiceProvider();
    }
}
