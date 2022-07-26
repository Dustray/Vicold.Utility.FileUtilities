using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
            _logger.Log(this, $"执行快速过滤");
            OpenFileDialog openF = new OpenFileDialog();
            openF.Title = "Load a txt file";
            openF.Filter = "文本文件|*.txt";
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

                    _logger.Log(this, $"执行快速过滤完成，源文件记录数{lines.Count}条，过滤后记录数{result.Count}条，过滤掉{lines.Count - result.Count}数据，文件已保存为{newFilePath}");
                }
                else
                {
                    _logger.Log(this, $"执行快速过滤结束，源文件条目数为0。");
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
            openF.Filter = "文本文件|*.txt";
            if (openF.ShowDialog() == true)
            {
                var sourcePath = openF.FileName;
                LoadInPath(sourcePath);
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                _logger.Log(this, $"执行过滤");
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
                        _logger.Log(this, $"执行过滤完成，源文件记录数{lines.Count}条，过滤后记录数{result.Count}条，过滤掉{lines.Count - result.Count}数据");
                    }

                    Dispatcher.Invoke(() =>
                    {
                        AfterText.Text = builder.ToString();
                    });
                }
                else
                {
                    _logger.Log(this, $"执行过滤结束，源文件条目数为0。");
                }
            });
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
            _logger.Log(this, $"文件已保存为{file}");
        }

        private void Page_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            var path = FilePathUtility.GetFileNameFromDragEventArgs(e);
            if (path is { })
            {
                LoadInPath(path);
            }
        }

        private void LoadInPath(string path)
        {
            InPathText.Text = path;
            OutPathText.Text = CreateFilteredFileName(path);
            BeforeText.Text = File.ReadAllText(path);
            _logger.Log(this, $"打开文件：{path}");
        }
    }
}
