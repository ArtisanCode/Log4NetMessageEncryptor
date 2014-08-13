using ArtisanCode.LogManager.Options;

namespace ArtisanCode.LogManager
{
    public interface ILogEncrypt
    {
        void Encrypt(EncryptOptions options);
    }
}
