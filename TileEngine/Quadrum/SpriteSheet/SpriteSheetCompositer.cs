using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX;
using System.IO;


namespace Quadrum.SpriteSheet
{
    using Device = SharpDX.Direct3D11.Device;

    public class Compositer : IDisposable
    {


        Device d;
        GraphicsDevice gd;
        SpriteBatch sb;

        TileTarget targetA, targetB;

        public event EventHandler TargetSizeWarning;
        protected virtual void OnTargetSizeWarning() { if (TargetSizeWarning != null)TargetSizeWarning(this, EventArgs.Empty); }


        Rectangle targetrect;
        Rectangle quad;

        /// <summary>
        /// creates the compositer
        /// </summary>
        /// <param name="twidth">the width of the final texture. NOTE: keep this greater than the cell width</param>
        /// <param name="theight">the height of the  final texture. NOTE: keep this greater than the cell height</param>
        /// <param name="cwidth">the width of each cell</param>
        /// <param name="cheight">the height of each cell</param>
        /// <remarks>depending on the ratio of texture size to cell size you may not render all textures. if this happens the event TargetSizeWarning is fired </remarks>
        public Compositer(int twidth, int theight, int cwidth, int cheight)
        {
            d = new Device(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.Debug);
            gd = GraphicsDevice.New(d);
            sb = new SpriteBatch(gd);

            targetA = new TileTarget(gd, twidth, theight);
            targetB = new TileTarget(gd, twidth, theight);


            targetrect = new Rectangle(0, 0, twidth, theight);
            quad = new Rectangle(0, 0, cwidth < 1 ? 1 : cwidth, cheight < 1 ? 1 : cheight);
        }

        /// <summary>
        /// saves a compositexture
        /// </summary>
        /// <param name="width">the width of each cell</param>
        /// <param name="height">the height of each cell</param>
        public void Composite(string outputpath, params string[] paths)
        {
            try
            {
                //make grid

                int gridwidth = targetrect.Width / quad.Width;
                int gridheight = targetrect.Height / quad.Height;

                if (gridwidth * quad.Width > targetrect.Width) throw new Exception("make sure the cell width is less than the target width ");
                if (gridheight * quad.Height > targetrect.Height) throw new Exception("make sure the cell height is less than the target height ");

                int gridvolume = gridwidth * gridheight;

                if (paths.Count() > gridvolume)
                {
                    OnTargetSizeWarning();//send warning 
                }

                bool bside = false;
                //this is the dataComposited
                DataSprite[] tdata = new DataSprite[gridvolume];

                float _u = 1 / (float)gridwidth;
                float _v = 1 / (float)gridheight;


                for (int x = 0; x < gridwidth; x++)
                {
                    for (int y = 0; y < gridheight; y++)
                    {
                        int index = x + (y * gridwidth);

                        if (index < paths.Count())
                        {
                            if (File.Exists(paths[index]))
                            {
                                try
                                {
                                    FileStream cfs = File.Open(paths[index], FileMode.Open, FileAccess.Read);

                                    Texture2D celltex = Texture2D.Load(gd, cfs);
                                    //load the texture



                                    if (bside)
                                    {//Render to targetB
                                        bside = !bside;
                                        gd.ResetTargets();
                                        gd.SetRenderTargets(targetB.Target);
                                        gd.Clear(Color.Transparent);

                                        sb.Begin();

                                        sb.Draw(targetA.Target, targetrect, Color.White);
                                        sb.Draw(celltex, new RectangleF(x * quad.Width, y * quad.Height, quad.Width, quad.Height), Color.White);

                                        sb.End();
                                    }
                                    else
                                    {//render to targetA
                                        bside = !bside;
                                        gd.ResetTargets();
                                        gd.SetRenderTargets(targetA.Target);
                                        gd.Clear(Color.Transparent);

                                        sb.Begin();

                                        sb.Draw(targetB.Target, targetrect, Color.White);
                                        sb.Draw(celltex, new RectangleF(x * quad.Width, y * quad.Height, quad.Width, quad.Height), Color.White);

                                        sb.End();
                                    }

                                    cfs.Close();
                                    tdata[index] = new DataSprite(paths[index], new SVector2(_u * x, _v * y ), index);

                                    celltex.Dispose();
                                }
                                catch (Exception EX)
                                {
                                    throw new Exception("inner Composite failed!\r\n" + EX.Message, EX);
                                }

                            }
                        }
                        else tdata[index] = new DataSprite("", new SVector2(_u * x, _v * y ), index);


                    }
                }

                DataSpriteCollection tdc = new DataSpriteCollection(Path.GetFileName(outputpath), _u, _v, tdata);
                DataSpriteCollection.WriteCollection(outputpath, tdc);

                string outputtexpath = Path.GetDirectoryName(outputpath) + "\\" + Path.GetFileNameWithoutExtension(outputpath) + ".png";//png right?

                gd.ResetTargets();

                if (!bside)
                {//save targetB
                    using (FileStream fs = File.Open(outputtexpath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        targetB.Target.Save(fs, ImageFileType.Png);
                    }
                }
                else
                { //saveTargetA
                    using (FileStream fs = File.Open(outputtexpath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        targetA.Target.Save(fs, ImageFileType.Png);
                    }
                }


            }
            catch (Exception EX)
            {
                throw new Exception("Composite failed!\r\n" + EX.Message, EX);
            }






        }








        public void Dispose()
        {
            if (d != null) d.Dispose();
            if (gd != null) gd.Dispose();
            if (sb != null) sb.Dispose();
            if (targetA != null) targetA.Dispose();
            if (targetB != null) targetB.Dispose();



        }
    }

    public class CompositeException : Exception
    {

    }
}


