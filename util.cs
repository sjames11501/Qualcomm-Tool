using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;

namespace QCOH
{
    public sealed class util
    {

        public static ulong HexLiteral2Unsigned(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentException("hex");

            int i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X') ? 2 : 0;
            ulong value = 0;

            while (i < hex.Length)
            {
                uint x = hex[i++];

                if (x >= '0' && x <= '9') x = x - '0';
                else if (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else throw new ArgumentOutOfRangeException("hex");

                value = 16 * value + x;

            }

            return value;
        }
        public static byte[] StringToByteArray(String hex) // HEX STRING TO BYTE ARRAY
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static byte[] getAsciiLiteral(byte[] bytes)
        {
            int len = bytes.Length;
            string[] strArray = new string[len];
            byte[] finalByte = new byte[32];
            int i = 0;
            int i2 = 0;
            int i3 = 0;
            int offset = 0;
            var hexFinalString = "0x";


         
            foreach (byte b in bytes)
            {
                strArray[i] = b.ToString("X2");
                i++;
            }

            i = 0;
            i3 = 0;

            if(len >5)//len>5, only use the last 4 {
            {
                while (true)
                {
                    if(offset + 4 == len)
                    {
                        break;
                    }
                    else
                    {
                        offset++;
                    }
                }

            }
            while (true)
            {

                if (offset == 32)
                    break;

                if (offset == len)
                    break;
              
                hexFinalString += strArray[offset];
                offset++;
                i3++;
            }



     

            var x = HexLiteral2Unsigned(hexFinalString);
            var ret = Convert.ToInt32(x);
            byte[] bA = BitConverter.GetBytes(x);
           
            return bA;
                
        }
        public static  bool Equality(byte[] a1, byte[] b1)
        {
            int i = 0;
            if (a1.Length == b1.Length)
            {
                while ((i < a1.Length) && (a1[i] == b1[i]))
                {
                    i++;
                }
            }
            return i == a1.Length;
        }
        public static byte[] stringHexArrayToByteArray(string[] x, int len)
        {
            var bytes = new List<byte[]>();
     

            foreach(var item in x)
            {

                if(item == "") { continue; }
                try
                {
                    var currentBytes = HexToByte(item);
                    bytes.Add(currentBytes);
                }
                catch(Exception e)
                {

                }
               

            }
            var xx = bytes.SelectMany(a => a).ToArray();


            return xx;
        }
        public static string ByteArrayToString(byte[] ba)
        {
          var ret= "";
            foreach (byte b in ba)
            {
                var b1 = new byte[] { b };
                var x = BitConverter.ToString(b1, 0);
                var o = "";

                if(ret == "")
                {
                    o = "0x" + x;

                }
                else
                {
                    o = " 0x" + x;
                }
                ret += o;
            }
             

                return ret;
        }
        public static byte[] HexToByte(string hexString)
        {
            int lengthCount = 0;

            // offset value is 2 for removing first two characters '0x' from hexadecimal string
            int offset = 2;
            int byteLength = 0;

            // byte array length will be half of hexadecimal string length
            byte[] bytes = new byte[(hexString.Length - offset) / 2];
            byteLength = bytes.Length;
            for (lengthCount = 0; lengthCount < byteLength; lengthCount++)
            {
                // Adding two nybble from hexadecimal string to create one byte
                bytes[lengthCount] = (byte)((int.Parse(hexString[offset].ToString(), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture) << 4) | int.Parse(hexString[offset + 1].ToString(), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                offset += 2;
            }

            return bytes;
        }
        public static string getFilePath(string inputFileName, string extension)
        {
            int i = 0;
            var fileName = "";
            
            
            if(!File.Exists("/" + inputFileName))
            {
                return "/" + inputFileName + extension;
            }

            while (true)
            {
                if (File.Exists(inputFileName + "_" + i + extension))
                {
                    continue;
                }
                else
                {
                    return "/" + inputFileName + "_" + i + extension;
                }
            }
        }
        public static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        public static int SearchBytes(byte[] haystack, byte[] needle)
        {
            var len = needle.Length;
            var limit = haystack.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (needle[k] != haystack[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

       

    }
}
