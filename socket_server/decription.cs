using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace socket_server
{
    class EncryptObj
    {
        public string encrypted;
        public string AesKey;
        public string AesIV;
    }

    class decryption
    {
        public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }

        public static string unscramble(string EncryptedJson, RSAParameters pRSA)
        {
            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {
                EncryptObj encrypted = new EncryptObj();

                JObject jsonData = JObject.Parse(EncryptedJson);
                //JArray items = (JArray)jsonData["{"];

                
              //  foreach (JToken item in items)
              //  {
                    encrypted.encrypted = jsonData["encrypted"].ToString();
                    encrypted.AesKey = jsonData["AesKey"].ToString();
                encrypted.AesIV = jsonData["AesIV"].ToString();
                // }
                byte[] cipherText = Convert.FromBase64String(encrypted.encrypted);
                byte[] Key = Convert.FromBase64String(encrypted.AesKey);
                Key = RSADecrypt(Key, pRSA , false);
                byte[] IV = Convert.FromBase64String(encrypted.AesIV);

                 

                string roundtrip = DecryptStringFromBytes_Aes(cipherText, Key, IV);
               // Console.WriteLine("результат функции unscramble ", roundtrip);
                //Console.WriteLine("***********************");
                //Console.WriteLine("***********************");

                return Convert.ToString(roundtrip);
            }
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

                // Create a decryptor to perform the stream transform.
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
    }
}