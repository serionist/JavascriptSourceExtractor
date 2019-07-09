using System;
using System.Collections.Generic;
using System.Text;

namespace JavascriptSourceExtractor
{
    public class MapSchema
    {
        public string file { get; set; }
        public int version { get; set; }
        public string[] sources { get; set; }
        public string[] sourcesContent { get; set; }
    }
}
