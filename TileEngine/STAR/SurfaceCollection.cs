using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;

namespace STAR
{
    /// <summary>
    /// a collections of surfaces on a grid
    /// </summary>
    public class SurfaceCollection : IEnumerable<Surface>
    {

        Surface[] surfaces;

        int width;
        /// <summary>
        /// gets the width of the grid
        /// </summary>
        public int Width { get { return width; } }

        int height;
        /// <summary>
        /// sets the width of the grid
        /// </summary>
        public int Height { get { return height; } }

        /// <summary>
        /// gets the number of cells in the grid.
        /// </summary>
        public int Length { get { return surfaces.Length; } }


        #region Surface listening

        List<int> watched; 

        #endregion

        #region events

        public event EventHandler<SurfaceEventArgs> WatchedSurfaceChanged;
        protected virtual void OnWatchedSurfaceChanged(SurfaceEventArgs e) { if (WatchedSurfaceChanged != null) { WatchedSurfaceChanged(this, e); } }

        #endregion

        /// <summary>
        /// creates a collections of surfaces on a grid
        /// </summary>
        /// <param name="w">the width in cells of the grid</param>
        /// <param name="h">the height in cells of the grid</param>
        public SurfaceCollection(int w, int h)
        { 
            surfaces = new Surface[w * h];
            watched = new List<int>();
        }

        /// <summary>
        /// performs the scpecified action on all cells
        /// </summary>
        /// <param name="a">the action to perform</param>
        public void ForAll(SurfaceTransform a)
        {
            for (int x = 0; x < surfaces.Length; x++)
            {
                Surface oldS = surfaces[x];
                a(ref surfaces[x]);
                Surface newS = surfaces[x];

                if (oldS != newS)
                {
                    if (watched.Contains(x))
                    {
                        if (width > 0)
                        {
                            OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, newS, x % width, x / width, x, surfaces.Length, width, height));
                        }
                        else
                        {//width is zero so we won't do math
                            OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, newS, 0, 0, x, surfaces.Length, width, height));
                        }
                    }
                }

                surfaces[x] = newS;                
            }
        }

        /// <summary>
        /// performs the scpecified action on all cells
        /// </summary>
        /// <param name="a">the action to perform</param>
        public void ForAll(SurfaceTransformWithCoord a)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = x + (y * width);

                    Surface oldS = surfaces[index];
                    a(ref surfaces[index], x, y);
                    Surface newS = surfaces[index];

                    if (watched.Count > 0)
                    {
                        if (oldS != newS)
                        {
                            if (watched.Contains(x))
                            {
                                if (width > 0)
                                {
                                    OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, newS, x % width, x / width, x, surfaces.Length, width, height));
                                }
                                else
                                {//width is zero so we won't do math
                                    OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, newS, 0, 0, x, surfaces.Length, width, height));
                                }
                            }
                        }
                    }

                    surfaces[x] = newS;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the cell at the specified coordinates
        /// </summary>
        /// <param name="x">the x COORD</param>
        /// <param name="y">the y COORD</param>
        /// <returns>a Surface of a cell on a grid</returns>
        public Surface this[int x,int y]
        {
            get { return surfaces[x + (y * width)]; }
            set
            {
                int index = x + (y * width);

                if(surfaces[index] != value)
                {
                    Surface oldS = surfaces[index];
                    surfaces[index] = value;
                    OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, value, x, y, index, surfaces.Length, width, height));
                }
            }
        }

        /// <summary>
        /// Gets or Sets the cell at the specified Index
        /// </summary>
        /// <param name="index">the index of a cell</param>
        /// <returns>a Surface of a cell on a grid</returns>
        public Surface this[int index]
        {
            get { return surfaces[index]; }
            set 
            {
                if (surfaces[index] != value)
                {
                    Surface oldS = surfaces[index];
                    surfaces[index] = value;
                    if (width > 0)
                    {
                        OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, value, index % width, index / width, index, surfaces.Length, width, height));
                    }

                    else
                    {//width is zero so we won't do math
                        OnWatchedSurfaceChanged(new SurfaceEventArgs(oldS, value, 0, 0, index, surfaces.Length, width, height));
                    }
                }
                    
            }
        }

        #region listening methods
        /// <summary>
        /// starts listening for an index to change
        /// </summary>
        /// <param name="i"> the index to listen to</param>
        public void StartListen(int i)
        { 
            if(!watched.Contains(i))
            {
                watched.Add(i);
            }
        }

        /// <summary>
        /// starts listening to multiple indices
        /// </summary>
        /// <param name="i">an array of indices to listen to</param>
        public void StartListen(int[] i)
        {
            watched.AddRange(i.Where(a => !watched.Contains(a)).ToArray());
        }

        /// <summary>
        /// stops listen to an index
        /// </summary>
        /// <param name="i">the index to stop listening for</param>
        public void StopListen(int i)
        {
            watched.Remove(i);
        }

        /// <summary>
        /// stops listen to the specified indices
        /// </summary>
        /// <param name="i">the indices to stop listening to</param>
        public void StopListen(int[] i)
        {
            watched.RemoveAll(a => i.Contains(a));
        }

        /// <summary>
        /// gets a copy of the watched indices
        /// </summary>
        /// <returns>a copy of indices as an integer array</returns>
        public int[] GetWatchedIndices()
        {
            return watched.ToArray();
        }

        #endregion

        #region IEnumerable<Surface>
        public IEnumerator<Surface> GetEnumerator()
        {
            return surfaces.AsEnumerable<Surface>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return surfaces.GetEnumerator();
        }
        #endregion

        #region SetValues
        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="TextureIndex">the TextureIndex to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index,uint TextureIndex)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].texindex = TextureIndex; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="Translation">the translation to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, Vector3 Translation)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].trans = Translation; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="Color">the color to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, Color Color)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].color = (Color3)Color; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="TextureIndex">the TextureIndex to set the cell to</param>
        /// <param name="Translation">the translation to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, uint TextureIndex, Vector3 Translation)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].trans = Translation; surfaces[index].texindex = TextureIndex; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="Color">the color to set the cell to</param>
        /// <param name="Translation">the translation to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, Color Color, Vector3 Translation)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].trans = Translation; surfaces[index].color = (Color3)Color; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="Color">the color to set the cell to</param>
        /// <param name="TextureIndex">the TextureIndex to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, Color Color, uint TextureIndex)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].texindex =TextureIndex; surfaces[index].color = (Color3)Color; return true; }
            else return false;
        }

        /// <summary>
        /// sets a value of the cell
        /// </summary>
        /// <param name="index">the index number of the cell</param>
        /// <param name="Color">the color to set the cell to</param>
        /// <param name="TextureIndex">the TextureIndex to set the cell to</param>
        /// <param name="Translation">the translation to set the cell to</param>
        /// <returns>true if the index is within bounds of the array</returns>
        public bool SetCellValue(int index, Color Color, uint TextureIndex, Vector3 Translation)
        {
            if (index < surfaces.Length && index > -1)
            { surfaces[index].texindex = TextureIndex; surfaces[index].color = (Color3)Color; surfaces[index].trans = Translation; return true; }
            else return false;
        }
        #endregion
    }

    /// <summary>
    /// a surface event
    /// </summary>
    public class SurfaceEventArgs : EventArgs
    {
        /// <summary>
        /// the surface before the change
        /// </summary>
        public readonly Surface before;
        /// <summary>
        /// the surface after the change
        /// </summary>
        public readonly Surface after;
        /// <summary>
        /// the x coord of the surface
        /// </summary>
        public readonly int x;
        /// <summary>
        /// the y coord of the surface
        /// </summary>
        public readonly int y;
        /// <summary>
        /// the index of the surface
        /// </summary>
        public readonly int index;
        /// <summary>
        /// the length of the grid
        /// </summary>
        public readonly int length;
        /// <summary>
        /// the width of the grid
        /// </summary>
        public readonly int width;
        /// <summary>
        /// the height of te grid
        /// </summary>
        public readonly int height;

        /// <summary>
        /// creates a surface event arg
        /// </summary>
        /// <param name="b">the surface before the change</param>
        /// <param name="a">the surface bafter the change</param>
        /// <param name="_x">the x coord of the surface</param>
        /// <param name="_y">the y coord of the surface</param>
        /// <param name="i">the index fo the surface</param>
        /// <param name="l">the length of the surface array</param>
        /// <param name="w">the width of the surface grid</param>
        /// <param name="h">the height of the surface grid</param>
        public SurfaceEventArgs(Surface b,Surface a,int _x,int _y,int i,int l,int w,int h)
        {
            before = b;
            after = a;
            x = _x;
            y = _y;
            index = i;
            length = l;
            width = w;
            height = h;
        }
    }

    public delegate void SurfaceTransform(ref Surface s);
    public delegate void SurfaceTransformWithCoord(ref Surface s,int x,int y);
}
