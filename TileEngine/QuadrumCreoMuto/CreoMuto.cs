using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace QuadrumCreoMuto
{
    public partial class CreoMuto : Form
    {

        SharpDX.Direct3D11.Device d;
        GraphicsDevice gd;
        SpriteBatch sb;

        
      

        public CreoMuto()
        {
            InitializeComponent();
        }
    }
}
