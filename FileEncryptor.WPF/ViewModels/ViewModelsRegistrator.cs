using Microsoft.Extensions.DependencyInjection;

namespace FileEncryptor.WPF.ViewModels
{
    internal static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
           .AddSingleton<MainWindowViewModel>()
        ;
    }
}
