using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unify
{
    public class SavedTrack
    {
        public DateTime AddedAt { get; set; }

        public Track Track { get; set; }
    }

    public class Track
    {
        public string Id { get; set; }

        public string Name { get; set; }
    
    }
}
