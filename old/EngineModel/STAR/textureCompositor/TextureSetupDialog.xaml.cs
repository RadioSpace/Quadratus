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

namespace textureCompositor
{
    /// <summary>
    /// Interaction logic for TextureSetupDialog.xaml
    /// </summary>
    public partial class TextureSetupDialog : Window
    {

        public float CellWidth 
        { 
            get 
            {
                float result;
                if(float.TryParse(cellwidthTextBox.Text,out result)) return result;
                else return 0f;
            }
        
        }

        public float CellHeight
        {
            get
            {
                float result;
                if (float.TryParse(cellheightTextBox.Text, out result)) return result;
                else return 0f;
            }

        }

        public float TexWidth
        {
            get
            {
                float result;
                if (float.TryParse(texwidthTextBox.Text, out result)) return result;
                else return 0f;
            }

        }

        public float TexHeight
        {
            get
            {
                float result;
                if (float.TryParse(texheightTextBox.Text, out result)) return result;
                else return 0f;
            }

        }

        public TextureSetupDialog()
        {
            InitializeComponent();
        }

        private void okayButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
