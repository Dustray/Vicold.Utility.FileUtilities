using System;
using System.Collections.Generic;
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

        public string FuncTitle { get; } = "代码搜索";

        private void FastSearchButton_Click(object sender, RoutedEventArgs e)
        {
            var codeLong = FastSearchText.Text;
            if (string.IsNullOrWhiteSpace(codeLong))
            {
                _logger.Log(this, "请输入搜索内容");
                return;
            }
            var codeStr = FilePathUtility.GetCodeFromLongStr(codeLong);
            if (codeStr is { } && int.TryParse(codeStr, out var code))
            {
                _logger.Log(this, $"查询代码：{codeStr}");
                var info = _coreHandler.DB.Search(code);
                if (info is { })
                {
                    _logger.Log(this, $"数据库中存在此代码，类型为{TypeTypeInfo.GetTypeName(info.Type)}，级别为{LevelTypeInfo.GetLevelName(info.Level)}");
                }
                else
                {
                    _logger.Log(this, "没有搜索到结果");
                }
            }
            else
            {
                _logger.Log(this, "搜索内容不合法，无法检测到代码");
            }
        }
    }
}
