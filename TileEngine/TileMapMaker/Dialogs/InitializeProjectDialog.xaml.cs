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
using System.Windows.Shapes;

namespace TileMapMaker.Dialogs
{
    public enum InitializeProjectDialogResult { None,NewMap,ImportMap}

    /// <summary>
    /// Interaction logic for InitializeProjectDialog.xaml
    /// </summary>
    public partial class InitializeProjectDialog : Window
    {
        public InitializeProjectDialogResult initializeProjectDialogResult =  InitializeProjectDialogResult.None;

        public InitializeProjectDialog()
        {
            InitializeComponent();
        }

        private void newMapButton_Click(object sender, RoutedEventArgs e)
        {
            initializeProjectDialogResult = InitializeProjectDialogResult.NewMap;
            DialogResult = true;

        }

        private void importMapButton_Click(object sender, RoutedEventArgs e)
        {
            initializeProjectDialogResult = InitializeProjectDialogResult.ImportMap;
            DialogResult = true;
        }



        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = false;
        }


    }
}
