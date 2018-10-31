using System;
using System.Collections.Generic;
using System.Linq;
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
                    new KeyValuePair<string, string>("code", "AQDOIRURHNlq9aFBiRWXxJ8DWLepOphhno3iDWqFxar5s8i91-iOAZTGtk6zhMVK1UncLKNVQue18TLdBA-5e0SR7UhHdDNYRnQzS298dd25FutYUigSkxrt_N_NmYkt7Nf7u6C_cHUqBIlvwX616z5W6rfkTf3xX7TqwUPv3JF9WTLytcNE9P-ow6JXnBvd5V-CTnYHgYOum751bUSGeeIGBVQl3Co"),
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

                var tracksContent = await get.Content.ReadAsStringAsync();

                var tracks = JsonConvert.DeserializeObject<Paging<SavedTrack>>(tracksContent);
                
                Console.WriteLine(string.Join('\n', tracks.Items.Select(x => x.Track.Name)));

                int count = tracks.Total;
                var songIds = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    // get ids by groups of 00
                    var ids = tracks.Items.Select(x => x.Track.Id).Skip(100 * i).Take(count);

                    // create comma separated list                  
                    var q = string.Join(',', ids);
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
