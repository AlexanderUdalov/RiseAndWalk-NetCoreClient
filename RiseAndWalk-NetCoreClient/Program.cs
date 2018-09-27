using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RiseAndWalk_NetCoreClient
{
    class RegisterData
    {
        public string Email;
        public string Password;
    }

    class Alarm
    {
        public Guid AlarmId { get; set; }
        public string UserName { get; set; }
        public DateTime AlarmTime { get; set; }
        public bool RepeatEveryWeek { get; set; }
        public string Description { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                var input = "";
                while (!(input = Console.ReadLine().ToLower()).Equals("exit"))
                {
                    switch (input)
                    {
                        case "get":
                            {
                                Get(client);
                                break;
                            }
                        case "post":
                            {
                                Post(client);
                                break;
                            }
                        case "deleteatindex":
                            {
                                DeleteAtIndex(client);
                                break;
                            }
                        case "deleteall":
                            {
                                DeleteAll(client);
                                break;
                            }
                        case "login":
                            {
                                Login(client);
                                break;
                            }
                        case "register":
                            {
                                Register(client);
                                break;
                            }
                        case "logout":
                            {
                                Logout(client);
                                break;
                            }
                    }
                    Console.WriteLine("Enter command: ");
                }
            }
        }
        
        private static void Get(HttpClient client)
        {
            var response = client.GetAsync("https://localhost:44343/api/alarms").Result;
            
            Console.WriteLine("Status code: " + response.StatusCode);
            var stringContent = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(stringContent);

            List<Alarm> alarms = JsonConvert.DeserializeObject<List<Alarm>>(stringContent)?? new List<Alarm>();
            
            foreach (var alarm in alarms)
            {
                Console.WriteLine($"(\n\tAlarmTime: {alarm.AlarmTime},\n\tDescription: {alarm.Description}/n)");
            }
        }
        private static void Post(HttpClient client)
        {
            Console.WriteLine("Enter description: ");
            var description = Console.ReadLine();

            StringContent content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        AlarmTime = DateTime.Now,
                        RepeatEveryWeek = true,
                        Description = description
                    }),
                    Encoding.UTF8,
                    "application/json");

            var postResponse = client.PostAsync("https://localhost:44343/api/alarms/", content).Result;


            Console.WriteLine("Status code: " + postResponse.StatusCode);
            var responseContent = postResponse.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseContent);
        }

        private static void DeleteAtIndex(HttpClient client)
        {
            Console.WriteLine("Enter index: ");
            var index = Console.ReadLine();

            var response = client.DeleteAsync("https://localhost:44343/api/alarms/" + index).Result;

            Console.WriteLine("Status code: " + response.StatusCode);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseContent);
        }

        private static void DeleteAll(HttpClient client)
        {
            var response = client.DeleteAsync("https://localhost:44343/api/alarms").Result;

            Console.WriteLine(response.StatusCode);
            var responseContent = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseContent);
        }

        private static void Register(HttpClient client)
        {
            Console.WriteLine("Enter email");
            var email = Console.ReadLine();
            Console.WriteLine("Enter password");
            var password = Console.ReadLine();

            var stringContent = new StringContent(
                JsonConvert.SerializeObject(
                    new RegisterData()
                    {
                        Email = email,
                        Password = password
                    }),
                Encoding.UTF8,
                "application/json");

            var postResponse = client.PostAsync("https://localhost:44343/api/account/register",
                stringContent).Result;

            Console.WriteLine("Status code: " + postResponse.StatusCode);
            var content = postResponse.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);

            if (postResponse.IsSuccessStatusCode)
            {
                var token = content;
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }

        private static void Login(HttpClient client)
        {
            Console.WriteLine("Enter email");
            var email = Console.ReadLine();
            Console.WriteLine("Enter password");
            var password = Console.ReadLine();

            var stringContent = new StringContent(
                JsonConvert.SerializeObject(
                    new RegisterData()
                    {
                        Email = email,
                        Password = password
                    }),
                Encoding.UTF8,
                "application/json");

            var postResponse = client.PostAsync("https://localhost:44343/api/account/login",
                stringContent).Result;

            Console.WriteLine("Status code: " + postResponse.StatusCode);
            var content = postResponse.Content.ReadAsStringAsync().Result;
            Console.WriteLine(content);

            if (postResponse.IsSuccessStatusCode)
            {
                var token = content;
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }

        private static void Logout(HttpClient client)
        {
            if (client.DefaultRequestHeaders.Remove("Authorization"))
                Console.WriteLine("Success!");
            else Console.WriteLine("Error!");
        }
        
        
        public static void DeleteAlarm(Alarm alarm, HttpClient client)
        {
            var _response = client.DeleteAsync("https://localhost:44343/api/alarms/" + alarm.AlarmId);


            var stringContent = _response;

            Console.WriteLine(stringContent + "\n\n\n");
        }
        
    }
}
