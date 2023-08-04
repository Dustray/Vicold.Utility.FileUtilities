using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// BaseTabPage.xaml 的交互逻辑
    /// </summary>
    public partial class BaseTabPage : Page, IFuncPage
    {
        private Dictionary<string, Page> _pages;
        private Dictionary<string, Button> _buttons;
        private string _selectedPage = string.Empty;

        public BaseTabPage(Dictionary<string, Page> pages)
        {
            InitializeComponent();
            _pages = pages;
            _buttons = new Dictionary<string, Button>();

            // 添加按钮
            int count = _pages.Count;
            int index = 0;
            foreach (var kv in _pages)
            {
                var btn = new Button { Content = kv.Key };
                btn.Click += Btn_Click;
                btn.Style = FindResource("TabButtonUnSelectedStyle") as Style;
                TabTitle.Children.Add(btn);
                _buttons.Add(kv.Key, btn);
                if (index != count - 1)
                {
                    var line = new Rectangle
                    {
                        Width = 1,
                        Height = 20,
                        Fill = new SolidColorBrush(Colors.Gray),
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    TabTitle.Children.Add(line);
                }
                index++;
            }

            // 默认显示第一页
            if (_pages.Count > 0)
            {
                _selectedPage = _pages.First().Key;
               // SelectedPage(_selectedPage);
            }
        }

        public string FuncTitle { get; } = "搜索";

        public void Reflush()
        {
            SelectedPage(_selectedPage);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var name = btn?.Content.ToString();
            if (name is { })
            {
                SelectedPage(name);
            }
        }

        private void SelectedPage(string pageName)
        {
            if (_selectedPage != null)
            {
                _buttons[_selectedPage].Style = FindResource("TabButtonUnSelectedStyle") as Style;
            }

            _selectedPage = pageName;
            _buttons[pageName].Style = FindResource("TabButtonSelectedStyle") as Style;
            MainFrame.Navigate(_pages[pageName]);
        }

    }
}
