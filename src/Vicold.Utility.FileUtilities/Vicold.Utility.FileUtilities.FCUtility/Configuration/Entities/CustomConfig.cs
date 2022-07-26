using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vicold.Utility.FileUtilities.FCUtility.Configuration.Entities
{
    internal class CustomConfig
    {
        public string? MainPath { get; set; }

        public IList<string>? SubPaths { get; set; }

        public string? StandardFormat { get; set; }
        public string? UnknownFolderName { get; set; }
    }
}
