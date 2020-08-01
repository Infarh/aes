using System.Windows;
using FileEncryptor.WPF.Infrastructure.Commands.Base;

namespace FileEncryptor.WPF.Infrastructure.Commands
{
    internal class CloseWindow : Command
    {
        protected override bool CanExecute(object parameter) => (parameter as Window ?? App.FocusedWindow ?? App.ActivedWindow) != null;

        protected override void Execute(object parameter) => (parameter as Window ?? App.FocusedWindow ?? App.ActivedWindow)?.Close();
    }
}
