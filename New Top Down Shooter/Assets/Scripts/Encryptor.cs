using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;

namespace TimeStrike.SaveManagement.Serializers
{
    public static class Encryptor
    {
        /// <summary>
        /// Encrypts and returns a string using the System.Security.Cryptography.AES Encryptor.
        /// </summary>
        /// <param name="toEncrypt">The string to encrypt.</param>
        /// <param name="key">Key to encrypt the string with. Must be the same as decryption key.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncryptString(string toEncrypt, string key) 
        {
            byte[] initVector = new byte[16];
            byte[] dataArray;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = initVector;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memStream = new())
                {
                    using (CryptoStream crypStream = new(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new(crypStream))
                        {
                            writer.Write(toEncrypt);
                        }

                        dataArray = memStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(dataArray);
        }
        /// <summary>
        /// Decrypts an encrypted string using the System.Security.Cryptography.AES Decryptor.
        /// </summary>
        /// <param name="toDecrypt">The encrypted string which should be processed</param>
        /// <param name="key">The key that was used to encrypt the string.</param>
        /// <returns>The decrypted string.</returns>
        public static string DecryptString(string toDecrypt, string key) 
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(toDecrypt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}   