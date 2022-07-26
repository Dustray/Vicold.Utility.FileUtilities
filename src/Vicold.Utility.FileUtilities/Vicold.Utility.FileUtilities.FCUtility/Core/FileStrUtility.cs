using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Core
{
    internal static class FileStrUtility
    {
        public static IList<string>? SplitLinks(string links)
        {
            var subPaths = links.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return subPaths;
        }
        
        public static IList<string>? ReadTxtFileAndSplitLinks(string path)
        {
            if (File.Exists(path))
            {
                using StreamReader sr = new StreamReader(path, Encoding.Default);
                var lines = new List<string>();
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines;
            }
            return null;
        }
    }
}
