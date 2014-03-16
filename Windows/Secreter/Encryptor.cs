using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Secreter
{
    public class Encryptor
    {
        public static int GetIVLength()
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                return aes.BlockSize / 8;
            }
        }

        public static byte[] CreateIV()
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                return aes.IV;
            }
        }

        private const string saltStr = "secreter";
        private Stream source;
        private byte[] IV;

        public Encryptor(Stream source, byte[] IV)
        {
            this.source = source;
            this.IV = IV;
        }

        public void Encrypt(string key, Stream dest)
        {
            var aes = new AesCryptoServiceProvider();

            aes.Key = this.GetKey(key, aes);
            aes.IV = this.IV;

            // Create an encrytor to perform the stream transform.
            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (var encStream = new CryptoStream(dest, encryptor, CryptoStreamMode.Write))
            {
                this.source.CopyTo(encStream);
            }
        }

        public Stream Decrypt(string key)
        {
            var aes = new AesCryptoServiceProvider();

            aes.Key = this.GetKey(key, aes);
            aes.IV = this.IV;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aes.CreateDecryptor();

            return new CryptoStream(this.source, decryptor, CryptoStreamMode.Read);
        }

        private byte[] GetKey(string password, AesCryptoServiceProvider aes)
        {
            var salt = Encoding.ASCII.GetBytes(Encryptor.saltStr);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(password, salt, "SHA1", 10);
            return DerivedPassword.GetBytes(aes.KeySize / 8);
        }

        /*        public static void Main()
                        {
                            try
                            {
                                string original = "Here is some data to encrypt!";

                                // Create a new instance of the AesCryptoServiceProvider 
                                // class.  This generates a new key and initialization  
                                // vector (IV). 
                                using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                                {

                                    // Encrypt the string to an array of bytes. 
                                    byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);

                                    // Decrypt the bytes to a string. 
                                    string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                                    //Display the original data and the decrypted data.
                                    Console.WriteLine("Original:   {0}", original);
                                    Console.WriteLine("Round Trip: {0}", roundtrip);
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: {0}", e.Message);
                            }
                        }

                        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
                        {
                            // Check arguments. 
                            if (plainText == null || plainText.Length <= 0)
                                throw new ArgumentNullException("plainText");
                            if (Key == null || Key.Length <= 0)
                                throw new ArgumentNullException("Key");
                            if (IV == null || IV.Length <= 0)
                                throw new ArgumentNullException("Key");
                            byte[] encrypted;
                            // Create an AesCryptoServiceProvider object 
                            // with the specified key and IV. 
                            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                            {
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;

                                // Create a decrytor to perform the stream transform.
                                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                                // Create the streams used for encryption. 
                                using (MemoryStream msEncrypt = new MemoryStream())
                                {
                                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                                    {
                                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                        {

                                            //Write all data to the stream.
                                            swEncrypt.Write(plainText);
                                        }
                                        encrypted = msEncrypt.ToArray();
                                    }
                                }
                            }


                            // Return the encrypted bytes from the memory stream. 
                            return encrypted;

                        }

                        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
                        {
                            // Check arguments. 
                            if (cipherText == null || cipherText.Length <= 0)
                                throw new ArgumentNullException("cipherText");
                            if (Key == null || Key.Length <= 0)
                                throw new ArgumentNullException("Key");
                            if (IV == null || IV.Length <= 0)
                                throw new ArgumentNullException("IV");

                            // Declare the string used to hold 
                            // the decrypted text. 
                            string plaintext = null;

                            // Create an AesCryptoServiceProvider object 
                            // with the specified key and IV. 
                            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                            {
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;

                                // Create a decrytor to perform the stream transform.
                                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                                // Create the streams used for decryption. 
                                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                                {
                                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                    {
                                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                        {

                                            // Read the decrypted bytes from the decrypting stream 
                                            // and place them in a string.
                                            plaintext = srDecrypt.ReadToEnd();
                                        }
                                    }
                                }

                            }

                            return plaintext;

                        }
                */    }
}
