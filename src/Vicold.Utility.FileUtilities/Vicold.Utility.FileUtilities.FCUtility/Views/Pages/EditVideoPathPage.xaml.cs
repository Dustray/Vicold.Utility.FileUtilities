using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Vicold.Utility.FileUtilities.FCUtility.Core;
using Vicold.Utility.FileUtilities.FCUtility.Database.Entities;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// EditVideoPathPage.xaml 的交互逻辑
    /// </summary>
    public partial class EditVideoPathPage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;
        private bool modified = false;

        internal EditVideoPathPage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
            LoadConfig();
        }

        public string FuncTitle { get; } = "编辑视频资源路径";

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

        public void Reflush()
        {
            if (!modified)
            {
                LoadConfig();
            }
        }


        private void MainPathButton_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择主资源路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MainPathText.Text = dialog.SelectedPath;
                modified = true;
            }

        }

        private void SubPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "请选择主资源路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SubPathText.AppendText($"{dialog.SelectedPath}\r\n");
                modified = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MainPathText.Text))
            {
                System.Windows.MessageBox.Show("主资源路径不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TryCreateMainDirectory(MainPathText.Text))
            {
                return;
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
            modified = false;
        }

        private bool TryCreateMainDirectory(string mainPath)
        {
            if (!Directory.Exists(mainPath))
            {
                try
                {
                    Directory.CreateDirectory(mainPath);
                    _logger.Log(this, "主资源路径不存在，已自动创建");
                }
                catch (DirectoryNotFoundException)
                {
                    System.Windows.MessageBox.Show("主资源路径不存在且无法创建，请检查路径是否合法。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                catch (IOException)
                {
                    System.Windows.MessageBox.Show("文件名、目录名或卷标语法不正确，请检查路径是否合法。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private void InitMainPathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TryCreateMainDirectory(MainPathText.Text))
            {
                return;
            }

            for (var i = LevelTypeInfo.MinTypeIndex; i <= LevelTypeInfo.MaxTypeIndex; i++)
            {
                var level = LevelTypeInfo.GetType(i);

                CreateDir(level, TypeType.Clean);
                CreateDir(level, TypeType.CleanMask);
                CreateDir(level, TypeType.Mosaic);
            }

            void CreateDir(LevelType levelName, TypeType typeName)
            {
                var dir = Path.Combine(MainPathText.Text, FileStrUtility.GetMainDirName(levelName, typeName));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            _logger.Log(this, "初始化主资源路径完成。");
            var re = System.Windows.MessageBox.Show("初始化主资源路径完成，是否打开路径。", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (re == MessageBoxResult.Yes)
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(MainPathText.Text)
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }

        }
    }
}
