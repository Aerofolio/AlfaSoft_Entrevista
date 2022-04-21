using System;

namespace AlfaSoft_Entrevista
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetFilePath();
        }

        protected static string GetFilePath()
        {
            Console.WriteLine("Insert the full path to a file containing users:");
            string filePath = Console.ReadLine();

            return filePath;
        }
    }
}
