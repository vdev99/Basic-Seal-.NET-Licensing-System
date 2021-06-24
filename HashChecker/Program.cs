using System;
using System.IO;
using System.Security.Cryptography;
using TextCopy;

namespace HashChecker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                HashAndCopy(args[0]);
            }
            else
            {
                Console.WriteLine("Please enter the file path of your application!");
                string filePath = Console.ReadLine();
                HashAndCopy(filePath);
            }
          
        }
        static void HashAndCopy(string path)
        {
            string hash = new MD5Hash().HashFile(path);

            Console.WriteLine("File hash:\t" + hash);
            ClipboardService.SetText(hash);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nHash is copied to the clipboard!");
            Console.ReadKey();
        }
    }
    class MD5Hash
    {
        public string HashFile(string path)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}
