using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Normalize arguments (lowercased with single hyphen)
            args = (args ?? new string[0]).Where(a => string.IsNullOrWhiteSpace(a) == false).Select(a => a.ToLowerInvariant().Replace("--", "-")).ToArray();

            Console.WriteLine("Finds images with no extensions and resizes them.");

            // Show help
            if (args.Length == 0 || args.Length < 4 || (args.Length == 1 && (args[0] == "-?" || args[0] == "-h" || args[0] == "-help")))
            {
                Console.WriteLine("Usage of the image finder application");
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine(" -?                      > Shows this information");
                Console.WriteLine(" -h[elp]                 > Shows this information");
                Console.WriteLine(" -p[ath]                 > Path to unknown files");
                Console.WriteLine(" -o[utput]               > Path to save image files");
                Console.WriteLine(" -mw                     > Maximum width of image (default 1048).");
                Console.WriteLine(" -mh                     > Maximum height of image (default 648).");
                Console.WriteLine(" -q[uality]              > Quality of resized image file (default 80)");
                Console.WriteLine(" -s[kip]                 > Skip images with a certain with, eg. 1322.");
                Console.WriteLine("-----------------------------------------------------------");
                return;
            }

            var dirPath = args.Any(a => a.StartsWith("-p")) ? args[Array.IndexOf(args, args.First(a => a.StartsWith("-p"))) + 1] : throw new ArgumentException("Missing argument -path");
            var outputPath = args.Any(a => a.StartsWith("-o")) ? args[Array.IndexOf(args, args.First(a => a.StartsWith("-o"))) + 1] : throw new ArgumentException("Missing argument -output");
            var maxWidth = args.Any(a => a.StartsWith("-mw")) ? Convert.ToInt32(args[Array.IndexOf(args, args.First(a => a.StartsWith("-mw"))) + 1]) : 1048;
            var maxHeight = args.Any(a => a.StartsWith("-mh")) ? Convert.ToInt32(args[Array.IndexOf(args, args.First(a => a.StartsWith("-mh"))) + 1]) : 768;
            var quality = args.Any(a => a.StartsWith("-q")) ? Convert.ToInt32(args[Array.IndexOf(args, args.First(a => a.StartsWith("-q"))) + 1]) : 80;
            int? skip = args.Any(a => a.StartsWith("-s"))
                ? (Int32.TryParse(args[Array.IndexOf(args, args.First(a => a.StartsWith("-s"))) + 1], out var tempSkip)
                    ? tempSkip
                    : (int?) null)
                : null;
            
            var extensionWhitelist = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };

            var config = new Config(dirPath, outputPath, maxWidth, maxHeight, quality, extensionWhitelist, skip);

            Stopwatch timer = new Stopwatch();
            timer.Restart();

            ProcessFiles(config);

            timer.Stop();
            Console.WriteLine("Took {0:hh\\:mm\\:ss\\.ff}", timer.Elapsed);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Hit any key...");
                Console.ReadKey();
            }
        }

        private static void ProcessFiles(Config config)
        {
            int processed = 0;
            int success = 0;
            int failed = 0;
            int notImages = 0;

            var list = Directory.EnumerateFiles(config.SourcePath).ToArray();
            Console.WriteLine("Processing files...");
            ConsoleUtility.WriteProgressBar(0);

            foreach (string file in list)
            {
                processed++;
                int complete = (int)Math.Round((double)(processed * 100) / list.Length);
                ConsoleUtility.WriteProgressBar(complete, true);

                if (Path.GetExtension(file).Length == 0)
                {
                    var bytes = File.ReadAllBytes(file);
                    var fileType = FilesHelper.GetKnownFileType(bytes);
                    var ext = ("." + fileType).ToLower();

                    if (!config.ExtensionWhitelist.Contains(ext))
                    {
                        notImages++;
                        File.Copy(file, Path.Combine(config.DestinationPath, Path.GetFileName(file) + ext));
                        continue;
                    }

                    FilesHelper.EnsureDirectoryExists(config.DestinationPath);
                    string newFilePath = Path.Combine(config.DestinationPath, Path.GetFileNameWithoutExtension(file) + ext);

                    try
                    {
                        ImageConverter.Convert(bytes, config.MaxWidth, config.MaxHeight, config.Quality, newFilePath, ext, config.SkipWidth);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        failed++;
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Done. Processed {0} files. Success: {1}, Failed: {2}, Not images {3}.", processed, success, failed, notImages);
        }
    }
}
