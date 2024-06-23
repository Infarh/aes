using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace FileEncryptor.WPF.Views.Windows
{
    public partial class ProgressWindow
    {
        #region Status : string - Статусное сообщение

        /// <summary>Статусное сообщение</summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(string),
                typeof(ProgressWindow),
                new PropertyMetadata(default(string)));

        /// <summary>Статусное сообщение</summary>
        //[Category("")]
        [Description("Статусное сообщение")]
        public string Status { get => (string)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

        #endregion

        #region ProgressValue : double - Значение прогресса

        /// <summary>Значение прогресса</summary>
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register(
                nameof(ProgressValue),
                typeof(double),
                typeof(ProgressWindow),
                new PropertyMetadata(double.NaN, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject D, DependencyPropertyChangedEventArgs E)
        {
            var progress_value = (double)E.NewValue;
            var progress_view = ((ProgressWindow)D).ProgressView;
            progress_view.Value = progress_value;
            progress_view.IsIndeterminate = double.IsNaN(progress_value);
        }

        /// <summary>Значение прогресса</summary>
        //[Category("")]
        [Description("Значение прогресса")]
        public double ProgressValue { get => (double)GetValue(ProgressValueProperty); set => SetValue(ProgressValueProperty, value); }

        #endregion

        private IProgress<double> _ProgressInformer;

        public IProgress<double> ProgressInformer => _ProgressInformer ??= new Progress<double>(p => ProgressValue = p);

        private IProgress<string> _StatusInformer;

        public IProgress<string> StatusInformer => _StatusInformer ??= new Progress<string>(status => Status = status);

        private IProgress<(double Percent, string Message)> _ProgressStatusInformer;

        public IProgress<(double Percent, string Message)> ProgressStatusInformer => _ProgressStatusInformer
            ??= new Progress<(double Percent, string Message)>(
                p =>
                {
                    ProgressValue = p.Percent;
                    Status = p.Message;
                });

        private CancellationTokenSource _Cancellation;

        public CancellationToken Cancel
        {
            get
            {
                if (_Cancellation != null) return _Cancellation.Token;
                _Cancellation = new CancellationTokenSource();
                CancellButton.IsEnabled = true;
                return _Cancellation.Token;
            }
        }

        public ProgressWindow() => InitializeComponent();

        private void OnCancellClick(object Sender, RoutedEventArgs E) => _Cancellation?.Cancel();
    }
}
