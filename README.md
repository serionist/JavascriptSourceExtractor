# JavascriptSourceExtractor
A small tool to extract javascript/typescript source files from .js.map files into a directory, reversing a javascript project from map files

# Usage
Run from command line. 
The app will also provide detailed Help text for required parameters

## Single file extraction
You can Extract the source of a single .js.map file by running ``dotnet JavascriptExtractor.dll file [arguments]`` with the following arguments:
```
  -i, --input     Required. The input *.js.map file

  -o, --output    Required. The output directory, where extracted source files will be placed

  -t, --no-ts     (Default: false) No typescript source files will be extracted

  -j, --no-js     (Default: false) No javascript source files will be extracted
```

## Directory extraction
You can Extract the source of all .js.map files from a directory by running ``dotnet JavascriptExtractor.dll dir [arguments]`` with the following arguments:
```
  -i, --input                     Required. The input *.js.map file

  -e, --include-js-with-no-map    (Default: false) Extract .js files which have no corresponding .map
                                  files

  -a, --all-levels                (Default: false) Search for .map files in all subdirectories

  -d, --diff-directory            (Default: false) Place files of each map to a different directory

  -o, --output                    Required. The output directory, where extracted source files will be
                                  placed

  -t, --no-ts                     (Default: false) No typescript source files will be extracted

  -j, --no-js                     (Default: false) No javascript source files will be extracted
```
