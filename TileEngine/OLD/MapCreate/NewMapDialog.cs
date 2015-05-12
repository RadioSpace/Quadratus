using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapCreate
{
    


    public partial class NewMapDialog : Form
    {

        public int NewMapWidth
        {
            get
            {
                int num;
                if (int.TryParse(widthTextBox.Text, out num)) return num;
                else return 0;
            }
        }

        public int NewMapHeight
        {

            get 
            {
                int num;
                if (int.TryParse(heightTextBox.Text, out num)) return num;
                else return 0; 
            }
        }


        public NewMapDialog()
        {
            InitializeComponent();
        }

        private void NewMapDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
