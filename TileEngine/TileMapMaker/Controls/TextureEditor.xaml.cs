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

using STAR;

namespace TileMapMaker.Controls
{
    /// <summary>
    /// Interaction logic for TextureEditor.xaml
    /// </summary>
    public partial class TextureEditor : UserControl
    {
        TextureData[] texturedata;
        BitmapImage spritesheet;
        SharpDX.Size2 texsize;

        /// <summary>
        /// gets the index selected in the list
        /// </summary>
        public int SelectedIndex { get { try { return ((TextureData)TextureList.SelectedItem).Index; } catch { return -1; } } }



        public TextureEditor(TextureData[] data, BitmapImage sheet, SharpDX.Size2 ts)
        {
            InitializeComponent();
            
            spritesheet = sheet;
            texsize = ts;
            texturedata = data;

            TextureList.DataContext = texturedata;
        }

        private void TextureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextureList.SelectedIndex > -1)
            {
                TextureData td = texturedata[TextureList.SelectedIndex];

                CroppedBitmap cb = new CroppedBitmap(
                    spritesheet,
                    new Int32Rect(
                       (int)(td.Texcoord.u * spritesheet.PixelWidth),
                       (int)(td.Texcoord.v * spritesheet.PixelHeight),
                       texsize.Width,
                       texsize.Height
                    )
                );

                TextureViewer.BeginInit();
                TextureViewer.Source = cb;
                TextureViewer.EndInit();
            }
            else
            {
                TextureViewer.BeginInit();
                TextureViewer.Source = null;
                TextureViewer.EndInit();
            }
        }
    }
}
