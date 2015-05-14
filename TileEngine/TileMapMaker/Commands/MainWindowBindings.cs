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
    public partial class MainWindow : Window
    {



        void SetCommandBindings()
        {
            //change texture

            CommandBindings.Add(new CommandBinding
                (
                    Commands.MapEditCommands.ChangeTexture,
                    TextureChangeOperation,
                    (sender, args) => { args.CanExecute = true; }
                ));
        }


        void TextureChangeOperation(object sender,ExecutedRoutedEventArgs args )
        {
            MessageBox.Show("works");
        }

       


    }
}
