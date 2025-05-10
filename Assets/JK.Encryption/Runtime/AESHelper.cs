using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace JK.Encryption
{
    public static class AESHelper
    {
        public const int KeySize = 32;
        public const int SaltSize = 64;
        private const int Iterations = 10000;

        public static byte[] CreateSalt ()
        {
            var salt = new byte[SaltSize];
            using var rngCsp = new RNGCryptoServiceProvider ();
            rngCsp.GetBytes (salt);

            return salt;
        }

        public static byte[] CreateKey (string password, byte[] salt)
        {
            return new Rfc2898DeriveBytes (password, salt, Iterations)
                .GetBytes (KeySize);
        }

        public static byte[] Encrypt (string data, byte[] key)
        {
            using var aes = Aes.Create ();
            aes.Key = key;

            using var encryptionStream = new MemoryStream ();

            using var encrypt = new CryptoStream (
                encryptionStream,
                aes.CreateEncryptor (),
                CryptoStreamMode.Write
                );

            var utfD = new UTF8Encoding (false).GetBytes (data);

            encrypt.Write (utfD, 0, utfD.Length);
            encrypt.FlushFinalBlock ();
            encrypt.Dispose ();

            var encryptedData = encryptionStream.ToArray ();
            return CombineDataAndIV (encryptedData, aes.IV); ;
        }

        public static string Decrypt (byte[] dataAndIV, byte[] key)
        {
            try
            {
                using var decryptionStream = new MemoryStream ();

                using (var decAlg = Aes.Create ())
                {
                    var (data, IV) = SeparateDataAndIV (dataAndIV);

                    decAlg.Key = key;
                    decAlg.IV = IV;

                    using var decrypt = new CryptoStream (
                        decryptionStream,
                        decAlg.CreateDecryptor (),
                        CryptoStreamMode.Write
                        );

                    decrypt.Write (data, 0, data.Length);
                    decrypt.FlushFinalBlock ();
                    decrypt.Dispose ();
                }

                var decryptedData = new UTF8Encoding (false)
                    .GetString (decryptionStream.ToArray ());

                return decryptedData;
            }
            catch
            {
                return null;
            }
        }

        private static byte[] CombineDataAndIV (byte[] encryptedData, byte[] IV)
        {
            var length = IV.Length + encryptedData.Length;
            var bytes = new byte[length];

            for (var i = 0; i < length; i++)
            {
                bytes[i] = i < IV.Length ? IV[i] : encryptedData[i - IV.Length];
            }

            return bytes;
        }

        private static (byte[] data, byte[] IV) SeparateDataAndIV (byte[] dataAndIV)
        {
            var IV = new byte[16];
            var data = new byte[dataAndIV.Length - 16];

            for (var i = 0; i < dataAndIV.Length; i++)
            {
                if (i < 16)
                {
                    IV[i] = dataAndIV[i];
                }
                else
                {
                    data[i - 16] = dataAndIV[i];
                }
            }

            return (data, IV);
        }
    }
}
