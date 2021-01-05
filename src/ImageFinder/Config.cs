using System.Collections.Generic;

namespace ImageFinder
{
    internal class Config
    {
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public int MaxWidth { get; }
        public int MaxHeight { get; }
        public int Quality { get; }
        public List<string> ExtensionWhitelist { get; }
        public int? SkipWidth { get; set; }

        public Config(string sourcePath, string destinationPath, in int maxWidth, in int maxHeight, in int quality, List<string> extensionWhitelist, int? skipWidth)
        {
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Quality = quality;
            ExtensionWhitelist = extensionWhitelist;
            SkipWidth = skipWidth;
        }
    }
}