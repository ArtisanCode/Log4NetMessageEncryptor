namespace ArtisanCode.Log4NetMessageEncryptor
{
    public interface IMessageDecryptor
    {
        string Decrypt(string source);
    }
}