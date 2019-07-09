using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace JavascriptSourceExtractor
{
    class Program
    {
        static void Main(string[] args) => Parser.Default.ParseArguments<FileOptions, DirOptions>(args).WithParsed(o =>
        {
            try
            {
                int ret = 0;
                if (o is FileOptions f)
                {
                    l($"Starting conversion from file: {f.InputFile}");
                    doCommon(f);
                    ret = doFile(f);
                }
                if (o is DirOptions d)
                {
                    l($"Starting conversion from directory: {d.InputFile}");
                    doCommon(d);
                    ret = doDirectory(d);
                }
                l($"Operation success! {ret} source files were extracted!");
                   
            }
            catch(Exception e)
            {
                l($"Operation failed! Details:\r\n{e}", true);
                Environment.Exit(1);
            }
        });
        static void l(string msg, bool error = false)
        {
            if (error)
                Console.Error.WriteLine(msg);
            else Console.WriteLine(msg);
        }

        static void doCommon(Options options)
        {
            l($"Output directory: {options.OutputDir}. Creating if necessary");
            Directory.CreateDirectory(options.OutputDir);
        }

        static int doFile(FileOptions options) => _processFile(options.InputFile, options.OutputDir, options);
        static int doDirectory(DirOptions options)
        {
            l("Enumerating input directory contents");
            var files = Directory.GetFiles(options.InputFile, "*", options.AllFiles ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            var jsMapFiles = files.Where(e => e.EndsWith(".js.map", StringComparison.CurrentCultureIgnoreCase)).ToList();
            jsMapFiles.RemoveAll(f => f.EndsWith(".min.js.map", StringComparison.CurrentCultureIgnoreCase) && jsMapFiles.Any(m => m == f.Replace(".min.js.map", ".js.map", StringComparison.CurrentCultureIgnoreCase)));

            var singleJsFiles = files.Where(e => e.EndsWith(".js", StringComparison.CurrentCultureIgnoreCase) && !jsMapFiles.Contains($"{e}.map", StringComparer.CurrentCultureIgnoreCase)).ToList();
            l($"{jsMapFiles.Count} js map files found!");
            var ret = 0;
            foreach (var map in jsMapFiles)
                ret += _processFile(map, options.DifferentDirs? Path.Combine(options.OutputDir, Path.GetFileName(map).Replace(".js.map", "", StringComparison.CurrentCultureIgnoreCase)) : options.OutputDir, options);
            l("Map files extracted!");
            if (options.ExtractJsWithoutMap)
            {
                l($"{singleJsFiles.Count} single js files found. Copying them to output");
                foreach (var j in singleJsFiles)
                    File.Copy(j, Path.Combine(options.OutputDir, Path.GetFileName(j)), true);
            }
            return ret;
        }

        static int _processFile(string filePath, string outputDir, Options options)
        {
            l($"\tExtracting files from source file: {filePath}");
            var fileContent = _doOp("\tFailed to read file", ()=> File.ReadAllText(filePath));
            var jsonContent = _doOp("\tFailed to parse file to JSON", () => JsonConvert.DeserializeObject<MapSchema>(fileContent));
            if (jsonContent.sources.Length == 0)
            {
                l($"\tSourceMap file has no content. Skipping");
                return 0;
            }
            if (jsonContent.sources.Length != jsonContent.sourcesContent.Length)
                throw new Exception("SourceMap file invalid. Not the same number of sources and sourcecontents are provided");
            int ret = 0;
            for (int sourceIndex = 0;sourceIndex < jsonContent.sources.Length; sourceIndex++)
            {
                var sourcePath = jsonContent.sources[sourceIndex];
                if (sourcePath.StartsWith("webpack:///external") || sourcePath.StartsWith("webpack:///(webpack)"))
                    continue;
                var sourceFilePath = Path.Combine(new string[] { outputDir }.Concat(getWebpackFilePath(sourcePath)).ToArray());

                if (sourceFilePath.EndsWith(".js", StringComparison.CurrentCultureIgnoreCase) && options.SkipJs)
                    continue;
                if (sourceFilePath.EndsWith(".ts", StringComparison.CurrentCultureIgnoreCase) && options.SkipTs)
                    continue;
                Directory.CreateDirectory(Path.GetDirectoryName(sourceFilePath));
                File.WriteAllText(sourceFilePath, jsonContent.sourcesContent[sourceIndex]);
                ret++;
            }
            return ret;
        }

        static T _doOp<T>(string error, Func<T> op)
        {
            try
            {
                return op();
            }
            catch(Exception ex)
            {
                throw new Exception(error, ex);
            }
        }

        static string[] getWebpackFilePath(string path)
        {
            var regex = Regex.Match(path, @"^(?>webpack:\/\/)*[.|\/~]*([a-zA-Z(@].*)$");
            if (!regex.Success || !regex.Groups[1].Success)
                throw new Exception($"Webpack file path: '{path}' is not of valid format");
            var val = regex.Groups[1].Value;
            foreach (var c in Path.GetInvalidFileNameChars())
                if (c != '/')
                    val = val.Replace(c, '_');
            return val.Split('/').Where(e=> !e.All(c=>c == '.')).ToArray();
        }
    }
}
