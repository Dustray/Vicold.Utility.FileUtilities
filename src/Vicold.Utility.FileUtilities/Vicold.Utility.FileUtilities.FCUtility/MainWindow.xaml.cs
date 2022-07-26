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
        private FilterLinkFilePage _filterLinkFilePage;
        private ImportToDBPage _importToDBPage;
        private EditVideoPathPage _editVideoPathPage;
        private Logger _logger;

        public MainWindow()
        {
            InitializeComponent();
            _logger = new(LogThis);
            _coreHandler = new CoreHandler(_logger);
            _filterLinkFilePage = new(_coreHandler, _logger);
            _importToDBPage = new(_coreHandler, _logger, UpdateDbInfo);
            _editVideoPathPage = new(_coreHandler, _logger);
            //_coreHandler.SyncDatabase();
            UpdateDbInfo();

        }
        /// <summary>
        /// 从视频文件目录同步（导入并更新）数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SyncDBButton_Click(object sender, RoutedEventArgs e)
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

        #region 切换Pages

        private void FilterLinkFile_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_filterLinkFilePage);
            UpdateFuncTitle(_filterLinkFilePage);
        }

        private void ImportToDB_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_importToDBPage);
            UpdateFuncTitle(_importToDBPage);
        }

        private void EditVideoPath_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_editVideoPathPage);
            UpdateFuncTitle(_editVideoPathPage);
        }

        #endregion

        private void UpdateFuncTitle(IFuncPage funcPage)
        {
            FuncTitleText.Text = funcPage.FuncTitle;
        }


        private void LogThis(string log)
        {
            LogText.AppendText($"{log}\r\n");
            LogText.ScrollToEnd();
        }

    }
}
