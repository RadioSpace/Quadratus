using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using STAR;

namespace TileMapMaker
{
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameShell shell;
        
       

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = ".cmp";
            ofd.Filter = "Composite Texture Data|*.cmp";
            ofd.Multiselect = false;
            ofd.Title = "select the TextureData to use with the map";

            if (ofd.ShowDialog() ?? false)
            {

                GameMap map = new GameMap(ofd.FileName, 2, 2);
                System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();

                shell = new GameShell(map);
                shell.TopLevel = false;

                host.Child = shell;
                maingrid.Children.Add(host);
                
            }
            else
            {//close for now until we get the game rendering in WPF
                Close();
            }
        }
    }
}
