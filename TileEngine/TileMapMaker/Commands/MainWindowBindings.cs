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

            CommandBindings.Add(new CommandBinding(
                Commands.MapEditCommands.ChangeTexture,
                TextureChangeOperation,
                (sender, args) => { args.CanExecute = true; }
                ));

            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Save,
                ApplicationSaveOperation,
                (Sender, args) => { args.CanExecute = true; }
                ));

            CommandBindings.Add(new CommandBinding(
                Commands.MapEditCommands.ChangeSize,
                ChangeSizeOperation,
                (Sender, args) => { args.CanExecute = true; }
                ));
            CommandBindings.Add(new CommandBinding(
                Commands.MapEditCommands.Lock,
                LockOperation,
                (Sender, args) => { args.CanExecute = true; }
                ));
        }


        void TextureChangeOperation(object sender,ExecutedRoutedEventArgs args )
        {

            ElementsList.Items.Clear();

            foreach (TextureData td in texturedata)
            {
                ElementsList.Items.Add(td.Index.ToString());
            }

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

                using (FileStream fs = File.Open(sfd.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    Stream s = shell.SaveMap();

                    s.CopyTo(fs);                    
                }

            }
            
            
            MessageBox.Show("Map Saved");


            App.ProjectState = ProjectState.Saved;
        }

        void LockOperation(object Sender, ExecutedRoutedEventArgs args)
        {
            App.ProjectState = ProjectState.ReadOnly;
        }

        void ChangeSizeOperation(object Sender, ExecutedRoutedEventArgs args)
        {
            App.ProjectState = ProjectState.Unsaved;
        }

       


    }
}
