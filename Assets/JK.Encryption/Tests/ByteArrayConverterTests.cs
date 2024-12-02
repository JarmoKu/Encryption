using NUnit.Framework;

namespace JK.Encryption.Tests
{
    public class ByteArrayConverterTests
    {
        private const string TestData = "testData";

        [Test]
        public void FromByteArray_RetursSameAsDataPassedToByteArray ()
        {
            var byteArray = ByteArrayConverter.ToByteArray (TestData);
            var returnedData = ByteArrayConverter.FromByteArray<string> (byteArray);
            Assert.AreEqual (TestData, returnedData);
        }
    }
}
