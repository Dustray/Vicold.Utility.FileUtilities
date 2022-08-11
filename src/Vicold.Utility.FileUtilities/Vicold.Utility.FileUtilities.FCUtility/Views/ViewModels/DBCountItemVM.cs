using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.ViewModels
{
    internal class DBCountItemVM
    {
        public DBCountItemVM(string title, int count, string? path = null)
        {
            _title = $"{title}: ";
            Count = count;
            Path = path;
        }

        public string _title { get; set; } = "Unknown";
        public string Title
        {
            get => _title;
            set
            {
                _title = $"{_title}: ";
            }
        }

        public int Count { get; set; } = 0;

        public string CountStr => $" {Count}";

        public string? Path { get; set; }

        public Visibility Visibility => Path is { } ? Visibility.Visible : Visibility.Hidden;

        public bool IsEnable => System.IO.Directory.Exists(Path);

    }
}
