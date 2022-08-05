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
        private CodeSearchPage _CodeSearchPage;
        private FilterLinkFilePage _filterLinkFilePage;
        private ImportToDBPage _importToDBPage;
        private EditVideoPathPage _editVideoPathPage;
        private FileOperationPage _renameFilePage;
        private Logger _logger;

        public MainWindow()
        {
            InitializeComponent();
            _logger = new(LogThis);
            _coreHandler = new CoreHandler(_logger);
            _CodeSearchPage = new(_coreHandler, _logger);
            _filterLinkFilePage = new(_coreHandler, _logger);
            _importToDBPage = new(_coreHandler, _logger, UpdateDbInfo);
            _editVideoPathPage = new(_coreHandler, _logger);
            _renameFilePage = new(_coreHandler, _logger);
            //_coreHandler.SyncDatabase();
            UpdateDbInfo();
            CodeSearch_Click(1, new RoutedEventArgs());
        }
        /// <summary>
        /// 从视频文件目录同步（导入并更新）数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private async void SyncDBButton_Click(object sender, RoutedEventArgs e)
        {
            await _coreHandler.SyncDatabase().ConfigureAwait(false);
            UpdateDbInfo();
        }

        /// <summary>
        /// 更新数据库计数数据
        /// </summary>
        private void UpdateDbInfo()
        {
            Dispatcher.Invoke(() =>
            {
                DBCountPanel.Children.Clear();
                var counts = _coreHandler.GetLevelAndTypeDataCount();
                foreach (var count in counts)
                {
                    var countItem = new DBCountItem(count.Key, count.Value);
                    DBCountPanel.Children.Add(countItem);
                }
            });
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

        private void CodeSearch_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_CodeSearchPage);
            UpdateFuncTitle(_CodeSearchPage);
            ChangeButtonFlag(CodeSearch);
        }

        private void FilterLinkFile_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_filterLinkFilePage);
            UpdateFuncTitle(_filterLinkFilePage);
            ChangeButtonFlag(FilterLinkFile);
        }

        private void ImportToDB_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_importToDBPage);
            UpdateFuncTitle(_importToDBPage);
            ChangeButtonFlag(ImportToDB);
        }

        private void EditVideoPath_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_editVideoPathPage);
            UpdateFuncTitle(_editVideoPathPage);
            ChangeButtonFlag(EditVideoPath);
            _editVideoPathPage.Reflush();
        }

        private void RenameFile_Click(object sender, RoutedEventArgs e)
        {
            FuncFrame.Navigate(_renameFilePage);
            UpdateFuncTitle(_renameFilePage);
            ChangeButtonFlag(RenameFile);
            _renameFilePage.Reflush();
        }
        #endregion

        private void UpdateFuncTitle(IFuncPage funcPage)
        {
            FuncTitleText.Text = funcPage.FuncTitle;
        }

        private void LogThis(string log)
        {
            Dispatcher.Invoke(() =>
            {
                LogText.AppendText($"{log}\r\n");
                LogText.ScrollToEnd();
            });
        }

        private void ChangeButtonFlag(System.Windows.Controls.Button button)
        {
            for (var i = 2; i < NavButtonPanel.Children.Count; i++)
            {
                var child = NavButtonPanel.Children[i];
                if (child is System.Windows.Controls.Button btn)
                {
                    if (btn.Content is System.Windows.Controls.StackPanel panel)
                    {
                        if (btn == button)
                        {
                            panel.Children[0].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            panel.Children[0].Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }

        private void ReadOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            LogText.IsReadOnly = !LogText.IsReadOnly;
            ReadOnlyButton.Content = LogText.IsReadOnly ? "编辑" : "只读";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LogText.Clear();
        }
    }
}
