namespace ArtisanCode.Log4NetMessageEncryptor.Encryption
{
    public interface IMessageEncryptor
    {
        string Encrypt(string source);
    }
}