using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IEncryptor
    {
        void Encrypt(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200);

        bool Decrypt(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200);

        Task EncryptAsync(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200, IProgress<double> Progress = null, CancellationToken Cancel = default);

        Task<bool> DecryptAsync(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200, IProgress<double> Progress = null, CancellationToken Cancel = default);
    }
}
