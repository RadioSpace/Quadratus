using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STAR
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential,Size=28)]
    public struct Surface
    {
        /// <summary>
        /// the amount to translate the model position by
        /// </summary>
        public SharpDX.Vector3 trans;

        /// <summary>
        /// the color of the new vertex
        /// </summary>
        public SharpDX.Vector3 color;

        /// <summary>
        /// the index number of the Texture to use
        /// </summary>
        public uint texindex;


        public Surface(SharpDX.Vector3 t, SharpDX.Vector3 c,uint tindex)
        {
            trans = t;
            color = c;
            texindex = tindex;
        }

        public override string ToString()
        {
            return "index = " + texindex;
        }
    }

    

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Size = 96)]
    public struct VArgs
    {
        public SharpDX.Matrix world;//64
        public SharpDX.Vector3 glbTrans;//12
        public float cs;//cell size//4
        public SharpDX.Vector2 texcoordbase;//8
    }
}

