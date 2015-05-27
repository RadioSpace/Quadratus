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

using SharpDX;


namespace TileMapMaker.Controls
{
    using Color = SharpDX.Color;


    /// <summary>
    /// Interaction logic for ColorEditor.xaml
    /// </summary>
    public partial class ColorEditor : UserControl
    {
        public Color SelectedColor { get { return Color.FromRgba(BitConverter.ToInt32(new byte[]{(byte)((redval.Value / 100000000.0) * 255), (byte)((greenval.Value / 100000000.0) * 255), (byte)((blueval.Value / 100000000.0) * 255),255},0)); } }
        public Vector3 SelectedColorVector3 { get { return new Vector3((float)(redval.Value / 100000000.0), (float)(greenval.Value / 100000000.0), (float)(blueval.Value / 100000000.0)); } }
        


        public ColorEditor()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            newcolor.Color = System.Windows.Media.Color.FromRgb(SelectedColor.R, SelectedColor.G, SelectedColor.B);
        }
    }
}
