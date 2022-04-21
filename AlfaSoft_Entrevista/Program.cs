using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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
            CreateFileWithResults(response);

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

        protected static void CreateFileWithResults(List<HttpResponseMessage> httpResponses)
        {
            var fileName = "\\Log.txt";
            var enviroment = Environment.CurrentDirectory;
            string directory = Directory.GetParent(enviroment).Parent.FullName + fileName;
            var infoString = HttpResponsesToString(httpResponses);

            try
            {
                using (FileStream fs = File.Create(directory))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(infoString);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected static string HttpResponsesToString(List<HttpResponseMessage> httpResponses)
        {
            string result = string.Empty;
            foreach (var httpReponse in httpResponses)
            {
                result += httpReponse.ToString() + "\r\n";
            }

            return result;
        }
    }
}
