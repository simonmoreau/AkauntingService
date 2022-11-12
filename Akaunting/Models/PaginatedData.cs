using System;
using System.Collections.Generic;

namespace AkauntingService
{

    public class Link
    {
        public string url { get; set; }
        public string label { get; set; }
        public bool active { get; set; }
    }

    public class Links
    {
        public string first { get; set; }
        public string last { get; set; }
        public object prev { get; set; }
        public object next { get; set; }
    }

    public class Meta
    {
        public int current_page { get; set; }
        public object from { get; set; }
        public int last_page { get; set; }
        public List<Link> links { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public object to { get; set; }
        public int total { get; set; }
    }

    public class PaginatedData
    {
        public List<object> data { get; set; }
        public Links links { get; set; }
        public Meta meta { get; set; }
    }


}
