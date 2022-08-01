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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibVLCSharp.Shared;

namespace Vicold.Utility.FileUtilities.FCUtility.Views.Pages
{
    /// <summary>
    /// VideoPage.xaml 的交互逻辑
    /// </summary>
    public partial class VideoPage : Page
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mp;
        
        public VideoPage()
        {
            InitializeComponent();
            _libVLC = new LibVLC();
            _mp = new MediaPlayer(_libVLC);
            VideoView.MediaPlayer = _mp;
        }

        public void Open()
        {
        }

        private void DummyGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void DummyGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
