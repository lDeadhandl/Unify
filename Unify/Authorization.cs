using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using System.Data;
using Accord.Statistics.Filters;
using Accord.Math;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Accord;

namespace Unify
{
    public class Authorization
    {        
        private readonly HttpClient _client;
        private readonly string _clientId = "e7b6b7ff0e444353aa5edfa5ea1728cb";
        private readonly string _clientSecret = "c0a6951732fa4aeb82f9975902ac6ff1";
        private readonly string _redirectUri = "https://example.com/callback";
        public List<string> Tracks { get; set; }
        public List<string> TargetList { get; set; }
        public List<int> TargetValues { get; set; }
        public List<AudioFeatures> audioFeatures { get; set; }
        public List<string> TargetListIds { get; set; }

        public Authorization()
        {
            //Used throughout the class
            _client = new HttpClient();

            //Used in GetTracks Function    
            //NOTE: *** Currently a list of Ids instead of names due to functionality need of GetFeatures function ***
            Tracks = new List<string>(); 

            audioFeatures = new List<AudioFeatures>();

            //Used in Comparator function 
            TargetList = new List<string>();
            TargetValues = new List<int>();
            TargetListIds = new List<string>();

        }

        //Authorizes the users account when given the code 
        public async Task SetAuthorization(string code)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
            });

            var g = await _client.PostAsync("https://accounts.spotify.com/api/token", content);
            var response = await g.Content.ReadAsStringAsync();
            Console.WriteLine(response);
            var auth = JsonConvert.DeserializeObject<AuthParams>(response);
            Console.WriteLine(auth.TokenType);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        }

        //TO DO: Need to generalize the parameter so it accepts a number that corresponds to the total
        //number of songs the user has
        //NOTE: *** Potenially may have to add a comparison function based off of individual data sizes so one user isn't favored over all others ***
        public async Task GetTracks(int x)
        {
            //Creates list of tasks that grab (50 * i) tracks                 
            var trackCall = Enumerable.Range(0, x).Select(i => _client.GetAsync("https://api.spotify.com/v1/me/tracks?limit=50&offset=" + (i * 50))).ToList();
            var tracksGrab = await Task.WhenAll(trackCall);

            foreach (var item in tracksGrab)
            {
                var tracksContent = await item.Content.ReadAsStringAsync();
                var tracks = JsonConvert.DeserializeObject<Paging<SavedTrack>>(tracksContent);
                var Swag = tracks.Items.Select(z => z.Track.Id).ToList();
                Tracks.AddRange(Swag);
            }
        }

        //
        public async Task GetAudioFeatures(List<string> Ids)
        {
            foreach (var item in Ids)
            {
                var l = await _client.GetAsync("https://api.spotify.com/v1/audio-features?ids=" + item);
                var featuresResponse = await l.Content.ReadAsStringAsync();
                var features = JsonConvert.DeserializeObject<AudioFeaturesObject>(featuresResponse);
                audioFeatures.AddRange((features.AudioFeatures.Select(z => z)).ToList());
            }
        }

        //TO DO: hard coded right now for 2 lists, generalize this to take any amount
        //of lists that coincides with the number of users in the party 
        //NOTE: *** Can be done with use of params[] List<T> then indexing list number in function ***
        public void Comparator(List<string> Stef, List<string> Chris)
        {
            //Creates temporary combined list of songs to be used later in function
            var FullList = new List<string>();
            FullList.AddRange(Stef);
            FullList.AddRange(Chris);

            //Creates list of unmatched items: Target0
            var Target0 = new List<string>();
            var firstNotSecond = Stef.Except(Chris).ToList();
            var secondNotFirst = Chris.Except(Stef).ToList();
            Target0.AddRange(firstNotSecond);
            Target0.AddRange(secondNotFirst);

            var Target1 = FullList.Except(Target0).ToList();

            //List of Songs that match(1) and do not match(0) with 1's being first in the list 
            TargetList.AddRange(Target1);
            TargetList.AddRange(Target0);

            var partition = Partition<string>(TargetList, 100);

            //Creates comma separated list of Ids to be used in GetFeatures function
            foreach (var item in partition)
            {
                TargetListIds.Add(string.Join(',', item.Select(e => e)));
            }

            //Adds to List of Target values that corresponds with the order of TargetList
            TargetValues.AddRange(Enumerable.Repeat(1, Target1.Count()));
            TargetValues.AddRange(Enumerable.Repeat(0, Target0.Count()));
        }

        //Takes a list and breaks it into a list of smaller lists with a specified size.
        //This was used to break up the target tracklist down into lists of 100 due to the GET audio features API limit
        public static IEnumerable<List<T>> Partition<T>(IList<T> source, Int32 size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }


        //***First Attempt: This decision tree was looking for input values that were already existing from the creation of the tree 
        //this does not comply with this situation as we need to input unknown values that can be predicted based on proximity to the 
        //existing input values in the tree.
        //This tree requires a set amount of potential inputs which in this case is very high for each feature
        //TO DO: Research what tree can suit this situation(Look at iris example)
        public void DecisionTree()
        {

            // In this example, we will process the famous Fisher's Iris dataset in 
            // which the task is to classify weather the features of an Iris flower 
            // belongs to an Iris setosa, an Iris versicolor, or an Iris virginica:
            // 
            //  - https://en.wikipedia.org/wiki/Iris_flower_data_set
            // 

            DataTable data = new DataTable("Mitchell's Tennis Example");
            data.Columns.Add("Name", "Acousticness", "Danceability", "Energy", "Instrumentalness", "Liveness", "Loudness", "Speechiness", "Tempo", "Valence", "Target");

            foreach (var item in TargetList)
            {
                data.Rows.Add(item.ToString());
            }

            for (int i = 0; i < audioFeatures.Count; i++)
            {

                data.Rows[i]["Acousticness"] = audioFeatures[i].Acousticness;
                data.Rows[i]["Danceability"] = audioFeatures[i].Danceability;
                data.Rows[i]["Energy"] = audioFeatures[i].Energy;
                data.Rows[i]["Instrumentalness"] = audioFeatures[i].Instrumentalness;
                data.Rows[i]["Liveness"] = audioFeatures[i].Liveness;
                data.Rows[i]["Loudness"] = audioFeatures[i].Loudness;
                data.Rows[i]["Speechiness"] = audioFeatures[i].Speechiness;
                data.Rows[i]["Tempo"] = audioFeatures[i].Tempo;
                data.Rows[i]["Valence"] = audioFeatures[i].Valence;
            }

            for (int i = 0; i < TargetValues.Count; i++)
            {
                data.Rows[i]["Target"] = TargetValues[i].ToString();
            }




            var codebook = new Codification(data);

            //// Translate our training data into integer symbols using our codebook:
            DataTable symbols = codebook.Apply(data);
#pragma warning disable CS0618 // Type or member is obsolete
            double[][] inputs = symbols.ToArray<double>("Acousticness", "Danceability", "Energy", "Instrumentalness", "Liveness", "Loudness", "Speechiness", "Tempo", "Valence");
#pragma warning restore CS0618 // Type or member is obsolete
            int[] outputs = symbols.ToArray<int>("Target");            

            // Create a teaching algorithm:
            var teacher = new C45Learning()
            {
                    new DecisionVariable("Acousticness",     DecisionVariableKind.Continuous),
                    new DecisionVariable("Danceability", DecisionVariableKind.Continuous),
                    new DecisionVariable("Energy",    DecisionVariableKind.Continuous),
                    new DecisionVariable("Instrumentalness",        DecisionVariableKind.Continuous),
                    new DecisionVariable("Liveness",        DecisionVariableKind.Continuous),
                    new DecisionVariable("Loudness",        DecisionVariableKind.Continuous),
                    new DecisionVariable("Speechiness",        DecisionVariableKind.Continuous),
                    new DecisionVariable("Tempo",        DecisionVariableKind.Continuous),
                    new DecisionVariable("Valence",        DecisionVariableKind.Continuous)
            };

            // Use the learning algorithm to induce a new tree:
            DecisionTree tree = teacher.Learn(inputs, outputs);

            // To get the estimated class labels, we can use
            //int[] predicted = tree.Decide(inputs);

            // The classification error (0.0266) can be computed as 
            //double error = new ZeroOneLoss(outputs).Loss(predicted);

            int[] query = codebook.Transform(new[,]
            {
                { "Valence", "0.37"},
                { "Acousticness", "0.00187"},
                { "Danceability", "0.808"},
                { "Energy", "0.626"},
                { "Instrumentalness", "0.159"},
                { "Liveness", "0.376"},
                { "Loudness", "-12.733"},
                { "Speechiness", "0.168"},
                { "Tempo", "123.99"}

            });

            // And then predict the label using
            int predicted = tree.Decide(query);  // result will be 0

            // We can translate it back to strings using
            string answer = codebook.Revert("Target", predicted); // Answer will be: "No"

            Console.WriteLine(predicted);
            //// Moreover, we may decide to convert our tree to a set of rules:
            //DecisionSet rules = tree.ToRules();

            //// And using the codebook, we can inspect the tree reasoning:
            //string ruleText = rules.ToString(codebook, "Output",
            //    System.Globalization.CultureInfo.InvariantCulture);



            //var id3learning = new ID3Learning()
            //{
            //    new DecisionVariable("Acousticness",     TargetList.Count), 
            //    new DecisionVariable("Danceability", TargetList.Count),
            //    new DecisionVariable("Energy",    TargetList.Count),  
            //    new DecisionVariable("Instrumentalness",        TargetList.Count),  
            //    new DecisionVariable("Liveness",        TargetList.Count), 
            //    new DecisionVariable("Loudness",        TargetList.Count),  
            //    new DecisionVariable("Speechiness",        TargetList.Count),  
            //    new DecisionVariable("Tempo",        TargetList.Count),  
            //    new DecisionVariable("Valence",        TargetList.Count) 
            //};

            //// Learn the training instances!
            //DecisionTree tree = id3learning.Learn(inputs, outputs);

            //// Compute the training error when predicting training instances
            //double error = new ZeroOneLoss(outputs).Loss(tree.Decide(inputs));

            //int[] query = codebook.Transform(new[,]
            //{
            //    { "Acousticness", "0.011"},
            //    { "Danceability", "0.905"},
            //    { "Energy", "0.905"},
            //    { "Instrumentalness", "0.000905"},
            //    { "Liveness", "0.302"},
            //    { "Loudness", "-2.743"},
            //    { "Speechiness", "0.103"},
            //    { "Tempo", "114.944"},
            //    { "Valence", "0.625"}
            //});

            //// And then predict the label using
            //int predicted = tree.Decide(query);  // result will be 0

            //// We can translate it back to strings using
            //string answer = codebook.Revert("Target", predicted); // Answer will be: "No"

            //Console.WriteLine(predicted);
        }
    }
}
