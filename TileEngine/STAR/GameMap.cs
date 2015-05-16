﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace STAR
{
    /// <summary>
    /// the game world. max size is 1028 by 1028
    /// </summary>
    [Serializable]
    public class GameMap
    {
        /// <summary>
        /// the file path of the texture data
        /// </summary>
        string texturedatapath;

        /// <summary>
        /// gets the file path of the texture data if the texture has data
        /// </summary>
        /// <remarks>returns null if texturedatapath equals "null"... texturedatapath equals "null" by default</remarks>
        public string TextureDataPath { get { return HasTextureDataPath ? texturedatapath : null; } }

        /// <summary>
        /// true if the texture has data, false if not
        /// </summary>
        bool HasTextureDataPath { get { return texturedatapath != "null"; } }

        /// <summary>
        /// the size in cells of the map grid
        /// </summary>
        Size gridsize;

        /// <summary>
        /// gets the width in cells of the map grid
        /// </summary>
        public int gridWidth { get { return gridsize.Width; } }

        /// <summary>
        /// gets the height in cells of the map grid
        /// </summary>
        public int gridHeight { get { return gridsize.Height; } }

        /// <summary>
        /// the cell size in pixels
        /// </summary>
        int cellsize;//standard is 24x24

        /// <summary>
        /// gets the cell width and height in pixels
        /// all cells are square
        /// </summary>
        public int cellSize { get { return cellsize; } }
        

        /// <summary>
        /// gets the product of the grid width and the cell width
        /// </summary>
        /// <remarks>SLOW</remarks>
        public int gridPXWidth { get { return gridsize.Width * cellsize; } }

        /// <summary>
        /// gets the product of the grid height and the cell height
        /// </summary>
        /// /// <remarks>SLOW</remarks>
        public int gridPXHeight { get { return gridsize.Height * cellsize; } }

        /// <summary>
        /// gets the product of the grid width and the grid height
        /// </summary>
        ///  <remarks>SLOW</remarks>
        public int gridArea { get { return gridsize.Width * gridsize.Height; } }

        /// <summary>
        /// gets the product of the grid pixel width and the grid pixel height
        /// </summary>
        ///  <remarks>SLOW</remarks>
        public int gridAreaPX { get { return gridPXWidth * gridPXHeight; } }

        SurfaceCollection cells;

        public Surface this[int x, int y]
        {
            get { return cells[x, y]; }
        }


        /// <summary>
        /// creates the data need to display a map
        /// </summary>
        /// <param name="tdp">the file path of the texture data</param>
        /// <param name="GridWidth">the width in cells of the map</param>
        /// <param name="GridHeight">the height in cells of the map</param>
        /// <param name="CellSize">the size in pixels of each cell.  each cell is square</param>
        public GameMap(string tdp,int GridWidth,int GridHeight,int CellSize = 24)
        {
            texturedatapath = string.IsNullOrWhiteSpace(tdp) ? "null" : tdp;
            gridsize = new Size(GridWidth,GridHeight);
            cellsize = CellSize;

            cells = new SurfaceCollection(GridWidth,GridHeight);

            for (int _u = 0; _u < gridsize.Width; _u++)
            {
                for (int _v = 0; _v < gridsize.Height; _v++)
                {                    
                    int index = _u + (_v * gridsize.Width);
                    //each loop is a quad   
                    Surface newcell = new Surface();
                    newcell.color = SharpDX.Vector3.One;
                    newcell.texindex = 0;
                    newcell.trans = new SharpDX.Vector3(_u * cellsize, _v * cellsize, 0);

                    cells[index] = newcell;
                }
            }
        }

        /// <summary>
        /// the the path of the PNG file associated with this texture coordianate data
        /// </summary>
        /// <returns>a png file path</returns>
        public string getPNGPath()
        {  
            return  System.IO.Path.GetDirectoryName(texturedatapath) + "\\" + System.IO.Path.GetFileNameWithoutExtension(texturedatapath) + ".png";
        }

        /// <summary>
        /// gets all the surfaces of the grid as an array
        /// </summary>
        /// <returns>A Surface array</returns>
        public Surface[] GetSurfaces()
        {
            return cells.ToArray();
        }

        /// <summary>
        /// performs an operation on all cells
        /// </summary>
        /// <param name="act">the act to perform</param>
        public void ForAllCells(SurfaceTransform act)
        {
            cells.ForAll(act);
        }

        /// <summary>
        /// performs an operation on all cells
        /// </summary>
        /// <param name="act">the act to perform</param>
        /// <remarks>provides location data</remarks>
        public void ForAllCells(SurfaceTransformWithCoord act)
        {
            cells.ForAll(act);
        }

        /// <summary>
        /// performs an operation on specified cells
        /// </summary>
        /// <param name="act">the act to perform</param>
        public void ForCells( int start, int length,SurfaceTransform act)
        {
            cells.For(act, start, length);
        }

        /// <summary>
        /// performs an operation on specified cells
        /// </summary>
        /// <param name="act">the act to perform</param>
        /// <remarks>provides location data</remarks>
        public void ForCells(int x, int y, int width, int height, SurfaceTransformWithCoord act)
        {
            cells.For(act, x, y, width, height);
        }

    }


}


