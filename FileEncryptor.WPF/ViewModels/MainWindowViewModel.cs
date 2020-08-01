using System.IO;
using System.Windows.Input;
using FileEncryptor.WPF.Infrastructure.Commands;
using FileEncryptor.WPF.Services.Interfaces;
using FileEncryptor.WPF.ViewModels.Base;

namespace FileEncryptor.WPF.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IUserDialog _UserDialog;

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Шифратор";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region Password : string - Пароль

        /// <summary>Пароль</summary>
        private string _Password = "123";

        /// <summary>Пароль</summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        #endregion

        #region SelectedFile : FileInfo - Выбранный файл

        /// <summary>Выбранный файл</summary>
        private FileInfo _SelectedFile;

        /// <summary>Выбранный файл</summary>
        public FileInfo SelectedFile { get => _SelectedFile; set => Set(ref _SelectedFile, value); }

        #endregion

        #region Команды

        private ICommand _SelectFileCommand;

        public ICommand SelectFileCommand => _SelectFileCommand ??= new LambdaCommand(OnSelectFileCommandExecuted);

        private void OnSelectFileCommandExecuted()
        {
            if (!_UserDialog.OpenFile("Выбор файла для шифрования", out var file_path)) return;
            var selected_file = new FileInfo(file_path);
            SelectedFile = selected_file.Exists ? selected_file : null;
        }

        #endregion

        public MainWindowViewModel(IUserDialog UserDialog)
        {
            _UserDialog = UserDialog;
        }
    }
}
