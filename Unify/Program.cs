using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Unify
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the default serialization
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
            };

            Run().Wait();
        }

        public static async Task Run()
        {

            var clientId = "e7b6b7ff0e444353aa5edfa5ea1728cb";
            var clientSecret = "c0a6951732fa4aeb82f9975902ac6ff1";
            var redirectUri = "https://example.com/callback";

            //var auth = new HttpClient();
            //var x = await auth.GetAsync("https://accounts.spotify.com/authorize/?client_id=e7b6b7ff0e444353aa5edfa5ea1728cb&response_type=code&scope=user-library-read&redirect_uri=https://example.com/callback");
            //Console.WriteLine(x.Content);



            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", "BQCZPUppz8CP7LTZFNrUZcWlOtoUCVm5EZFzIxnALeZrJjSECJavhwkP"),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                });
                var g = await client.PostAsync("https://accounts.spotify.com/api/token", content);

                var response = await g.Content.ReadAsStringAsync();
                Console.WriteLine(response);

                var auth = JsonConvert.DeserializeObject<AuthParams>(response);

                Console.WriteLine(auth.TokenType);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

                var get = await client.GetAsync("https://api.spotify.com/v1/me/tracks");

                var tracks = await get.Content.ReadAsStringAsync();
                dynamic z = JsonConvert.DeserializeObject(tracks);

                foreach (var item in z)
                {
                    Console.WriteLine(z.items);
                }
            }

        }

        //public static async Task Run2()
        //{
        //    var s = new SpotifyAPI.Web.SpotifyWebAPI();
        //    var g = new SpotifyAPI.Web.SpotifyWebBuilder(); 

            

        //    WebAPIFactory webApiFactory = new WebAPIFactory(
        //                             "https://example.com/callback",
        //                             8000,
        //                             "e7b6b7ff0e444353aa5edfa5ea1728cb",
        //                             Scope.UserReadPrivate
        //                        );

        //    SpotifyAPI.Web.SpotifyWebAPI _spotify = null;

        //    try
        //    {
        //        //This will open the user's browser and returns once
        //        //the user is authorized.
        //        _spotify = await webApiFactory.GetWebApi();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    if (_spotify == null)
        //        return;
        //}
    }
}
