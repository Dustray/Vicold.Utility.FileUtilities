using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
    /// GlobalSearchPage.xaml 的交互逻辑
    /// </summary>
    public partial class GlobalSearchPage : Page, IFuncPage
    {
        class SearchInfo
        {
            public string? Code { get; set; }
            public string? Stars { get; set; }
            public string? Seller { get; set; }
            public string? FilePath { get; set; }
        }

        private CoreHandler _coreHandler;
        private Logger _logger;

        internal GlobalSearchPage(CoreHandler coreHandler, Logger logger)
        {
            InitializeComponent();
            _coreHandler = coreHandler;
            _logger = logger;
        }

        public async void Search(string condition)
        {
            var infos = new List<SearchInfo>();

            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(condition))
                {
                    _logger.Log(this, "请输入搜索内容");
                    return;
                }

                var tables = _coreHandler.DB.SearchLike(condition);
                Dispatcher.Invoke(() =>
                {
                    var result = new List<SearchInfo>();
                    foreach (var table in tables)
                    {
                        var info = new SearchInfo
                        {
                            Code = table.Code.ToString(),
                            Seller = table.GetMovieSeller()?.Name,
                            FilePath = table.FilePath
                        };

                        var stars = table.GetMovieStars();
                        if (stars is { })
                        {
                            info.Stars = string.Join(',', stars.Select(x => x.Name));
                        }

                        result.Add(info);

                    }

                    ResultGrid.ItemsSource = result;
                    SearchText.Focus();
                    SearchText.SelectAll();
                });
            }).ConfigureAwait(false);

        }

        public string FuncTitle { get; } = "全局综合搜索";

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var input = SearchText.Text;

            Search(input);
        }
        private void PasteAndSearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
