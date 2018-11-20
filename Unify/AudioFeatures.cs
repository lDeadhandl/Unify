using System;
using System.Collections.Generic;
using System.Text;

namespace Unify
{
    public class AudioFeaturesObject
    {
        public List<AudioFeatures> AudioFeatures { get; set; }
    }

    public class AudioFeatures
    {
        public float Acousticness { get; set; }

        public float Danceability { get; set; }

        public float Energy { get; set; }

        public float Instrumentalness { get; set; }

        public float Liveness { get; set; }

        public float Loudness { get; set; }

        public float Speechiness { get; set; }

        public float Tempo { get; set; }

        public float Valence { get; set; }

        public string Uri { get; set; }

    }
}
