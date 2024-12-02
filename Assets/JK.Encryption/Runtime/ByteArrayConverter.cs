using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JK.Encryption
{
    public static class ByteArrayConverter
    {
        public static byte[] ToByteArray (object target)
        {
            var binaryFormatter = new BinaryFormatter ();
            using var memoryStream = new MemoryStream ();
            binaryFormatter.Serialize (memoryStream, target);

            return memoryStream.ToArray ();
        }

        public static T FromByteArray<T> (byte[] source)
        {
            using var memoryStream = new MemoryStream ();

            var binaryFormatter = new BinaryFormatter ();
            memoryStream.Write (source, 0, source.Length);
            memoryStream.Seek (0, SeekOrigin.Begin);

            var obj = binaryFormatter.Deserialize (memoryStream);

            return (T)obj;
        }
    }
}
