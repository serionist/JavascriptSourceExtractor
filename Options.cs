using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace JavascriptSourceExtractor
{
    public abstract class Options
    {
        [Option('o', "output", HelpText = "The output directory, where extracted source files will be placed", Required =true)]
        public string OutputDir { get; set; }
        [Option('t', "no-ts", HelpText = "No typescript source files will be extracted", Required =false, Default =false)]
        public bool SkipTs { get; set; }

        [Option('j', "no-js", HelpText = "No javascript source files will be extracted", Required = false, Default = false)]
        public bool SkipJs { get; set; }
    }
    [Verb("file", HelpText = "Extracts source files from a single .js.map file")]
    public class FileOptions: Options
    {
        [Option('i', "input", HelpText = "The input *.js.map file", Required =true)]
        public string InputFile { get; set; }
    }
    [Verb("dir", HelpText = "Extracts source files each .js.map file contained in a directory")]
    public class DirOptions : Options
    {
        [Option('i', "input", HelpText = "The input *.js.map file", Required = true)]
        public string InputFile { get; set; }
        [Option('e', "include-js-with-no-map", Required =false, Default =false, HelpText = "Extract .js files which have no corresponding .map files")]
        public bool ExtractJsWithoutMap { get; set; }
        [Option('a', "all-levels", Required = false, Default = false, HelpText = "Search for .map files in all subdirectories")]
        public bool AllFiles { get; set; }
        [Option('d', "diff-directory", Required =false, Default =false, HelpText = "Place files of each map to a different directory")]
        public bool DifferentDirs { get; set; }
    }
}
