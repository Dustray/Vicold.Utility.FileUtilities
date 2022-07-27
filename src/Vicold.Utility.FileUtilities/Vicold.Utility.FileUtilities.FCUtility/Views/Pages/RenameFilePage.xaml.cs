﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Vicold.Utility.FileUtilities.FCUtility.Core;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// RenameFilePage.xaml 的交互逻辑
    /// </summary>
    public partial class RenameFilePage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;
        private bool modified = false;

        internal RenameFilePage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
            LoadConfig();
            modified = false;
        }

        public string FuncTitle { get; } = "视频资源格式重命名";

        private void LoadConfig()
        {
            var config = _coreHandler.GetConfig();
            if (config is { })
            {
                var builder = new StringBuilder();
                builder.AppendLine(config.MainPath);
                if (config.SubPaths is { })
                {
                    foreach (var subPath in config.SubPaths)
                    {
                        builder.AppendLine(subPath);
                    }
                }

                PathText.Text = builder.ToString();
                modified = true;
            }
        }

        public void Reflush()
        {
            if (!modified)
            {
                LoadConfig();
            }
        }

        private void GetPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择资源路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PathText.AppendText($"{dialog.SelectedPath}\r\n");
            }
        }

        private async void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.Log(this, "资源重命名开始");
            
            // 遍历SubPathText的每一行
            var subPaths = PathText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            await Task.Run(() =>
            {
                foreach (var path in subPaths)
                {
                    if (Directory.Exists(path))
                    {
                        _coreHandler.RenameFileFormat(path, true, false);
                    }
                    else
                    {
                        _logger.Log(this, $"文件{path}不存在，已跳过。");
                    }
                }
            });

            _logger.Log(this, "资源重命名完成");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            modified = false;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            PathText.Clear();
            modified = true;
        }

        private void PathText_TextChanged(object sender, TextChangedEventArgs e)
        {
            modified = true;
        }
    }
}