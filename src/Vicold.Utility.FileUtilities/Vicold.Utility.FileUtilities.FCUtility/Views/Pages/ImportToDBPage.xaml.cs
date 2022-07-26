using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vicold.Utility.FileUtilities.FCUtility.Core;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// ImportToDBPage.xaml 的交互逻辑
    /// </summary>
    public partial class ImportToDBPage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;
        private Action _updateDbInfo;

        internal ImportToDBPage(CoreHandler coreHandler, Logger logger, Action updateDbInfo)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
            _updateDbInfo = updateDbInfo;
        }

        public string FuncTitle { get; } = "从链接文本导入数据库";

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            //打开文件
            var dialog = new OpenFileDialog();
            dialog.Title = "选择链接文件";
            dialog.Filter = "文本文件|*.txt";
            if (dialog.ShowDialog() == true)
            {
                LinkText.Text = File.ReadAllText(dialog.FileName);
                _logger.Log(this, $"打开文件：{dialog.FileName}");
            }

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                var lines = FileStrUtility.SplitLinks(LinkText.Text);
                if (lines is { })
                {
                    _coreHandler.WriteUnknownCodeToDatebase(lines);
                    _updateDbInfo.Invoke();
                    _logger.Log(this, $"导入了{lines.Count}条数据");
                }
            });
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
                LinkText.Text = File.ReadAllText(path);
                _logger.Log(this, $"打开文件：{path}");
            }
        }
    }
}
