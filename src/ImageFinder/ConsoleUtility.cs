using System;

namespace ImageFinder
{
    internal static class ConsoleUtility
    {
        private const char Block = '■';
        private const string Back = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
        private const string Twirl = "-\\|/";

        public static void WriteProgressBar(int percent, bool update = false)
        {
            if (update)
            {
                Console.Write(Back);
            }
            Console.Write("[");
            var p = (int)((percent / 10f) + .5f);
            for (var i = 0; i < 10; ++i)
            {
                Console.Write(i >= p ? ' ' : Block);
            }
            Console.Write("] {0,3:##0}%", percent);
        }

        public static void WriteProgress(int progress, bool update = false)
        {
            if (update)
                Console.Write("\b");
            Console.Write(Twirl[progress % Twirl.Length]);
        }
    }
}
