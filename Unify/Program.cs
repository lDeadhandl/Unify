using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Accord.MachineLearning.DecisionTrees;
using System.Data;
using Accord.Statistics.Filters;
using Accord.Math;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math.Optimization.Losses;
using Accord;

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
            //Console.WriteLine(x.Content);
            var Stef = new Authorization();
            var Chris = new Authorization();

            await Chris.SetAuthorization("AQAVWY-wd8y1XiEQ0k0PNmRjt1T_EodsXAS2GHRPr_xSaxSd4drOrAgnsjzceCEoFqX3G2TK_4yC9UPj8ry45XEQrqBZWoZ_D9Q0oO213p3D2T_6OL9FTuI6Or1AleexRJZ280Aarz-QPiXClBdu6RUY6SqQ7IQ1fSr3CJdYbj0j6VfrU-t_GfqYwemd0JX4ZPGbaPpo94vFNqZDl1sTL-kztl9Euj8");
            await Stef.SetAuthorization("AQCiHdTAgUgHyCeLPFgvqMMovjMziLKDWLlPP6SyWBqrCfDl_BSAksQnlWRpYS5X0XIO6E1JgDYa7GDg4ST98KcScbTN3_JntmYwZ0xo7j56KDch00g3lS0f3Iw1YHGE3VhaybgFhFdAUjm_7iG7dJ6ciapa_SNPz-Xocx5_Tt-_UIPO-bma-Jgh990veOLd3l2uXTyjBzQ2g5C304lWDnoS6WlhmxM");
            
            await Stef.GetTracks(6);
            await Chris.GetTracks(6);

            Stef.Comparator(Stef.Tracks, Chris.Tracks);

            //var stef = string.Join('\n', Stef.Tracks);
            //var chris = string.Join('\n', Chris.Tracks); 
            var list = Stef.TargetList;
            var list1 = Stef.TargetValues;


            
            //TO DO: Now that GetAudioFeatures accepts the list post-comparator, it is time to develop the decision tree
            await Stef.GetAudioFeatures(Stef.TargetListIds);
            Stef.DecisionTree();

            Console.ReadLine();
        }

    }
}
