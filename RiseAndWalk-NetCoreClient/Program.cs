using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace RiseAndWalk_NetCoreClient
{
    class RegisterData
    {
        public string Email;
        public string Password;
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                GetValues(client);

                var registerData = new RegisterData()
                {
                    Email = "udalov@mail.ru",
                    Password = "A1a$2a3a4a"
                };

                var bodyString = JsonConvert.SerializeObject(registerData);

                var body = new StringContent(bodyString, Encoding.UTF8, "application/json");

                var postResponse = client.PostAsync("https://localhost:44343/account/login", body).Result;
                var content = postResponse.Content.ReadAsStringAsync();

                var token = content.Result;

                Console.WriteLine(token);

                using (var authorizatedClient = new HttpClient())
                {
                    authorizatedClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    GetValues(authorizatedClient);
                }
            }
            Console.ReadLine();
        }

        private static void GetValues(HttpClient client)
        {
            try
            {
                var _response = client.GetStringAsync("https://localhost:44343/api/values").Result;
                Console.WriteLine(_response);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
