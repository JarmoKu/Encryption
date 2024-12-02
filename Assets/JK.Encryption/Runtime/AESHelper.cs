using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace JK.Encryption
{
    public static class AESHelper
    {
        public const int SaltSize = 32;
        public const int KeySize = 32;
        private const int Iterations = 10000;

        public static byte[] CreateSalt ()
        {
            var salt = new byte[SaltSize];
            using var rngCsp = new RNGCryptoServiceProvider ();
            rngCsp.GetBytes (salt);

            return salt;
        }

        public static Rfc2898DeriveBytes CreateKey (string password, byte[] salt)
        {
            return new Rfc2898DeriveBytes (password, salt, Iterations);
        }

        public static (byte[] EncryptedData, byte[] IV) Encrypt (string data, Rfc2898DeriveBytes key)
        {
            try
            {
                var engAlc = Aes.Create ();
                engAlc.Key = key.GetBytes (KeySize);

                var encryptionStream = new MemoryStream ();
                var utfD = new UTF8Encoding (false).GetBytes (data);

                var encrypt = new CryptoStream (
                    encryptionStream,
                    engAlc.CreateEncryptor (),
                    CryptoStreamMode.Write
                    );

                encrypt.Write (utfD, 0, utfD.Length);
                encrypt.FlushFinalBlock ();
                encrypt.Close ();
                key.Reset ();

                var encryptedData = encryptionStream.ToArray ();
                return (encryptedData, engAlc.IV);
            }
            catch
            {
                return (null, null);
            }
        }

        public static string Decrypt (byte[] data, byte[] IV, Rfc2898DeriveBytes key)
        {
            try
            {
                var decAlg = Aes.Create ();
                decAlg.Key = key.GetBytes (KeySize);
                decAlg.IV = IV;

                var decryptionStream = new MemoryStream ();
                var decrypt = new CryptoStream (
                    decryptionStream,
                    decAlg.CreateDecryptor (),
                    CryptoStreamMode.Write
                    );

                decrypt.Write (data, 0, data.Length);
                decrypt.Flush ();
                decrypt.Close ();
                key.Reset ();

                var decryptedData = new UTF8Encoding (false).GetString (decryptionStream.ToArray ());
                return decryptedData;
            }
            catch
            {
                return null;
            }
        }
    }
}
