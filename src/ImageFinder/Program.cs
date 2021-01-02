﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ImageFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Finds images with no extensions and resizes them.");
            if(args.Length < 4)
            {
                System.Console.WriteLine("Usage: imagefinder.exe --path C:\\temp\\somedir --output c:\\temp\\anotherdir --mw 1048 --mh 768 --q 80");
                return;
            }

            string dirPath = args.ToList().IndexOf("--path") >= 0 ? args[args.ToList().IndexOf("--path") + 1] : throw new ArgumentException("Missing argument --path"); 
            string outputPath = args.ToList().IndexOf("--output") >= 0 ? args[args.ToList().IndexOf("--output") + 1] : throw new ArgumentException("Missing argument --output"); 
            int maxWidth = args.ToList().IndexOf("--mw") >= 0 ? Convert.ToInt32(args[args.ToList().IndexOf("--mw") + 1]) : 1048;
            int maxHeight = args.ToList().IndexOf("--mh") >= 0 ? Convert.ToInt32(args[args.ToList().IndexOf("--mh") + 1]) : 768;
            int quality = args.ToList().IndexOf("--q") >= 0 ? Convert.ToInt32(args[args.ToList().IndexOf("--q") + 1]) : 80;

            int processed = 0;
            int success = 0;
            int failed = 0;
            int notImages = 0;
            var extensionWhitelist = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };

            Stopwatch timer = new Stopwatch();
            timer.Restart();
            Console.WriteLine("Working...");
            ConsoleUtility.WriteProgressBar(0);

            var list = Directory.EnumerateFiles(dirPath).ToArray();

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
                    
                    if (!extensionWhitelist.Contains(ext))
                    {
                        notImages++;
                        continue;
                    }

                    string newFilePath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(file) + ext);
                    
                    try
                    {
                        ImageConverter.Convert(bytes, maxWidth, maxHeight, quality, newFilePath, ext);
                        success++;
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        failed++;
                    }
                }
            }

            timer.Stop();
            Console.WriteLine();
            System.Console.WriteLine("Done. Processed {0} files. Success: {1}, Failed: {2}, Not images {3}. Took {4}ms", processed, success, failed, notImages, timer.ElapsedMilliseconds);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Hit any any...");
                Console.ReadKey();
            }
        }
    } 
}
