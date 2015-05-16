using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STAR
{
    public class GameShellMouseClickEventArgs : EventArgs
    {

        public readonly Surface surfaceFromGame;//maybe should be an array for layered surfaces?

        public readonly SharpDX.Vector2 mpos;


        Surface surfaceForGame;
        bool IsSurfaceSet = false;
        public void SetGameSurface(Surface surface)
        {
            if (!IsSurfaceSet)
            {
                surfaceForGame = surface;
                IsSurfaceSet = true;
            }
        }

        public GameShellMouseClickEventArgs(Surface surface,SharpDX.Vector2 pos)
        {
            surfaceFromGame = surface;
            mpos = pos;
        }


    }
}
