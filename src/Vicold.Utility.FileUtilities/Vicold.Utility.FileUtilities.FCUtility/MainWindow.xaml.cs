using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Vicold.Utility.FileUtilities.FCUtility.Core;
using Vicold.Utility.FileUtilities.FCUtility.Database;
using Vicold.Utility.FileUtilities.FCUtility.Views.Controls;
using Vicold.Utility.FileUtilities.FCUtility.Views.Pages;

namespace Vicold.Utility.FileUtilities.FCUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CoreHandler _coreHandler;
        private EditVideoPathPage _editVideoPathPage;
        private ImportToDBPage _importToDBPage;
        private Logger _logger;

        public MainWindow()
        {
            InitializeComponent();
            _coreHandler = new CoreHandler();
            _logger = new(LogThis);
            _editVideoPathPage = new  (_coreHandler, _logger);
            _importToDBPage = new(_coreHandler, _logger, UpdateDbInfo);
            //_coreHandler.SyncDatabase();
            UpdateDbInfo();

        }

        private void SyncDBButton_Click(object sender, RoutedEventArgs e)
        {
            _coreHandler.SyncDatabase();
        }

        /// <summary>
        /// 过滤文本文件中的数据并保存为新的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTxtFileAndFilterButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();
            openF.Title = "Load a txt file";
            if (openF.ShowDialog() == true)
            {
                var sourcePath = openF.FileName;
                var lines = ReadTxtFileAndGetLinks(sourcePath);
                if (lines is { })
                {
                    var result = _coreHandler.LinkDuplicateRemoval(lines);

                    var newFileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath) + "_filter";
                    var newFilePath = FilePathUtility.GetNewFileName(sourcePath, newFileName);
                    using StreamWriter sw = new StreamWriter(newFilePath, false, Encoding.UTF8);
                    foreach (var resultLine in result)
                    {
                        sw.WriteLine(resultLine);
                    }
                }
            }
        }

        /// <summary>
        /// 从文本文件中的链接中导入数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTxtFileAndImportToDBButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openF = new OpenFileDialog();
            openF.Title = "Load a txt file";
            if (openF.ShowDialog() == true)
            {
                var lines = ReadTxtFileAndGetLinks(openF.FileName);
                if (lines is { })
                {
                    _coreHandler.WriteUnknownCodeToDatebase(lines);
                    UpdateDbInfo();
                }
            }
        }

        /// <summary>
        /// 从视频文件目录同步（导入并更新）数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncDBToImportToDBButton_Click(object sender, RoutedEventArgs e)
        {
            _coreHandler.SyncDatabase();
            UpdateDbInfo();
        }

        /// <summary>
        /// 重命名所有文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenameAllFileButton_Click(object sender, RoutedEventArgs e)
        {
            return;
            _coreHandler.RenameAllFile();
        }

        private static IList<string>? ReadTxtFileAndGetLinks(string path)
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

        /// <summary>
        /// 更新数据库计数数据
        /// </summary>
        private void UpdateDbInfo()
        {
            DBCountPanel.Children.Clear();

            var counts = _coreHandler.GetLevelAndTypeDataCount();
            foreach (var count in counts)
            {
                var countItem = new DBCountItem(count.Key, count.Value);
                DBCountPanel.Children.Add(countItem);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _coreHandler.Dispose();
            if (_coreHandler.HasDatabaseChanged())
            {
                DBBackupTool.BackupDatabase();
            }
        }

        private void EditVideoPath_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_editVideoPathPage);
            UpdateFuncTitle(_editVideoPathPage);
        }

        private void UpdateFuncTitle(IFuncPage funcPage)
        {
            FuncTitleText.Text = funcPage.FuncTitle;
        }


        private void LogThis(IFuncPage funcPage,string log)
        {
            LogText.AppendText($"{funcPage.FuncTitle}：{log}\r\n");
            LogText.ScrollToEnd();
        }
    }
}
