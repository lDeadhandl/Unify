using System;
using System.Collections.Generic;
using System.Text;

namespace Unify
{
    class Paging
    {
        public string Href { get; set; }
        public List<> Items { get; set; }
        public int Limit { get; set; }
        public string Next { get; set; }
        public int Offset { get; set; }
        public string Previous { get; set; }
        public int Total { get; set; }
    }
}
