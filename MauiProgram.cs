using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MiniTrello.Persistences;
using MiniTrello.Persistences.Contracts;
using MiniTrello.Services;
using MiniTrello.ViewModels;
using MiniTrello.Views;

namespace MiniTrello;

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
            })
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        // Persistences (SQLite)
        mauiAppBuilder.Services.AddScoped<IBoardPersistence, BoardSQLite>();
        mauiAppBuilder.Services.AddScoped<ICardItemPersistence, CardItemSQLite>();

        ServiceLocator.ServiceProvider = mauiAppBuilder.Services.BuildServiceProvider();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddScoped<BoardListViewModel>();
        mauiAppBuilder.Services.AddScoped<BoardDetailViewModel>();
        mauiAppBuilder.Services.AddScoped<CardDetailViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddScoped<BoardListPage>();
        mauiAppBuilder.Services.AddScoped<BoardDetailPage>();
        mauiAppBuilder.Services.AddScoped<CardDetailPage>();

        return mauiAppBuilder;
    }
}
