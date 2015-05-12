using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STAR;
using SharpDX;

namespace MapCreate
{

    [Serializable]
    public class MapSurface
    {
        Surface surface;
        public Vector3 trans { get { return surface.trans; } }
        public Color color { get { return new Color(surface.color); } set { surface.color = value.ToVector3(); } }
        public uint texindex { get { return surface.texindex; } set { surface.texindex = value; } }        
        
        public MapSurface(Surface s)
        {
            surface = s;
            
        }

        public Surface getSurface()
        {
            return surface;
        }


    }
}
