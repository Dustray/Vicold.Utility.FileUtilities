using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using Vicold.Utility.FileUtilities.FCUtility.Database.Entities;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// CodeSearchPage.xaml 的交互逻辑
    /// </summary>
    public partial class CodeSearchPage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;

        internal CodeSearchPage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
        }

        public string FuncTitle { get; } = "代码查询";

        private async void FastSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var codeLong = FastSearchText.Text;
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(codeLong))
                {
                    _logger.Log(this, "请输入搜索内容");
                    return;
                }

                var codeStr = FilePathUtility.GetCodeFromLongStr(codeLong);
                Dispatcher.Invoke(() =>
                {
                    if (codeStr is { } && int.TryParse(codeStr, out var code))
                    {
                        SearchCodeText.Text = codeStr;
                        var info = _coreHandler.DB.Search(code);
                        _logger.Log(this, $"查询代码：{codeStr}{(info is { } ? "  查询成功" : string.Empty)}");
                        if (info is { })
                        {
                            SearchResultText.Text = "查询成功";
                            SearchTypeText.Text = $"{TypeTypeInfo.GetTypeName(info.Type)} - {LevelTypeInfo.GetLevelName(info.Level)}";
                            //_logger.Log(this, $"查找成功，类型：{TypeTypeInfo.GetTypeName(info.Type)}，级别：{LevelTypeInfo.GetLevelName(info.Level)}");
                            //_logger.Log(this, $"文件路径：{info.FilePath}");

                            if (info.FilePath is { } && System.IO.File.Exists(info.FilePath))
                            {
                                OpenFileButton.Visibility = OpenPathButton.Visibility = Visibility;
                                SearchPathText.Text = info.FilePath;
                            }
                            else
                            {
                                OpenFileButton.Visibility = OpenPathButton.Visibility = Visibility.Collapsed;
                                SearchPathText.Text = "未查询到文件位置";
                            }
                        }
                        else
                        {
                            SearchTypeText.Text = string.Empty;
                            SearchPathText.Text = string.Empty;
                            OpenFileButton.Visibility = OpenPathButton.Visibility = Visibility.Collapsed;
                            SearchResultText.Text = "没有搜索到结果";
                            //_logger.Log(this, "没有搜索到结果");
                        }
                    }
                    else
                    {
                        SearchTypeText.Text = string.Empty;
                        SearchCodeText.Text = string.Empty;
                        SearchPathText.Text = string.Empty;
                        OpenFileButton.Visibility = OpenPathButton.Visibility = Visibility.Collapsed;
                        SearchResultText.Text = "搜索内容不合法，无法检测到代码";
                        //_logger.Log(this, "搜索内容不合法，无法检测到代码");
                    }
                });
            });

            FastSearchText.Focus();
            FastSearchText.SelectAll();
        }


        private void PasteAndSearchButton_Click(object sender, RoutedEventArgs e)
        {
            string pastedText = Clipboard.GetText(TextDataFormat.Text);

            if (!string.IsNullOrWhiteSpace(pastedText))
            {
                FastSearchText.Text = pastedText;
                FastSearchButton_Click(sender, e);
            }
            else
            {
                var error = $"没有复制任何文本";
                _logger.Log(this, error);
                MessageBox.Show(error);
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var file = SearchPathText.Text;
            if (System.IO.File.Exists(file))
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(file)
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
            else
            {
                var error = $"文件：{file}不存在";
                _logger.Log(this, error);
                MessageBox.Show(error);
            }
        }

        private void OpenPathButton_Click(object sender, RoutedEventArgs e)
        {
            var file = SearchPathText.Text;
            var path = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(path))
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(path)
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
            else
            {
                var error = $"文件路径：{path}不存在";
                _logger.Log(this, error);
                MessageBox.Show(error);
            }
        }

    }
}
