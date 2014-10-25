using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Direct3D11;

namespace Quadrum.Map
{
    using Buffer = SharpDX.Direct3D11.Buffer;

    public class MapRenderer
    {//should just take a device and set the graphics stuff on it
        
        //        
        Map map;
        public Map Map { get { return map; } }

        
        
        Texture2D spritesheet;

        public int[] indices;
        
        Buffer ib;
                
        Buffer surfacedata;
        ShaderResourceView surfacedataview;

        Buffer texturedata;
        ShaderResourceView texturedataview;

        
        public MapRenderer(Map m)
        {
            map = m;

            indices = GenerateIndices();

            
        }
        


        int[] GenerateIndices()
        {
            if (map != null)
            {

                List<int> indices = new List<int>();

                //012213 

                map.MapGrid.ForAllCells((i, x, y) =>
                {
                    int baseindex = i * 6;

                    indices.Add(baseindex + 0);
                    indices.Add(baseindex + 1);
                    indices.Add(baseindex + 2);
                    indices.Add(baseindex + 2);
                    indices.Add(baseindex + 1);
                    indices.Add(baseindex + 3);
                });


                return indices.ToArray();
            }
            else throw new Exception("Map is null");
        }

        void PrepareForDraw(DeviceContext context)
        { 
            //set index buffer

            context.InputAssembler.

            //set spritesheet 
            //set sprite data (tex coords of each individual texture in the sheet)
            //set per cell data 


            
            
        }

        public void Draw(DeviceContext context)
        {
            PrepareForDraw(context);


        }

    }
}
