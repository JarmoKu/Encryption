using NUnit.Framework;

namespace JK.Encryption.Tests
{
    public class AESHelperTests
    {
        private const string Password = "testPassword";
        private const string WrongPassword = "testpassword";
        private const string TestData = "testData";

        [Test]
        public void CreateSalt_ResultLength_IsCorrect ()
        {
            var salt = AESHelper.CreateSalt ();
            Assert.AreEqual (AESHelper.SaltSize, salt.Length);
        }

        [Test]
        public void CreateSalt_RetursDifferentResult_AtDifferentTimes ()
        {
            var saltOne = AESHelper.CreateSalt ();
            var saltTwo = AESHelper.CreateSalt ();
            Assert.AreNotEqual (saltOne, saltTwo);
        }

        [Test]
        public void PassWordHash_Length_IsCorrect ()
        {
            var salt = AESHelper.CreateSalt ();
            var key = AESHelper.CreateKey (Password, salt);
            Assert.AreEqual (AESHelper.KeySize, key.GetBytes(AESHelper.KeySize).Length);
        }

        [Test]
        public void CreateKey_ReturnsSameResult_WithSameInput ()
        {
            var salt = AESHelper.CreateSalt ();
            var keyOne = AESHelper.CreateKey (Password, salt).GetBytes (AESHelper.KeySize);
            var keyTwo = AESHelper.CreateKey (Password, salt).GetBytes (AESHelper.KeySize);
            CollectionAssert.AreEqual (keyOne, keyTwo);
        }

        [Test]
        public void Data_IsEncrypted_AfterEncryption ()
        {
            var salt = AESHelper.CreateSalt ();
            var key = AESHelper.CreateKey (Password, salt);
            var encryptedData = AESHelper.Encrypt (TestData, key);
            Assert.AreNotEqual (TestData, encryptedData);
        }

        [Test]
        public void Data_IsEqualToBeforeEncryption_AfterDecryption ()
        {
            var salt = AESHelper.CreateSalt ();
            var key = AESHelper.CreateKey (Password, salt);
            var (bytes, IV) = AESHelper.Encrypt (TestData, key);
            var decryptedData = AESHelper.Decrypt (bytes, IV, key);
            Assert.AreEqual (TestData, decryptedData);
        }

        [Test]
        public void Data_CannotBeDecrypted_WithWrongPassword ()
        {
            var salt = AESHelper.CreateSalt ();
            var correctKey = AESHelper.CreateKey (Password, salt);
            var (bytes, IV) = AESHelper.Encrypt (TestData, correctKey);
            var falseKey = AESHelper.CreateKey (WrongPassword, salt);
            var encryptedData = AESHelper.Decrypt (bytes, IV, falseKey);
            Assert.IsNull (encryptedData);
        }

        [Test]
        public void Data_IsEqualToBeforeEncryption_AfterModification ()
        {
            var salt = AESHelper.CreateSalt ();
            var key = AESHelper.CreateKey (Password, salt);
            var (bytes, IV) = AESHelper.Encrypt (TestData, key);
            var decryptedData = AESHelper.Decrypt (bytes, IV, key);

            var modifiedData = decryptedData + "addition";
            (bytes, IV) = AESHelper.Encrypt (modifiedData, key);
            decryptedData = AESHelper.Decrypt (bytes, IV, key);
            Assert.AreEqual (modifiedData, decryptedData);
        }
    }
}
