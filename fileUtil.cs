using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QCOH
{
    class fileUtil
    {
        public static void clearFile(string path)
        {
           
            if (File.Exists(path))
            {
           
                File.Delete(path);
            }
        }

        public static bool appendToFile(string path, byte[] BUFFER)
        {
            try
            {

                using (var stream = new FileStream(path, FileMode.Append))
                {
                    stream.Write(BUFFER, 0, BUFFER.Length);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
