using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;

namespace STAR
{
    public class SurfaceCollection
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

        public SurfaceCollection(int w, int h)
        { 
            surfaces = new Surface[w * h];
        }

        public void ForAll(Func<Surface,Surface> a)
        {
            for (int x = 0; x < surfaces.Length; x++)
            {
                Surface oldS = surfaces[x];
                Surface newS = a(surfaces[x]);

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

        public void ForAll(Func<Surface, int, int, Surface> a)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = x + (y * width);

                    Surface oldS = surfaces[index];
                    Surface newS = a(surfaces[index], x, y);

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
}
