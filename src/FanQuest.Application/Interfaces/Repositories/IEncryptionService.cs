namespace FanQuest.Application.Interfaces.Repositories
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
