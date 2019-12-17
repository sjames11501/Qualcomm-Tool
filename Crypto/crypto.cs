using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace QCOH.Crypto
{
    public class encryptionReqs
    {
        public byte[] encryptedSerial { get; set; }
        public byte[] encryptedSerial2 { get; set; }
    
    }
    
    public class cryptoUtil
    {
        public encryptionReqs tEncrypt(byte[]serial)
        {
            //var master_aes_key = "3g!c$Db6k~-nWc~J";
            var xor_mask = "d$ati#m*q@7,kTMl";
            var master_aes_key = "$Db63g!ck~-nWc~J";

            var master_key_bytes = System.Text.Encoding.ASCII.GetBytes(master_aes_key);
            var xor_mask_bytes = System.Text.Encoding.ASCII.GetBytes(xor_mask);
            //1. Perform SHA-256 on the serial***************************************************/
            var hashedSerial = doSHA256(serial);
            //2. XOR WITH MASK, before encryption.
            var xorSerial = xor(hashedSerial.Take(16).ToArray(), xor_mask_bytes, 16);
            //3. Perform AES, using AES master key
            var encrypted_Serial = encrypt(xorSerial,master_key_bytes);
            /***********************************************************************************/
            /***********************************************************************************/
            //4. "serial_2" is the last 16 bytes of hashed serial*******************************/
            var serial_2 = hashedSerial.Skip(16).Take(16).ToArray();
            //5. XOR WITH MASK,before encryption.
            var xor_serial_2 = xor(serial_2, xor_mask_bytes,16);
            //6. Perform AES, using master key
            var encrypted_Serial_2 = encrypt(xor_serial_2, master_key_bytes);
            /***********************************************************************************/

            encryptionReqs x = new encryptionReqs();
            x.encryptedSerial = encrypted_Serial;
            x.encryptedSerial2 = encrypted_Serial_2;
            return x;
        }
        public byte[] encrypt(byte[] input, byte[] key)
        {
            var myRijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                KeySize = 128,
                BlockSize = 128
            };

            var IV = Encoding.ASCII.GetBytes("");
            var encryptor = myRijndael.CreateEncryptor(key, IV);
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            var toEncrypt = (input);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            var encryptedData = msEncrypt.ToArray();
            return encryptedData;
        }
        public byte[] decrypt(byte[] input, byte[] key) 
        {
            var myRijndael = new RijndaelManaged()
            {
                Padding = PaddingMode.None,
                Mode = CipherMode.ECB,
                KeySize = 128,
                BlockSize = 128
            };

            var IV = Encoding.ASCII.GetBytes("");
            var decryptor = myRijndael.CreateDecryptor(key, IV);
            var msDecrypt = new MemoryStream();
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);
            var toDecrypt = input;
            csDecrypt.Write(toDecrypt, 0, toDecrypt.Length);
            csDecrypt.FlushFinalBlock();
            var x = msDecrypt.ToArray();
            return x;
        }
        public byte[] xor(byte[] input, byte[] mask, int length)
        {
            for (int i = 0; i < length; i++)
            {
                input[i] = (byte)(input[i] ^ mask[i]);
            }
            return input;
        }
        public byte[] doSHA256(byte[] input)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            var hashValue = mySHA256.ComputeHash(input);
            return hashValue;
        }
    }
}
