using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using SharpDX;
using SharpDX.Direct3D11;

namespace STAR
{
    using Color = SharpDX.Color;
    using Buffer = SharpDX.Direct3D11.Buffer;

    /// <summary>
    /// the game world. max size is 1028 by 1028
    /// </summary>
    [Serializable]
    public class GameMap : SurfaceCollection ,IDisposable
    {
        /// <summary>
        /// the file path of the texture data
        /// </summary>
        string texturedatapath;

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


        #region NON-Serilized
        [NonSerialized]
        Buffer ib;

        [NonSerialized]
        Buffer surfacedata;

        [NonSerialized]
        ShaderResourceView surfacedataveiw;

        [NonSerialized]
        Buffer texturedata;

        [NonSerialized]
        ShaderResourceView texturedataveiw;

        [NonSerialized]
        Buffer P;

        [NonSerialized]
        Buffer V;

        [NonSerialized]
        Buffer Arg;

        [NonSerialized]
        Vector3 basepos;

        [NonSerialized]
        TextureDataCollection tdc;

        [NonSerialized]
        Texture2D tex;

        [NonSerialized]
        ShaderResourceView texview;
 
        #endregion


        /// <summary>
        /// creates the data need to display a map
        /// </summary>
        /// <param name="tdp">the file path of the texture data</param>
        /// <param name="GridWidth">the width in cells of the map</param>
        /// <param name="GridHeight">the height in cells of the map</param>
        /// <param name="CellSize">the size in pixels of each cell.  each cell is square</param>
        public GameMap(string tdp,int GridWidth,int GridHeight,int CellSize = 24) : base(GridWidth,GridHeight)
        {
                     

            texturedatapath = string.IsNullOrWhiteSpace(tdp) ? "null" : tdp;
            gridsize = new Size(GridWidth,GridHeight);
            cellsize = CellSize;           

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

                    surfaces[index] = newcell;
                }
            }


        }

        /// <summary>
        /// the the path of the PNG file associated with this texture coordianate data
        /// </summary>
        /// <returns>a png file path</returns>
        public string getPNGPath()
        {
            string dirname = System.IO.Path.GetDirectoryName(texturedatapath);

            return (string.IsNullOrWhiteSpace(dirname) ? "" : dirname  + "\\") + System.IO.Path.GetFileNameWithoutExtension(texturedatapath) + ".png";
        }

        public string GetCMPPath()
        {
            return texturedatapath;
        }

        #region graphics

        /// <summary>
        /// initializes the directx objects for the map
        /// </summary>
        /// <param name="ClientWidth">the windows client width</param>
        /// <param name="ClientHeight">the windows client height</param>
        /// <param name="cellUnit">a texture data collections cell unit property</param>
        public void InitializeGraphics(Device d,int ClientWidth , int ClientHeight)
        {
            #region data
            tdc = TextureDataCollection.ReadCollection(texturedatapath);   
            Vector2[] textureData = tdc.GetTexCoords();

            
            basepos = new SharpDX.Vector3(
                (-ClientWidth / 2) + (cellsize / 2),
                (-ClientHeight / 2) + (cellsize / 2),
                0);
            
            //generate the indexes to reference      
            int indexcount = gridArea * 6;
            uint[] indices = new uint[indexcount];

            for (uint x = 0, y = 0; x < indexcount; dualincrease(ref x, 6, ref y, 4))
            {
                indices[x + 0] = 0 + y;
                indices[x + 1] = 1 + y;
                indices[x + 2] = 2 + y;
                indices[x + 3] = 2 + y;
                indices[x + 4] = 1 + y;
                indices[x + 5] = 3 + y;
            }     



            Matrix p = Matrix.OrthoOffCenterLH(0, ClientWidth, ClientHeight, 0, 1, 2);
            Matrix v = Matrix.LookAtLH(new SharpDX.Vector3(0,0, -1), SharpDX.Vector3.Zero, SharpDX.Vector3.UnitY);
            Matrix w = Matrix.Identity;

            p.Transpose();
            v.Transpose();
            w.Transpose();



            VArgs va = new VArgs() 
            { 
                world = w, 
                glbTrans = basepos,
                cs = cellsize / 2f, 
                texcrdbase = tdc.CellUnit
            };
            #endregion


            #region create graphics  objects
            //index buffer
            ib = SharpDX.Direct3D11.Buffer.Create(d, SharpDX.Direct3D11.BindFlags.IndexBuffer, indices);

            surfacedata = SharpDX.Direct3D11.Buffer.Create(d, surfaces, new SharpDX.Direct3D11.BufferDescription()
            {
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                SizeInBytes = SharpDX.Utilities.SizeOf(surfaces),
                StructureByteStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Surface)),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });
            surfacedataveiw = new SharpDX.Direct3D11.ShaderResourceView(d, surfacedata);

            texturedata = SharpDX.Direct3D11.Buffer.Create(d,textureData, new SharpDX.Direct3D11.BufferDescription()
            {
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                SizeInBytes = SharpDX.Utilities.SizeOf(textureData),
                StructureByteStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(SharpDX.Vector2)),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });
            texturedataveiw = new SharpDX.Direct3D11.ShaderResourceView(d, texturedata);


            P = SharpDX.Direct3D11.Buffer.Create<SharpDX.Matrix>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new SharpDX.Matrix[] { p });
            V = SharpDX.Direct3D11.Buffer.Create<SharpDX.Matrix>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new SharpDX.Matrix[] { v });
            Arg = SharpDX.Direct3D11.Buffer.Create<VArgs>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new VArgs[] { va });

            tex = Texture2D.FromFile<Texture2D>(d,getPNGPath());
            texview = new ShaderResourceView(d, tex);

            #endregion
        }

        public void UpdateGraphics(Device d,Vector3 args)
        {
            Matrix w = Matrix.Identity;
            w.Transpose();

            VArgs varg = new VArgs() 
            {
                cs = cellsize/2,
                glbTrans =  basepos - args,
                texcrdbase = tdc.CellUnit,
                world = w//yeah, i know          
            };

            d.ImmediateContext.UpdateSubresource(ref varg, Arg);
        }

        public void PrepareGraphics(Device d )
        {
            //input assembly
            
            d.ImmediateContext.InputAssembler.SetIndexBuffer(ib, SharpDX.DXGI.Format.R32_UInt, 0);

            //vertex shader

            d.ImmediateContext.VertexShader.SetConstantBuffers(0, P, V, Arg);
            d.ImmediateContext.VertexShader.SetShaderResource(0, surfacedataveiw);
            d.ImmediateContext.VertexShader.SetShaderResource(1, texturedataveiw);


            //pixelshader
            d.ImmediateContext.PixelShader.SetShaderResource(0, texview);
        }

        public void UpdateSurfaces(Device d)
        {
            d.ImmediateContext.UpdateSubresource(surfaces, surfacedata);
        }
       

        void dualincrease(ref uint x, uint a, ref uint y, uint b)
        {
            x += a;
            y += b;
        }
        #endregion

        public void Dispose()
        {
            if (ib != null) ib.Dispose();
            if (surfacedata != null) surfacedata.Dispose();   
            if (P != null) P.Dispose();
            if (V != null) V.Dispose();
            if (surfacedataveiw != null) surfacedataveiw.Dispose();
            if (tex != null) tex.Dispose();
            if (texview != null) texview.Dispose();

        }
    }


}


