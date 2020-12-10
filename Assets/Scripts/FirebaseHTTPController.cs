using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class FirebaseHTTPController
    {
        private string apiKey = "AIzaSyC5lGUMD6hlJPbI7zkcWEU3aV_seCde3Wo";
        private string username = "drijvern@gmail.com";
        private string credentials = "B00D3n1t";
        
        public FirebaseHTTPController PlaythroughLog = new FirebaseHTTPController();
        private static FirebaseHTTPController _instance;
        public static FirebaseHTTPController Instance { get => _instance; }

        static FirebaseHTTPController() => _instance = new FirebaseHTTPController();

        public IEnumerator LoginPlaythroughDetails(Playthrough playthrough) 
        {
            UnityWebRequest login = UnityWebRequest.Post("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + apiKey,
                JsonConvert.SerializeObject(new UserLoginCredentials(username, credentials)));
            yield return login.SendWebRequest();

            if (login.isNetworkError || login.isHttpError)
            {
                Debug.Log(login.error);
            }
            else
            {
                // login.GetResponseHeaders()["idToken"];
            }
        }
        //
        // public IEnumerator PostPlaythrough()
        // {
        //     UnityWebRequest post = UnityWebRequest.Post();
        // }

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