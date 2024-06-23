using Microsoft.Extensions.DependencyInjection;

namespace FileEncryptor.WPF.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
    }
}
