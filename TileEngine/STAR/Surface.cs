using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STAR
{
    /// <summary>
    /// teh surface of a cell on a grid
    /// </summary>
    [Serializable]
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential,Size=28)]
    public struct Surface : IEquatable<Surface>
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

        /// <summary>
        /// creates a surface of a cell on a grid
        /// </summary>
        /// <param name="t">the translation of the cell</param>
        /// <param name="c">the color offset of the cell</param>
        /// <param name="tindex">the index of the texture on the sprite sheet to texture this cell</param>
        public Surface(SharpDX.Vector3 t, SharpDX.Vector3 c,uint tindex)
        {
            trans = t;
            color = c;
            texindex = tindex;
        }

        /// <summary>
        /// returns the index number of the texture used and the translation of the cell as text
        /// </summary>
        /// <returns>a string representation of the Surface</returns>
        public override string ToString()
        {
            return "texindex = " + texindex + " :: trans = {" + trans.X + "," + trans.Y + "};";
        }

        #region equality
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + trans.GetHashCode();
            hash = hash * 31 + color.GetHashCode();
            return hash * 31 + texindex.GetHashCode();
        }
        
        public static bool operator ==(Surface s1, Surface s2)
        {
            return s1.Equals(s2);
        }
        public static bool operator !=(Surface s1, Surface s2)
        {
            return !s1.Equals(s2);
        }

        public override bool Equals(object obj)
        {

            if (obj is Surface)
                return Equals((Surface)obj);
            else return false;
        }

        public bool Equals(Surface other)
        {
            return this.color == other.color && this.texindex == other.texindex && this.trans == other.trans;
        }
        #endregion
         
    }

    
    /// <summary>
    /// vertex shader arguments
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Size = 96)]
    public struct VArgs
    {
        public SharpDX.Matrix world;//64
        public SharpDX.Vector3 glbTrans;//12
        public float cs;//cell size//4
        public SharpDX.Vector2 texcoordbase;//8
    }
}

