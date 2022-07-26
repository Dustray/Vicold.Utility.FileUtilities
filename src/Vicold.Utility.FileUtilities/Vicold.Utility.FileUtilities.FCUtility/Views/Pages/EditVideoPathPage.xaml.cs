﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Vicold.Utility.FileUtilities.FCUtility.Core;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// EditVideoPathPage.xaml 的交互逻辑
    /// </summary>
    public partial class EditVideoPathPage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;

        internal EditVideoPathPage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
            LoadConfig();
        }

        private void LoadConfig()
        {
            var config = _coreHandler.GetConfig();
            if (config is { })
            {
                MainPathText.Text = config.MainPath;
                var builder = new StringBuilder();
                if (config.SubPaths is { })
                {
                    foreach (var subPath in config.SubPaths)
                    {
                        builder.AppendLine(subPath);
                    }
                }

                SubPathText.Text = builder.ToString();
            }
        }

        public string FuncTitle { get; } = "编辑视频资源路径";

        private void MainPathButton_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择主资源路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MainPathText.Text = dialog.SelectedPath;
            }

        }

        private void SubPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择主资源路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SubPathText.AppendText($"{dialog.SelectedPath}\r\n");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MainPathText.Text))
            {
                System.Windows.MessageBox.Show("主资源路径不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Directory.Exists(MainPathText.Text))
            {
                try
                {
                    Directory.CreateDirectory(MainPathText.Text);
                    _logger.Log(this, "主资源路径不存在，已自动创建");
                }
                catch (DirectoryNotFoundException)
                {
                    System.Windows.MessageBox.Show("主资源路径不存在且无法创建，请检查路径是否合法。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 遍历SubPathText的每一行
            var subPaths = SubPathText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in subPaths)
            {
                if (!Directory.Exists(path))
                {
                    var result = System.Windows.MessageBox.Show($"子资源路径“{path}”不存在，是否继续保存", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
            }

            var config = _coreHandler.GetConfig();
            config.MainPath = MainPathText.Text;
            config.SubPaths = subPaths;
            _coreHandler.SaveConfig();
            _logger.Log(this, "路径保存成功");
        }
    }
}
