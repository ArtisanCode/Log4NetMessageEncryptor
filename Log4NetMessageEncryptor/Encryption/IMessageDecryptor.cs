namespace ArtisanCode.Log4NetMessageEncryptor.Encryption
{
    public interface IMessageDecryptor
    {
        string Decrypt(string source);
    }
}