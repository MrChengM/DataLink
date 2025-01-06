using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utillity.File
{
    public class FileBinary
    {

        public  static byte[] Read(string fileName)
        {
            byte[] result;
            using (FileStream fileStream=new FileStream(fileName,FileMode.Open,FileAccess.Read))
            {
                result = new byte[fileStream.Length];
                fileStream.Read(result, 0, result.Length);
            }

            return result;
        }
        public static Stream ReadStream(string fileName)
        {
            byte[] temp = new byte[1024];
            Stream stream = new MemoryStream();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(stream);
            }

            return stream;
        }
        public static void Write(string fileName,byte[] sourceByte)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.Write(sourceByte, 0, sourceByte.Length);
            }
        }
        public static void Write(string fileName, Stream stream)
        {

            string filePath = fileName;
            byte[] dataBytes = new byte[1024];
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                int number = 0;
                while (true)
                {
                    number = stream.Read(dataBytes, 0, dataBytes.Length);
                    fileStream.Write(dataBytes, 0, number);
                    //Console.WriteLine(number);
                    if (number == 0)
                    {
                        break;
                    }
                }
            }
            stream.Close();
        }
    }
}
