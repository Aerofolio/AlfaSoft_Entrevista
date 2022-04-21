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
        private static readonly string LogFileName = "\\Log.txt";
        private static readonly string ConnectionStringAPI = "https://api.bitbucket.org/2.0/users/";
        private static readonly int MiliSecondsBetweenRequests = 5000;
        private static readonly int MiliSecondsBeforeColse = 5000;
        private static readonly int SecondsBetweenRequests = 60;
        public static void Main(string[] args)
        {
            if (CheckLastRequestTime())
            {
                List<string> users = GetFileAndUsers();
                var response = GetUsersFromAPI(users);
                CreateFileWithResults(response);
            }
            else
            {
                Console.WriteLine("One request was made less than 60 seconds ago.\r\nClosing application");
            }

            Thread.Sleep(MiliSecondsBeforeColse);
        }

        private static string GetFilePath()
        {
            Console.WriteLine("Insert the full path to a file containing users:");
            string filePath = Console.ReadLine();
            Console.Clear();

            return filePath;
        }

        private static List<string> GetUsers(string filePath)
        {
            List<string> users = new List<string>();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    var fileData = sr.ReadToEnd();
                    var fileLines = fileData.Replace("\r\n", "\n").Split('\n');
                    foreach (var line in fileLines)
                    {
                        users.Add(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return users;
        }
        private static List<string> GetFileAndUsers()
        {
            string filePath = GetFilePath();
            return GetUsers(filePath);
        }
        private static List<HttpResponseMessage> GetUsersFromAPI(List<string> users)
        {
            HttpClient client = new HttpClient();
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
            foreach (var user in users)
            {
                var response = GetUserFromAPI(user, client);
                Thread.Sleep(MiliSecondsBetweenRequests);
                response.Wait();
                responses.Add(response.Result);
                Console.WriteLine($"Response: {response.Result.StatusCode}\r\n");
            }

            return responses;
        }

        private static async Task<HttpResponseMessage> GetUserFromAPI(string user, HttpClient client)
        {
            var connectionString = ConnectionStringAPI + user;
            Console.WriteLine($"User: '{user}' is being retrived.\r\nURL: {connectionString}");
            return await client.GetAsync(connectionString);
        }

        private static void CreateFileWithResults(List<HttpResponseMessage> httpResponses)
        {
            var enviroment = Environment.CurrentDirectory;
            string directory = enviroment + LogFileName;
            var infoString = HttpResponsesToString(httpResponses);

            try
            {
                using (FileStream fs = File.Create(directory))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(infoString);
                    fs.Write(info, 0, info.Length);
                }
                Console.WriteLine($"Log file created at: {directory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string HttpResponsesToString(List<HttpResponseMessage> httpResponses)
        {
            string result = string.Empty;
            foreach (var httpReponse in httpResponses)
            {
                result += httpReponse.Headers.Date.ToString() +
                    " Method: " + httpReponse.RequestMessage.Method.ToString() + ", RequestRequestUri: " + httpReponse.RequestMessage.RequestUri.ToString() +
                    "\r\n" + httpReponse.ToString() + ".\r\n";
            }

            return result;
        }

        private static DateTime GetLogFileDate()
        {
            var enviroment = Environment.CurrentDirectory;
            string directory = enviroment + LogFileName;
            DateTime lastWriteTime = new DateTime();
            try
            {
                lastWriteTime = File.GetLastWriteTime(directory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return lastWriteTime;
        }

        private static bool CheckLastRequestTime()
        {
            var lastRequestTime = GetLogFileDate();
            var difference = DateTime.Now - lastRequestTime;
            return difference.TotalSeconds >= SecondsBetweenRequests;
        }
    }
}
