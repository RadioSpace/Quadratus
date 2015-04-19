using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System.Linq;


namespace Quadrum.SpriteSheet
{


    /// <summary>
    /// the render target used during the compositing process
    /// </summary>    
    public class TileTarget : IDisposable
    {
        public int Width { get { return target.Width; } }
        public int Height { get { return target.Height; } }



        RenderTarget2D target;
        public RenderTarget2D Target { get { return target; } }

        RectangleF quad;


        public TileTarget(GraphicsDevice gd, int width, int height)
        {
            target = RenderTarget2D.New(gd, width, height, 1, PixelFormat.R8G8B8A8.UNorm);
            quad = new RectangleF(0,0,width,height);
        }

        public void Save(string path)
        {
            target.Save(path, ImageFileType.Png);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(target, quad, Color.White);
        }

        public void Set(GraphicsDevice gd)
        {
            gd.ResetTargets();

            gd.SetRenderTargets(target);
        }

        

        public void Dispose()
        {
            target.Dispose();
        }
    }
}
