using System.Collections.Generic;
using System.IO;
using System.Text;

namespace itsec_lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            const string openPath = @"D:\Karl_Vigers_Dzhoy_Bitti_-_Razrabotka_trebovaniy_k_programmnomu_obespecheniyu_3-e_izdanie_-_2014.pdf";
            const string savePath = @"D:\out.pdf";
            
            var key = new List<byte>(Encoding.UTF8.GetBytes("Какой-то очень крутой ключ!"));
            var message = new List<byte>(File.ReadAllBytes(openPath));

            var blowfish = new Blowfish(key);
            blowfish.Encrypt(message);
            blowfish.Decrypt(message);

            File.WriteAllBytes(savePath, message.ToArray());
        }
    }
}