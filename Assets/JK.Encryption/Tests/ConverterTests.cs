using NUnit.Framework;

namespace JK.ByteArrays.Tests
{
    public class ConverterTests
    {
        private const string TestData = "testData";

        [Test]
        public void GivenTestString_WhenPassedToFromByteArray_TheReturnsTheSameString ()
        {
            var byteArray = Converter.ToByteArray (TestData);

            var returnedData = Converter.FromByteArray (byteArray);

            Assert.AreEqual (TestData, returnedData);
        }

        [Test]
        public void GivenTestClassInstance_WhenPassedToFromByteArray_TheReturnsInstanceWithSameContents ()
        {
            var testInstance = new TestClass ();
            var byteArray = Converter.ToByteArray (testInstance);

            var returnedData = Converter
                .FromByteArray<TestClass> (byteArray);

            Assert.AreEqual (testInstance.TestData, returnedData.TestData);
        }

        [System.Serializable]
        public class TestClass
        {
            public string TestData = ConverterTests.TestData;
        }
    }
}
