using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using STAR;

namespace MapCreate
{
    public partial class MapAnchor : Form
    {
        MapProject project;

        

        public MapAnchor()
        {
            InitializeComponent();


        }

        private void MapAnchor_Load(object sender, EventArgs e)
        {


            project = new MapProject("C:\\Users\\daniel\\Desktop\\Eat the fail.cmp", 2, 2, 24);





        }

        private void button1_Click(object sender, EventArgs e)
        {
            //i don't know if I want to have the GameShell know about the MapProject Type just yet.
            //MapProject is designed after the gameshells needs but they don't rely on each other.


        }


    }
}
