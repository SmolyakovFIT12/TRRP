using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace socket_client
{
    class EncryptObj
    {
        public string encrypted;
        public string AesKey;
        public string AesIV;
    }

    class encryption
    {

        RSACryptoServiceProvider rsaCrypto = new RSACryptoServiceProvider();
        public static string scramble(string original, RSAParameters rsaCrypto)
        {
            using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
            {
                byte[] AESKey = myAes.Key;
                byte[] AESIV = myAes.IV;

                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_Aes(original, AESKey, AESIV);

                EncryptObj EncriptedString = new EncryptObj();
                Console.WriteLine("AES Key: {0}", Convert.ToBase64String(myAes.Key));
                AESKey = RSAEncrypt(AESKey, rsaCrypto, false);
                Console.WriteLine("AES KeyRSA: {0}", Convert.ToBase64String(AESKey));

                EncriptedString.encrypted = Convert.ToBase64String(encrypted);
                EncriptedString.AesKey = Convert.ToBase64String(AESKey);
                EncriptedString.AesIV = Convert.ToBase64String(AESIV);

                string EncryptedJson = JsonConvert.SerializeObject(EncriptedString);


                //Display the original data and the decrypted data.
                Console.WriteLine("Оригинальные:   {0}", original);
                Console.WriteLine();
                Console.WriteLine("Зашифрованные:  {0}", Convert.ToBase64String(encrypted));
                Console.WriteLine();

                //Console.WriteLine("AES Key: {0}", Convert.ToBase64String(myAes.Key));
                //Console.WriteLine("AES Key: {0}", Convert.ToBase64String(AESKey));
                Console.WriteLine("Зашифрованный Json:   {0}", EncryptedJson);
                Console.WriteLine();
                return EncryptedJson;
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
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
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
        public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //toinclude the public key information.
                    RSA.ImportParameters(RSAKeyInfo);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            //Catch and display a CryptographicException to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }
    }
}
