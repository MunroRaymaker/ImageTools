# Introduction 
This is a small console helper written with VS Code to find images without extensions from a directory and also resize them.

# Getting started
There is only one console application which takes four arguments
* path, the input directory.
* output, where resized images should be written.
* mw, maximum width.
* mh, maximum height.
* q, image quality of compressed file.

Usage:
```
imagefinder.exe --path C:\\temp\\somedir --output c:\\temp\\anotherdir --mw 1048 --mh 768 --q 80
```

# Build and Test
Run 
```
dotnet build
```
to build the code

