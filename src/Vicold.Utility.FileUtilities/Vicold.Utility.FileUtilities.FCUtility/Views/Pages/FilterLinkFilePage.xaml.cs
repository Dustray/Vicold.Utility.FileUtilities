using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Vicold.Utility.FileUtilities.FCUtility.Core;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// FilterLinkFilePage.xaml 的交互逻辑
    /// </summary>
    public partial class FilterLinkFilePage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;

        internal FilterLinkFilePage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
        }

        public string FuncTitle { get; } = "过滤链接文件";

        private void FastFilterButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();
            openF.Title = "Load a txt file";
            if (openF.ShowDialog() == true)
            {
                var sourcePath = openF.FileName;
                var lines = FileStrUtility.ReadTxtFileAndSplitLinks(sourcePath);
                if (lines is { })
                {
                    var result = _coreHandler.LinkDuplicateRemoval(lines);
                    string newFilePath = CreateFilteredFileName(sourcePath);
                    using StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.UTF8);
                    foreach (var resultLine in result)
                    {
                        sw.WriteLine(resultLine);
                    }
                }
            }
        }

        private static string CreateFilteredFileName(string sourcePath)
        {
            var newFileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath) + "_filter";
            var newFilePath = FilePathUtility.GetNewFileName(sourcePath, newFileName);
            return newFilePath;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();
            openF.Title = "Load a txt file";
            if (openF.ShowDialog() == true)
            {
                var sourcePath = openF.FileName;
                InPathText.Text = sourcePath;
                OutPathText.Text = CreateFilteredFileName(sourcePath);
                BeforeText.Text = File.ReadAllText(sourcePath);
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            var lines = FileStrUtility.SplitLinks(BeforeText.Text);
            if (lines is { })
            {
                var result = _coreHandler.LinkDuplicateRemoval(lines);
                var builder = new StringBuilder();
                if (result is { })
                {
                    foreach (var subPath in result)
                    {
                        builder.AppendLine(subPath);
                    }
                }

                AfterText.Text = builder.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var file = OutPathText.Text;
            if (string.IsNullOrWhiteSpace(file))
            {
                MessageBox.Show("输出路径不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var dir = Path.GetDirectoryName(file);
            if (string.IsNullOrWhiteSpace(dir))
            {
                MessageBox.Show("输出路径格式错误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    _logger.Log(this, "主资源路径不存在，已自动创建");
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("主资源路径不存在且无法创建，请检查路径是否合法。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 判断是否覆盖
            if (File.Exists(file))
            {
                var result = MessageBox.Show("文件已存在，是否覆盖？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            
            File.WriteAllText(file, AfterText.Text);
        }


    }
}
