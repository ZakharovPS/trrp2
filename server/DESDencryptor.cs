using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    static class DESDencryptor
    {
        public static byte[] DESDecrypt(byte[] DataToDecrypt, DES des)
        {
            try
            {
                byte[] decryptedData;
                ICryptoTransform decryptor = des.CreateDecryptor();
                decryptedData = decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
