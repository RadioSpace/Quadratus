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

using System.Windows.Forms.Integration;

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

            cellProps = new ContextMenu();
            
            
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

                GameMap map = new GameMap(ofd.FileName, 10, 10);
                
                shell = new GameShell(map,true);
                shell.TopLevel = false;
                shell.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                shell.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                shell.AllowTransparency = true;                
                shell.GameClick += shell_GameClick;
                
                WinHost.Child = shell;   

                SetCommandBindings();
            }
            else
            {//close for now 
                
                Close();
            }
        }

        void shell_GameClick(object sender, GameShellMouseClickEventArgs e)
        {
            
            if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ResetCellProps(e.surfaceFromGame.color.ToString(), e.surfaceFromGame.texindex.ToString(), e.surfaceFromGame.trans.ToString());
                cellProps.IsOpen = true;
            }
            else
            {
               
                e.SetGameSurface(new Surface(e.surfaceFromGame.trans, SharpDX.Vector3.One,1));//test code
            }



        }


        void ResetCellProps(params string[] props)
        {
            cellProps.Items.Clear();

            foreach(string prop in props)
            {
                cellProps.Items.Add(prop);
            }
        }

        

        private void Window_Closed(object sender, EventArgs e)
        {
            shell.Close();
            shell.Dispose();
        }

        private void cellProps_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }


    }
}
