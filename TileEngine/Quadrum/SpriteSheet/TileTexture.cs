using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;


namespace Quadrum.SpriteSheet
{
    /// <summary>
    /// the Sprite class used to Composite this texture into the spritesheet used by the Game
    /// </summary>
    public class TileTexture
    {


        Texture2D tex;

        /// <summary>
        /// craetes the texture that ius placed in the spritesheet
        /// </summary>
        /// <param name="d">the Device</param>
        /// <param name="path">the path t o the texture</param>
        /// <remarks>this method is not responsable for validating the path</remarks>
        public TileTexture(GraphicsDevice gd, string path)
        {
            tex = Texture2D.Load(gd, path);
        }


        public void Draw(SpriteBatch sb, RectangleF quad)
        {
            sb.Draw(tex, quad, Color.White);
        }
    }

}
