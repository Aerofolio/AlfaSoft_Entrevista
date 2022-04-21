using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AlfaSoft_Entrevista
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> users = GetFileAndUsers();
            var response = GetUsersFromAPI(users);
            Console.ReadLine();
        }

        protected static string GetFilePath()
        {
            Console.WriteLine("Insert the full path to a file containing users:");
            string filePath = Console.ReadLine();

            return filePath;
        }

        protected static List<string> GetUsers(string filePath)
        {
            List<string> users = new List<string>();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    var fileData = sr.ReadToEnd();
                    fileData = fileData.Replace("\r\n", "\n");
                    var fileLines = fileData.Split('\n');
                    foreach (var line in fileLines)
                    {
                        users.Add(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
            return users;
        }
        protected static List<string> GetFileAndUsers()
        {
            string filePath = GetFilePath();
            return GetUsers(filePath);
        }
        protected static List<HttpResponseMessage> GetUsersFromAPI(List<string> users)
        {
            HttpClient client = new HttpClient();
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
            foreach (var user in users)
            {
                var response = GetUserFromAPI(user, client);
                Thread.Sleep(5000);
                response.Wait();
                responses.Add(response.Result);
            }

            return responses;
        }

        protected static async Task<HttpResponseMessage> GetUserFromAPI(string user, HttpClient client)
        {
            return await client.GetAsync("https://api.bitbucket.org/2.0/users/" + user);

        }
    }
}
