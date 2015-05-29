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


using STAR;



namespace TileMapMaker
{
    public partial class MainWindow : Window
    {



        void SetCommandBindings()
        {
            //change texture

            CommandBindings.Add(
                new CommandBinding(
                    Commands.MapEditCommands.ChangeTexture,
                    TextureChangeOperation,
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
        }



        void ApplicationNewOperation(object sender, ExecutedRoutedEventArgs e)
        {

            NewMapDialog nmd = new NewMapDialog();
            if (nmd.ShowDialog() ?? false)
            {
                shell.PauseGameToggle();

                shell.ClearProject();


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

        }

        void TextureChangeOperation(object sender,ExecutedRoutedEventArgs args )
        {
            EditorControl.Children.Clear();
            

            EditorControl.Children.Add(new Controls.TextureEditor(texturedata.ToArray(), bi, texsize));


            //set the command mode of the game shell
            comMode = Commands.MapCommandMode.ChangeTexture;

           
           
        }

        void ApplicationSaveOperation(object Sender,ExecutedRoutedEventArgs args)
        {
            //save the game

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Map";
            sfd.Filter = "Game Map|*.gmap";
            sfd.DefaultExt = ".gmap";
            
            if (sfd.ShowDialog() ?? false)
            {
                File.WriteAllBytes(sfd.FileName,shell.SaveMap());             

            }
            
            
            MessageBox.Show("Map Saved");


            App.ProjectState = ProjectState.Saved;
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

        void ApplicationOpenOperation(object sender, ExecutedRoutedEventArgs args)
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

                if (shell == null)
                { 
                    
                }

                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(ofd.FileName)))
                {
                    

                    try
                    {
                        shell.PauseGameToggle();

                        shell.ClearProject();

                        string newmapname = Guid.NewGuid().ToString("N");
                        GameMap map = (GameMap)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(ms);
                        bool maploaded = shell.UploadMap(newmapname,map);

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
                    catch (Exception EX)
                    {
                        MessageBox.Show("could not open map : " + EX.Message);

                    }
                }
            }


            
        }

    }
}
