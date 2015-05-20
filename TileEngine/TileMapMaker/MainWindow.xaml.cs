using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
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

       

        ObservableCollection<TextureData> texturedata;

        Commands.MapCommandMode comMode = Commands.MapCommandMode.None;
       

        public MainWindow()
        {
            InitializeComponent();

            cellProps = new ContextMenu();

            texturedata = new ObservableCollection<TextureData>();
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

                 GameMap map = new GameMap(ofd.FileName, 20, 20);
                
                shell = new GameShell(map,true);
                shell.TopLevel = false;
                shell.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                shell.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                shell.AllowTransparency = true;                
                shell.GameClick += shell_GameClick;
                
                WinHost.Child = shell;

                TextureDataCollection tdc = TextureDataCollection.ReadCollection(map.TextureDataPath);

                foreach (TextureData td in tdc)
                {
                    texturedata.Add(td);
                }
                
                


                SetCommandBindings();
            }
            else
            {//close for now 
                
                Close();
            }
        }

        void shell_GameClick(object sender, GameShellMouseClickEventArgs e)
        {
            switch (comMode)
            { 
                case Commands.MapCommandMode.None:


                    break;


                case Commands.MapCommandMode.ChangeTexture:

                    if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        ResetCellProps(e.surfaceFromGame.color.ToString(), e.surfaceFromGame.texindex.ToString(), e.surfaceFromGame.trans.ToString());
                        cellProps.IsOpen = true;
                    }
                    else
                    {

                        if (ElementsList.SelectedIndex > -1)
                        {
                            int index;
                            if (int.TryParse((string)ElementsList.SelectedItem,out index))
                            {
                                e.SetGameSurface(new Surface(e.surfaceFromGame.trans, SharpDX.Vector3.One, (uint)Math.Abs(index)));//test code
                            }
                        }
                    }
            

                    break;



                default:
                    break;                   
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
