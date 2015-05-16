using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STAR
{
    /// <summary>
    /// 
    /// </summary>
    public class GameShellMouseClickEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly Surface surfaceFromGame;//maybe should be an array for layered surfaces?
        /// <summary>
        /// 
        /// </summary>
        public readonly SharpDX.Vector2 cellpos;
        /// <summary>
        /// 
        /// </summary>
        public readonly System.Windows.Forms.MouseEventArgs MouseArgs;


        Surface surfaceForGame;
        internal Surface SurfaceForgame { get { return surfaceForGame; } }

        bool isSurfaceSet = false;
        public bool IsSurfaceSet { get { return isSurfaceSet; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="surface"></param>
        public void SetGameSurface(Surface surface)
        {
            if (!isSurfaceSet)
            {
                surfaceForGame = surface;
                isSurfaceSet = true;
            }
        }

        

        /// <summary>
        /// creates a new Mouse CLick Args for the game sshell
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="pos"></param>
        /// <param name="cpos"></param>
        /// <param name="ma"></param>
        public GameShellMouseClickEventArgs(Surface surface,SharpDX.Vector2 cpos, System.Windows.Forms.MouseEventArgs ma)
        {
            surfaceFromGame = surface;
          
            cellpos = cpos;
            MouseArgs = ma;
        }


    }
}
