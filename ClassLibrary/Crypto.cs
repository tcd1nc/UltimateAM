﻿using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AssetManager
{
    public static class Crypto
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tcd1am9+2geM0%s7";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        //Encrypt
        //public static string EncryptString(string plainText, string passPhrase)
        //{
        //    byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        //    byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        //    PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
        //    byte[] keyBytes = password.GetBytes(keysize / 8);
        //    RijndaelManaged symmetricKey = new RijndaelManaged();
        //    symmetricKey.Mode = CipherMode.CBC;
        //    ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        //    MemoryStream memoryStream = new MemoryStream();
        //    CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        //    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        //    cryptoStream.FlushFinalBlock();
        //    byte[] cipherTextBytes = memoryStream.ToArray();
        //    memoryStream.Close();
        //    cryptoStream.Close();
        //    return Convert.ToBase64String(cipherTextBytes);
        //}
        //Decrypt
        public static string DecryptString(string cipherText, string passPhrase)
        {
            
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };
            byte[] plainTextBytes;
            int decryptedByteCount;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(cipherTextBytes);                    
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    plainTextBytes = new byte[cipherTextBytes.Length];
                    decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    memoryStream = null;
                }                    
            }
            catch
            {
                try
                {
                    if (memoryStream != null)
                        memoryStream.Dispose();                    
                }
                catch { }
                return string.Empty;
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Dispose();
            }

            string output = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return output.Replace(@"\\", @"\");
           
            //return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}
