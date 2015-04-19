using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using System.IO;

using SharpDX;


namespace textureCompositor
{
    using Path = System.IO.Path;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> texturepaths;
        public List<string> localtexpaths;


        public MainWindow()
        {
            InitializeComponent();

            texturepaths = new ObservableCollection<string>();
            
            textureList.DataContext = texturepaths;
            textureList.SelectionChanged += textureList_SelectionChanged;

            localtexpaths = new List<string>();
        }

        void textureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //get string from list
            string path = (string)textureList.SelectedItem;


            BitmapImage bi = new BitmapImage(new Uri(path));
            textureImage.Source = bi;
        

        }



        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog(); 
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;            
            ofd.Multiselect = true;
            ofd.Filter = "PNG files|*.png";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//this needs changed
            ofd.Title = "choose your textures to composite";


            if (ofd.ShowDialog() ?? false)
            {
                foreach (string path in ofd.FileNames)
                {
                    string lpath = Path.GetFileName(path);

                    File.Copy(path, lpath,true);
                    localtexpaths.Add(lpath);
                    texturepaths.Add(path);
                }
            }
            
        }

        private void newMI_Click(object sender, RoutedEventArgs e)
        {

        }

        private void savMI_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.CheckPathExists = true;
            sfd.Filter = "Composite Texture Data|*.cmp";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sfd.Title = "choose a destinatino for your texture and data";
           

            if (sfd.ShowDialog()??false)
            {
                TextureSetupDialog tsd = new TextureSetupDialog();
                if (tsd.ShowDialog() ?? false)
                {
                    Compositer comp = new Compositer((int)tsd.TexWidth, (int)tsd.TexHeight, (int)tsd.CellWidth, (int)tsd.CellHeight);

                    comp.TargetSizeWarning += comp_TargetSizeWarning;

                    comp.Composite(sfd.FileName, localtexpaths.ToArray());

                    comp.Dispose();
                }

                
            }



        }

        void comp_TargetSizeWarning(object sender, EventArgs e)
        {
            MessageBox.Show("size warning!");//this is stupid
        }

        private void opnMI_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
