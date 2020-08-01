namespace FileEncryptor.WPF.Services.Interfaces
{
    internal interface IEncryptor
    {
        void Encrypt(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200);

        bool Decrypt(string SourcePath, string DestinationPath, string Password, int BufferLength = 104200);
    }
}
