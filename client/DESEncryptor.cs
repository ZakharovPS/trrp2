using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Client
{
    static class DESEncryptor
    {
        public static byte[] DESEncrypt(byte[] DataToEncrypt, DESCryptoServiceProvider des)
        {
            try
            {
                byte[] encryptedData;
                ICryptoTransform encryptor = des.CreateEncryptor();
                encryptedData = encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
