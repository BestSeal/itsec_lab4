using System.Collections.Generic;
using System.IO;
using System.Text;

namespace itsec_lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            const string openPath = @"D:\1.bmp";
            const string savePath = @"D:\bmp.bmp";
            
            var key = new List<byte>(Encoding.UTF8.GetBytes("Какой-то очень крутой ключ!"));
            var message = new List<byte>(File.ReadAllBytes(openPath));

            var blowfish = new Blowfish(key);
            blowfish.Encrypt(message);
            blowfish.Decrypt(message);

            File.WriteAllBytes(savePath, message.ToArray());
        }
    }
}