using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;


namespace Quadrum.Map
{

    /// <summary>
    /// information about the map cells
    /// </summary>
    [Serializable]
    public struct Grid
    {
        Size2 size;
        public Size2 Size { get { return size; } }

        Size2 cellsize;
        public Size2 CellSize { get { return cellsize; } }

        int volume;
        public int Volume { get { return volume; } }


        /// <summary>
        /// creates a grid
        /// </summary>
        /// <param name="grid">the number of cells wide and high the grid will be</param>
        /// <param name="cell">the size of each cell</param>
        public Grid(Size2 grid , Size2 cell)
        {
            size = grid;
            cellsize = cell;
            volume = size.Width * size.Height;
        }

        public Rectangle GetRectangle(int x, int y)
        {
            return new Rectangle(x * cellsize.Width, y * cellsize.Height, cellsize.Width, cellsize.Height);            
        }

        public int GetIndex(int x, int y)
        {
            return x + (y * size.Width);            
        }


        /// <summary>
        /// performes the provided action on each cell
        /// </summary>
        /// <param name="action">the action to perform params are (INDEX,Xcoord,Ycoord)</param>
        public void ForAllCells(Action<int,int,int> action)
        {
            for (int u = 0; u < size.Width; u++)
            {
                for (int v = 0; v < size.Height; v++)
                {
                    action(this.GetIndex(u,v),u,v);
                }
            }                
        }

        /// <summary>
        /// performs an action on a range of contigious cells
        /// </summary>
        /// <param name="sx">the starting x coord</param>
        /// <param name="ex">the ending x coord</param>
        /// <param name="sy">the starting y coord</param>
        /// <param name="ey">the ending y coord</param>
        /// <param name="action">the action to perform params are (INDEX,Xcoord,Ycoord)</param>
        public void ForCellRange(int sx,int ex, int sy,int ey,Action<int, int, int> action)
        {

            if (!CheckWidth(sx)) { throw new ArgumentOutOfRangeException("sx", sx + " is not within grid range. " + size.ToString()); }
            if (!CheckWidth(ex)) { throw new ArgumentOutOfRangeException("ex", ex + " is not within grid range. " + size.ToString()); }
            if (!CheckHeight(sy)) { throw new ArgumentOutOfRangeException("sy", sy + " is not within grid range. " + size.ToString()); }
            if (!CheckHeight(ey)) { throw new ArgumentOutOfRangeException("ey", ey + " is not within grid range. " + size.ToString()); }
            
            for (int u = sx; u <ex; u++)
            {
                for (int v = sy; v < ey; v++)
                {
                    action(this.GetIndex(u, v), u, v);
                }
            }
        }

        /// <summary>
        /// performs an action on select cells
        /// </summary>
        /// <param name="action">the action to perform params are (INDEX,Xcoord,Ycoord)</param>
        /// <param name="cells">the cell coords to perform the action on</param>        
        public void ForSomeCell(Action<int, int, int> action, params SVector2[] cells)
        {
            foreach (SVector2 cell in cells)
            {
                action(GetIndex((int)cell.X, (int)cell.Y), (int)cell.X, (int)cell.Y);
            }
        }


        bool CheckWidth(int x)
        {
            return x > -1 && x < size.Width;
        }

        bool CheckHeight(int y)
        {
            return y > -1 && y < size.Height;
        }


    }
}
