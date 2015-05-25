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

using Microsoft.Win32;

namespace TileMapMaker
{
    /// <summary>
    /// Interaction logic for NewMapDialog.xaml
    /// </summary>
    public partial class NewMapDialog : Window
    {

        public string NewMapCmpPath { get { return PathInput.Text; } }
        public int NewMapWidth
        {
            get
            {
                int result;
                if (int.TryParse(WidthInput.Text, out result))
                {
                    return result;
                }
                else 
                {
                    return 0;
                }                
            }
        }

        public int NewMapHeight
        {
            get
            {
                int result;
                if (int.TryParse(HeightInput.Text, out result))
                {
                    return result;
                }
                else
                {
                    return 0;
                }
            }
        }

        


        public NewMapDialog()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.DefaultExt = ".cmp";
            ofd.Filter = "Composite Texture File|*.cmp";
            ofd.Multiselect = false;
            ofd.Title = "attach a Composite Texture";

            if (ofd.ShowDialog() ?? false)
            {
                PathInput.Text = ofd.FileName;
            }            
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = !string.IsNullOrWhiteSpace(NewMapCmpPath);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

        }
    }
}
