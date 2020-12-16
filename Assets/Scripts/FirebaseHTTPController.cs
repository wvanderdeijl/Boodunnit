using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class FirebaseHTTPController
    {
        private static readonly string apiKey = "AIzaSyC5lGUMD6hlJPbI7zkcWEU3aV_seCde3Wo";
        private static readonly string username = "drijvern@gmail.com";
        private static readonly string credentials = "B00D3n1t";
        private static string idToken;

        public static async void PostPlaythrough(Playthrough playthrough)
        {
            HttpClient client = new HttpClient();
            string content = JsonConvert.SerializeObject(new UserLoginCredentials(username, credentials));

            HttpResponseMessage responseMessage = await client.PostAsync(
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + apiKey,
                new StringContent(content));
            
            if (responseMessage.IsSuccessStatusCode)
            {
                string result = await responseMessage.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(result);
                idToken = jObject["idToken"].Value<string>();
                
                HttpResponseMessage msg = await client.PostAsync(
                    $"https://boodunnitcharts-default-rtdb.firebaseio.com/rest.json?auth={idToken}",
                    new StringContent(JsonConvert.SerializeObject(playthrough))
                );
                Console.WriteLine(msg.StatusCode);
            }
        }

        public class UserLoginCredentials
        {
            public string email;
            public string password;
            public bool returnSecureToken = true;

            public UserLoginCredentials(string email, string password)
            {
                this.email = email;
                this.password = password;
            }
        }
    }
}