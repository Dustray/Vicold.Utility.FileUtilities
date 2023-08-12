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

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// EditSourcePage.xaml 的交互逻辑
    /// </summary>
    public partial class EditCodePage : Page, IFuncPage
    {
        private CoreHandler _coreHandler;
        private Logger _logger;
        internal EditCodePage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
        }

        public string FuncTitle { get; } = "编辑资源";

        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // 获取输入的数据
            string code = TxtCode.Text;
            string type = CboType.SelectedItem.ToString();
            string actor = CboActor.SelectedItem.ToString();

            // 检查是否为更新
            if (true)
            {
                // 更新逻辑
            }
            else
            {
                // 增加逻辑 
            }
        }
    }
}
