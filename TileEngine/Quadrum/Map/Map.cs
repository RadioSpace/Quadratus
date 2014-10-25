using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Direct3D11;


namespace Quadrum.Map
{

    //class that manages the Map and is used by the map renderer for drawing
    [Serializable]
    public class Map
    {
        SpriteSheet.SpriteSheet mapsheet;
        public SpriteSheet.SpriteSheet MapSheet { get { return mapsheet; } }
        
        Grid mapgrid;
        public Grid MapGrid { get { return mapgrid; } }

      
        public Map(Device d,string MapSheetPath, Grid mgrid)
        {
            mapsheet = SpriteSheet.SpriteSheet.Load( MapSheetPath);
            if (mapsheet.Name == "FAILED")
            {
                throw new Exception("Loading the prite sheet failed");                
            }
            
            mapgrid = mgrid;
        }


        
    }
}
