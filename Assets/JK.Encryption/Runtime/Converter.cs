using System.IO;
using UnityEngine;

namespace JK.ByteArrays
{
    public static class Converter
    {
        public static byte[] ToByteArray<T> (T target)
        {
            var targetString = JsonUtility.ToJson (target);
            return ToByteArray (targetString);
        }

        public static byte[] ToByteArray (string target)
        {
            using var memoryStream = new MemoryStream ();
            using (var binaryWriter = new BinaryWriter (memoryStream))
            {
                 binaryWriter.Write (target);   
            }

            return memoryStream.ToArray ();
        }

        public static T FromByteArray<T> (byte[] source)
        {
            using var memoryStream = new MemoryStream (source);
            using var binaryReader = new BinaryReader (memoryStream);
            return JsonUtility.FromJson<T> (binaryReader.ReadString ());
        }

        public static string FromByteArray (byte[] source)
        {
            using var memoryStream = new MemoryStream (source);
            using var binaryReader = new BinaryReader (memoryStream);
            return binaryReader.ReadString ();
        }
    }
}
