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

            GameProject project = new GameProject();
            project.AddMap("default",new GameMap("dev1.cmp",1,1));

            
            shell = new GameShell(project, true);
            shell.SelectMap("default");
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
                    ChangeColor(e);
                    break;
                
                case Commands.MapCommandMode.ChangePosition:
                    ChangePosition(e);
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

                    ChangeColor(e);

                    break;

                case Commands.MapCommandMode.ChangePosition:

                    ChangePosition(e);

                    break;

                case Commands.MapCommandMode.ChangeTexture:

                    ChangeTexture(e);

                    break;

                case Commands.MapCommandMode.None:
                default:


                    break;
            }

        }

        private void ChangePosition(GameShellMouseEventArgs e)
        {
            if (EditorControl.Children.Count > 0)
            {
                if (EditorControl.Children[0] is Controls.PositionEditor)
                {
                    if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        //we are not ready because of the way the editor finds the cells it is editing 
                        //will have to find an nice way to find the cells by there Translation

                        //Controls.PositionEditor pe =  (Controls.PositionEditor)EditorControl.Children[0];


                        //do nothing
                        
                    }

                }


                App.ProjectState = ProjectState.Unsaved;
            }

            
        }

        private void ChangeColor(GameShellMouseEventArgs e)
        {
            if (EditorControl.Children.Count > 0)
            {

                
                if(EditorControl.Children[0] is Controls.ColorEditor)
                {
                    

                    if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        Controls.ColorEditor ce = (Controls.ColorEditor)EditorControl.Children[0]; 

                        e.SetGameSurface(new Surface(e.surfaceFromGame.HasValue ? e.surfaceFromGame.Value.trans : SharpDX.Vector3.Zero, ce.SelectedColorVector3, e.surfaceFromGame.HasValue ? e.surfaceFromGame.Value.texindex : 0));//test code
                        App.ProjectState = ProjectState.Unsaved;

                    }
                }
            }
        }

        private void ChangeTexture(GameShellMouseEventArgs e)
        {
            if (EditorControl.Children.Count > 0)
            {                

                if(EditorControl.Children[0] is Controls.TextureEditor)
                {
                    if (e.MouseArgs.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        Controls.TextureEditor te = (Controls.TextureEditor)EditorControl.Children[0];    

                        int index = te.SelectedIndex;

                        if (index > -1)
                        {
                            e.SetGameSurface(new Surface(e.surfaceFromGame.HasValue ? e.surfaceFromGame.Value.trans:SharpDX.Vector3.Zero, SharpDX.Vector3.One, (uint)index));//test code
                            App.ProjectState = ProjectState.Unsaved;
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
