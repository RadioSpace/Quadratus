using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Linq;
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
        
        BitmapImage bi;
        SharpDX.Size2 texsize;

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
            

            
            shell = new GameShell(new GameMap("dev1.cmp",1,1), true);
            shell.TopLevel = false;
            shell.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            shell.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            shell.AllowTransparency = true;
            shell.GameClick += shell_GameClick;
            shell.GameMouseMove += shell_GameMouseMove;

            WinHost.Child = shell;


            App.ProjectState = ProjectState.Empty;


            SetCommandBindings();//from Commands\MainWindowBindings.cs
        }

        private void shell_GameMouseMove(object sender, GameShellMouseEventArgs e)
        {
            switch (comMode)
            { 
                case Commands.MapCommandMode.ChangeColor:
                    break;
                
                case Commands.MapCommandMode.ChangePosition:
                    break;
                
                case Commands.MapCommandMode.ChangeTexture:

                    ChangeTexture(e);
                    
                    break;
                
                case Commands.MapCommandMode.None:
                default:


                    break;
            }
        }

        void shell_GameClick(object sender, GameShellMouseEventArgs e)
        {
            switch (comMode)
            {
                case Commands.MapCommandMode.ChangeColor:
                    break;

                case Commands.MapCommandMode.ChangePosition:


                    break;

                case Commands.MapCommandMode.ChangeTexture:

                    ChangeTexture(e);

                    break;

                case Commands.MapCommandMode.None:
                default:


                    break;
            }

        }

        private void ChangeTexture(GameShellMouseEventArgs e)
        {
            if (EditorControl.Children.Count > 0)
            {

                Controls.TextureEditor te;

                try { te = (Controls.TextureEditor)EditorControl.Children[0]; }
                catch { te = null; }

                if (te != null)
                {
                    if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        int index = te.SelectedIndex;

                        if (index > -1)
                        {
                            e.SetGameSurface(new Surface(e.surfaceFromGame.trans, SharpDX.Vector3.One, (uint)index));//test code                                
                        }
                    }
                }
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
