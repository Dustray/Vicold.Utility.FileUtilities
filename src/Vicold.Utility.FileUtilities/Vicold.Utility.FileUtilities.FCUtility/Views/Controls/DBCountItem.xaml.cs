using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Vicold.Utility.FileUtilities.FCUtility.Views.ViewModels;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Controls
{
    /// <summary>
    /// DBCountItem.xaml 的交互逻辑
    /// </summary>
    public partial class DBCountItem : UserControl
    {
        private DBCountItemVM _vm;
        internal DBCountItem(DBCountItemVM vm)
        {
            InitializeComponent();
            _vm = vm;
            this.DataContext = vm;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.Path is { } && _vm.IsEnable)
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(_vm.Path)
                    {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
        }
    }
}
