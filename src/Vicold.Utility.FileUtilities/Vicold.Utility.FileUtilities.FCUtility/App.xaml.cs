using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Vicold.Utility.FileUtilities.FCUtility.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Vicold.Utility.FileUtilities.FCUtility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // singleton: App.Current

        public App() : base()
        {
            PanelLogger = new Logger();
            DataCore = new CoreHandler(PanelLogger);
            Window = new MainWindow(DataCore, PanelLogger);
        }

        internal static new App Current { get => (Application.Current as App) ?? throw new Exception("undefined App"); }

        internal OrderCommandHandler OrderCommand { get; } = new OrderCommandHandler();

        internal Logger PanelLogger { get; private set; }

        internal CoreHandler DataCore { get; private set; }

        internal MainWindow Window { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            this.MainWindow = Window;
            this.MainWindow.Show();
        }


    }
}
