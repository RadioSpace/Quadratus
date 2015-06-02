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

using Microsoft.Win32;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

using STAR;



namespace TileMapMaker
{
    public partial class MainWindow : Window
    {

        void SetCommandBindings()
        {
            

            CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Save,
                    ApplicationSaveOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState == ProjectState.Unsaved; }
                ));

            CommandBindings.Add(
                new CommandBinding(
                    ApplicationCommands.Open,
                    ApplicationOpenOperation,
                    (Sender, args) => { args.CanExecute = (App.ProjectState == ProjectState.Saved || App.ProjectState == ProjectState.Empty); }
                ));

            CommandBindings.Add(
                 new CommandBinding(
                    ApplicationCommands.New,
                    ApplicationNewOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState == ProjectState.Saved || App.ProjectState == ProjectState.Empty; }
                 ));



            CommandBindings.Add(
                new CommandBinding(
                    Commands.MapEditCommands.ChangeTexture,
                    ChangeTextureOperation,
                    (sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; ; }
                ));

            CommandBindings.Add(
                new CommandBinding(
                    Commands.MapEditCommands.ChangeColor,
                    ChangeSizeOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; ; }
                 ));

            CommandBindings.Add(
                new CommandBinding(
                    Commands.MapEditCommands.ChangePosition,
                    ChangePositionOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; }
                ));



            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.Createmap,
                    CreateMapOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; }
                 ));
            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.ImportMap,
                    ImportMapOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; }
                 ));
            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.RemoveMap,
                    RemoveMapOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty && MapListBox.SelectedIndex > -1; }
                 ));
            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.ExportMap,
                    ExportMapOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty && MapListBox.SelectedIndex > -1; }
                 ));
            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.ClearProject,
                    ClearProjectOperation,
                    (Sender, args) => { args.CanExecute = App.ProjectState != ProjectState.Empty; }
                 ));

            CommandBindings.Add(
                 new CommandBinding(
                    Commands.MapEditCommands.SelectMap,
                    SelectMapOperation,
                    (Sender, args) => { args.CanExecute =  App.ProjectState != ProjectState.Empty && MapListBox.SelectedIndex > -1; }
                 ));
        }


        private void SelectMapOperation(object sender, ExecutedRoutedEventArgs e)
        {
            
            
            GameMap gm = shell.SelectMap((string)MapListBox.SelectedItem);
       


            foreach (TextureData td in gm.TDC)
            {
                texturedata.Add(td);
            }
           
            bi = new BitmapImage(new Uri(gm.getPNGPath()));

            texsize = new SharpDX.Size2((int)(bi.PixelWidth * gm.TDC.CellUnit.u), (int)(bi.PixelHeight * gm.TDC.CellUnit.v));

            if (EditorControl.Children.Count > 0)
            {
                if (EditorControl.Children[0] is Controls.TextureEditor)
                {
                    Controls.TextureEditor te = (Controls.TextureEditor)EditorControl.Children[0];
                    te.Reset();


                }
            }
            
        }

        private void ClearProjectOperation(object sender, ExecutedRoutedEventArgs e)
        {
            shell.PauseGameToggle();


            

            Dialogs.InitializeProjectDialog ipd = new Dialogs.InitializeProjectDialog();
            if (ipd.ShowDialog() ?? false)
            {
                shell.ClearProject();
                switch (ipd.initializeProjectDialogResult)
                { 
                    case Dialogs.InitializeProjectDialogResult.ImportMap:

                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Title = "Choose Map to Import";
                        ofd.Filter = "Game Map|*.map";
                        ofd.CheckFileExists = true;
                        ofd.CheckPathExists = true;
                        ofd.DefaultExt = ".gmap";
                        ofd.Multiselect = true;
                        

                        if (ofd.ShowDialog() ?? false)
                        {
                            foreach (string path in ofd.FileNames)
                            { 


                                
                            }
                        }

                        break;
                    
                    
                    case Dialogs.InitializeProjectDialogResult.NewMap:
                        Dialogs.NewMapDialog nmd = new Dialogs.NewMapDialog();

                        if (nmd.ShowDialog() ?? false)
                        { 
                           GameMap gm = new GameMap(nmd.NewMapCmpPath,nmd.NewMapWidth,nmd.NewMapHeight);

                           string newname = Guid.NewGuid().ToString("N");
                           shell.UploadMap(newname,gm);
                           shell.SelectMap(newname);
                        }

                        App.ProjectState = ProjectState.Saved;

                        break;
                    
                    
                    case Dialogs.InitializeProjectDialogResult.None:
                    default:


                        break;
                }


            }
            else { }


            shell.PauseGameToggle();
            
        }

        private void ExportMapOperation(object sender, ExecutedRoutedEventArgs e)
        {
            //save the gamemap

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Map";
            sfd.Filter = "Game Map|*.gmap";
            sfd.DefaultExt = ".gmap";

            if (sfd.ShowDialog() ?? false)
            {
                File.WriteAllBytes(sfd.FileName, shell.SaveMap());

            }


            MessageBox.Show("Map Saved");


            App.ProjectState = ProjectState.Saved;
        }

        private void RemoveMapOperation(object sender, ExecutedRoutedEventArgs e)
        {
            shell.PauseGameToggle();

            shell.RemoveSelectedMap();
            FillNames();

            shell.PauseGameToggle();

        }

        private void ImportMapOperation(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Game Map|*.gmap";
            ofd.Title = "find a map to load";
            ofd.Multiselect = false;
            ofd.DefaultExt = ".gmap";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.AddExtension = true;



            if (ofd.ShowDialog() ?? false)
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(ofd.FileName)))
                {
                    try
                    {
                        shell.PauseGameToggle();

                        shell.ClearProject();

                        string newmapname = Guid.NewGuid().ToString("N");
                        GameMap map = (GameMap)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(ms);
                        bool maploaded = shell.UploadMap(newmapname, map);

                        shell.SelectMap(newmapname);

                        shell.PauseGameToggle();

                        if (maploaded)
                        {

                        }
                        else MessageBox.Show("could not load map"); maploaded = false;


                        App.ProjectState = ProjectState.Saved;
                    }
                    catch (Exception EX)
                    {
                        MessageBox.Show("could not open map : " + EX.Message);

                    }
                }
            }

            FillNames();
        }

        private void CreateMapOperation(object sender, ExecutedRoutedEventArgs e)
        {
            Dialogs.NewMapDialog nmd = new Dialogs.NewMapDialog();
            if (nmd.ShowDialog() ?? false)
            {
                shell.PauseGameToggle();              


                GameMap map = new GameMap(nmd.NewMapCmpPath, nmd.NewMapWidth, nmd.NewMapHeight);
                bool maploaded = false;


                string newmapname = Guid.NewGuid().ToString("N");
                maploaded = shell.UploadMap(newmapname, map);
                shell.SelectMap(newmapname);

                shell.PauseGameToggle();


                if (maploaded)
                {
                    TextureDataCollection tdc;
                    try { tdc = TextureDataCollection.ReadCollection(map.GetCMPPath()); }
                    catch { tdc = new TextureDataCollection(); }

                    foreach (TextureData td in tdc)
                    {
                        texturedata.Add(td);
                    }

                    bi = new BitmapImage(new Uri(map.getPNGPath()));

                    texsize = new SharpDX.Size2((int)(bi.PixelWidth * tdc.CellUnit.u), (int)(bi.PixelHeight * tdc.CellUnit.v));
                }
                else MessageBox.Show("could not load map"); maploaded = false;



                App.ProjectState = ProjectState.Saved;
            }

            FillNames();
        }
        


        void ApplicationNewOperation(object sender, ExecutedRoutedEventArgs e)
        {

            //new project 

          

            Dialogs.InitializeProjectDialog ipd = new Dialogs.InitializeProjectDialog();
            if (ipd.ShowDialog() ?? false)
            {
                shell.PauseGameToggle();
                shell.ClearProject();

                

                switch (ipd.initializeProjectDialogResult)
                { 
                    case  Dialogs.InitializeProjectDialogResult.ImportMap:
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Game Map|*.gmap";
                        ofd.Title = "find a map to load";
                        ofd.Multiselect = false;
                        ofd.DefaultExt = ".gmap";
                        ofd.CheckFileExists = true;
                        ofd.CheckPathExists = true;
                        ofd.AddExtension = true;



                        if (ofd.ShowDialog() ?? false)
                        {
                            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(ofd.FileName)))
                            {
                                try
                                {
                                    string newmapname = Guid.NewGuid().ToString("N");
                                    GameMap map = (GameMap)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(ms);
                                    bool maploaded = shell.UploadMap(newmapname, map);

                                    shell.SelectMap(newmapname);



                                    if (maploaded)
                                    {

                                    }
                                    else MessageBox.Show("could not load map"); maploaded = false;


                                    App.ProjectState = ProjectState.Saved;
                                }
                                catch (Exception EX)
                                {
                                    MessageBox.Show("could not open map : " + EX.Message);

                                }
                            }
                        }
                         
                        break;

                    case Dialogs.InitializeProjectDialogResult.NewMap:
                        Dialogs.NewMapDialog nmd = new Dialogs.NewMapDialog();
                         if (nmd.ShowDialog() ?? false)
                         {
                             


                             GameMap map = new GameMap(nmd.NewMapCmpPath, nmd.NewMapWidth, nmd.NewMapHeight);
                             bool maploaded = false;


                             string newmapname = Guid.NewGuid().ToString("N");
                             maploaded = shell.UploadMap(newmapname, map);
                             shell.SelectMap(newmapname);

                            
                         }
                        break;

                    case Dialogs.InitializeProjectDialogResult.None:

                    default:
                        break;
                }
            }





            shell.PauseGameToggle();

            FillNames();

            App.ProjectState = ProjectState.Saved;

        }

        void ApplicationOpenOperation(object sender, ExecutedRoutedEventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open a Game Project";
            ofd.DefaultExt = ".gpro";
            ofd.Filter = "Game Project|*.gpro";
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            

            if(ofd.ShowDialog() ?? false)
            {
                shell.PauseGameToggle();
                shell.ClearProject();

                try
                {
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(ofd.FileName)))
                    {
                        shell.SetGameProject((GameProject)new BinaryFormatter().Deserialize(ms));
                    }
                }
                catch (Exception EX) { MessageBox.Show("could not load project because " + EX.Message + "\r\nFailed At:\r\n" + EX.StackTrace); }

                FillNames();

                shell.PauseGameToggle();

                App.ProjectState = ProjectState.Saved;
                comMode = Commands.MapCommandMode.None;
            }


            //
            
            
            

        }        

        void ApplicationSaveOperation(object Sender,ExecutedRoutedEventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save your project";
            sfd.Filter = "Game Project|*.gpro";
            sfd.CheckFileExists = false;
            sfd.CheckPathExists = true;
            sfd.DefaultExt = ".gpro";

            if (sfd.ShowDialog() ?? false)
            {
                File.WriteAllBytes(sfd.FileName, shell.SaveProject());
            }
        }       
        


        void ChangePositionOperation(object Sender, ExecutedRoutedEventArgs args)
        {
            EditorControl.Children.Clear();
            EditorControl.Children.Add(new Controls.PositionEditor());

            comMode = Commands.MapCommandMode.ChangePosition;
        }        

        void ChangeSizeOperation(object Sender, ExecutedRoutedEventArgs args)
        {

            EditorControl.Children.Clear();
            EditorControl.Children.Add(new Controls.ColorEditor());

            comMode = Commands.MapCommandMode.ChangeColor;


        }

        void ChangeTextureOperation(object sender, ExecutedRoutedEventArgs args)
        {
            EditorControl.Children.Clear();


            EditorControl.Children.Add(new Controls.TextureEditor(texturedata.ToArray(), bi, texsize));


            //set the command mode of the game shell
            comMode = Commands.MapCommandMode.ChangeTexture;



        }
    }
}
