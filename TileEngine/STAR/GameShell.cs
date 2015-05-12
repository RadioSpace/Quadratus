using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LibNoise;

namespace STAR
{
    public partial class GameShell : Form
    {
        #region static 
        public static Random rand;
        public static object locker;
        static GameShell()
        {
            rand = new Random();
            locker = new object();
        }

        #endregion

        #region sharpDX objects

        SharpDX.Direct3D11.Device d;
        SharpDX.DXGI.SwapChain sc;

        SharpDX.Direct3D11.Buffer ib;

        SharpDX.Direct3D11.Buffer surfacedata;
        SharpDX.Direct3D11.ShaderResourceView surfacedataveiw;

        SharpDX.Direct3D11.Buffer texturedata;
        SharpDX.Direct3D11.ShaderResourceView texturedataveiw;


        SharpDX.Direct3D11.Buffer P;
        SharpDX.Direct3D11.Buffer V;
        SharpDX.Direct3D11.Buffer Arg;

        SharpDX.Direct3D11.Texture2D target;
        SharpDX.Direct3D11.RenderTargetView targetveiw;

        
        SharpDX.Direct3D11.Texture2D surface;
        SharpDX.Direct3D11.ShaderResourceView surfaceveiw;

        SharpDX.Direct3D11.SamplerState sampler;

        #endregion

        #region util

        bool Power = true;

        uint indexcount;
        float cellsize = 24;

        TextureDataCollection tdc;
        SurfaceCollection surfaces;


        int old_X;
        int old_y;

        int ms_z;

        SharpDX.Vector3 newlook;

        bool firstcatch = true;
        bool change = false;

        bool cellupdate = false;



        #endregion

        
        public GameShell(string cmpPath,Surface[] surfaceData,int cellsWide,int cellsHigh,int cellSize,string texturepath)
        {
            InitializeComponent();
            tdc = TextureDataCollection.ReadCollection(cmpPath);//test code!!! later this path will be passed in
            surfaces = new SurfaceCollection(cellsWide, cellsHigh);


            InitializeGraphics( cellsWide, cellsHigh,texturepath);

            //start drawing
            System.Threading.Tasks.Task.Factory.StartNew(render);

            
                        
        }


        void InitializeGraphics(int width,int height,string spritesheetpath)
        {
            #region generate data

            
            
            //the grid
            Size grid = new System.Drawing.Size(width, height );
            uint gridvolume = (uint)(grid.Width * grid.Height);
            
            //generate the indexes to reference      
            indexcount = gridvolume * 6;
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

            SharpDX.Matrix p = SharpDX.Matrix.PerspectiveLH(this.ClientSize.Width,this.ClientSize.Height, 1, 201);
            SharpDX.Matrix v = SharpDX.Matrix.LookAtLH(new SharpDX.Vector3(0, 0, 100), SharpDX.Vector3.Zero, SharpDX.Vector3.UnitY);
            SharpDX.Matrix w = SharpDX.Matrix.Identity;

            p.Transpose();
            v.Transpose();
            w.Transpose();

            VArgs va = new VArgs(){world = w, glbTrans = SharpDX.Vector3.Zero,cs = cellsize / 2f, texcoordbase = tdc.CellUnit };

            #endregion

            #region create graphics objects

            //create directx objects

            //device and swap chain
            SharpDX.Direct3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, SharpDX.Direct3D11.DeviceCreationFlags.Debug, new SharpDX.DXGI.SwapChainDescription()
            {
                BufferCount = 4,
                Flags = SharpDX.DXGI.SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new SharpDX.DXGI.ModeDescription(this.ClientSize.Width, this.ClientSize.Height, new SharpDX.DXGI.Rational(60, 1), SharpDX.DXGI.Format.R8G8B8A8_UNorm),
                OutputHandle = this.Handle,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                SwapEffect = SharpDX.DXGI.SwapEffect.Discard,
                Usage = SharpDX.DXGI.Usage.RenderTargetOutput
            }, out d, out sc);

            //the render target
            target = SharpDX.Direct3D11.Texture2D.FromSwapChain<SharpDX.Direct3D11.Texture2D>(sc, 0);
            targetveiw = new SharpDX.Direct3D11.RenderTargetView(d, target);



            //index buffer
            ib = SharpDX.Direct3D11.Buffer.Create(d, SharpDX.Direct3D11.BindFlags.IndexBuffer,indices);

            surfacedata = SharpDX.Direct3D11.Buffer.Create(d,surfaces.ToArray(),new SharpDX.Direct3D11.BufferDescription()
            {
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                SizeInBytes = surfaces.SizeOf,
                StructureByteStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Surface)),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });
            surfacedataveiw = new SharpDX.Direct3D11.ShaderResourceView(d, surfacedata);

            texturedata = SharpDX.Direct3D11.Buffer.Create(d, tdc.GetTexCoords(), new SharpDX.Direct3D11.BufferDescription() 
            {
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                SizeInBytes = SharpDX.Utilities.SizeOf(tdc.GetTexCoords()),
                StructureByteStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(SharpDX.Vector2)),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default
            });
            texturedataveiw = new SharpDX.Direct3D11.ShaderResourceView(d, texturedata);


            P = SharpDX.Direct3D11.Buffer.Create<SharpDX.Matrix>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new SharpDX.Matrix[] { p });
            V = SharpDX.Direct3D11.Buffer.Create<SharpDX.Matrix>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new SharpDX.Matrix[] { v });
            Arg = SharpDX.Direct3D11.Buffer.Create<VArgs>(d, SharpDX.Direct3D11.BindFlags.ConstantBuffer, new VArgs[] { va });

            surface = SharpDX.Direct3D11.Texture2D.FromFile<SharpDX.Direct3D11.Texture2D>(d, spritesheetpath);//the other side of TEST1.cmp
            surfaceveiw = new SharpDX.Direct3D11.ShaderResourceView(d, surface);

            sampler = new SharpDX.Direct3D11.SamplerState(d, new SharpDX.Direct3D11.SamplerStateDescription()
            {
                AddressU = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressV = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                AddressW = SharpDX.Direct3D11.TextureAddressMode.Wrap,
                BorderColor = SharpDX.Color.Transparent,
                ComparisonFunction = SharpDX.Direct3D11.Comparison.Never,
                Filter = SharpDX.Direct3D11.Filter.MinMagMipLinear,
                MaximumAnisotropy = 4,     
                MaximumLod = 1,
                MinimumLod = 0,
                MipLodBias = 0
            });

            #endregion

            #region set objects on the immediate context
            //set the objects to the immeidate context


            //input assembly
            d.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            d.ImmediateContext.InputAssembler.SetIndexBuffer(ib, SharpDX.DXGI.Format.R32_UInt, 0);

            //vertex shader
            d.ImmediateContext.VertexShader.Set(new SharpDX.Direct3D11.VertexShader(d, CSOLoader.Load("VertexShader.cso")));
            d.ImmediateContext.VertexShader.SetConstantBuffers(0, P, V, Arg);
            d.ImmediateContext.VertexShader.SetShaderResource(0, surfacedataveiw);
            d.ImmediateContext.VertexShader.SetShaderResource(1, texturedataveiw);

            //hull shader

            //domain shader

            //pixelshader
            d.ImmediateContext.PixelShader.Set(new SharpDX.Direct3D11.PixelShader(d, CSOLoader.Load("PixelShader.cso")));
            d.ImmediateContext.PixelShader.SetSampler(0, sampler);
            d.ImmediateContext.PixelShader.SetShaderResource(0, surfaceveiw);

            //rasterizer
            d.ImmediateContext.Rasterizer.SetViewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            d.ImmediateContext.Rasterizer.State = new SharpDX.Direct3D11.RasterizerState(d, new SharpDX.Direct3D11.RasterizerStateDescription()
            {
                CullMode = SharpDX.Direct3D11.CullMode.None,
                FillMode = SharpDX.Direct3D11.FillMode.Solid
            });

            //output
            d.ImmediateContext.OutputMerger.SetRenderTargets(targetveiw);

            #endregion

            
        }

        void render()
        {

            Power = true;

            while (Power)
            {
                ///////////////////////////////////////////////////////////////////////////////
                //this is temporary till i figure out how i want the data to be updated/////////
                ///////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////
                lock (locker)
                {
                    if (change)
                    {

                        SharpDX.Matrix w = SharpDX.Matrix.Identity;
                        w.Transpose();

                        VArgs vargs = new VArgs() {world = w,cs = cellsize / 2f,glbTrans = newlook,texcoordbase = tdc.CellUnit};

                        d.ImmediateContext.UpdateSubresource( ref vargs, Arg);

                        
                        SurfaceData[0].texindex = (uint)(SurfaceData[0].texindex == 0 ? 1 : 0);

                        d.ImmediateContext.UpdateSubresource(SurfaceData, surfacedata);
                        

                        change = false;
                    }


                }
                ///////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////////////
                


                //clear the screen
                d.ImmediateContext.ClearRenderTargetView(targetveiw, SharpDX.Color.CornflowerBlue);
                
                //draw all tiles
                d.ImmediateContext.DrawIndexed(System.Math.Abs((int)indexcount), 0, 0);

                //render to the screen
                sc.Present(0, SharpDX.DXGI.PresentFlags.None);                
            }
        }

        void dualincrease(ref uint x, uint a,ref uint y, uint b)
        {
            x += a;
            y += b;            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Power = false;//power is used by another thread

            System.Threading.Thread.Sleep(500);//give the other thread time to react

            //shut'er down
            if ( d != null) d.Dispose();
            if ( sc != null) sc.Dispose();
            if ( ib != null) ib.Dispose();
            if ( surfacedata != null) surfacedata.Dispose();            
            if ( target != null) target.Dispose();
            if ( targetveiw != null) targetveiw.Dispose();
            if ( P != null) P.Dispose();
            if ( V != null) V.Dispose();
            if ( surfacedataveiw != null) surfacedataveiw.Dispose();
            if ( sampler != null) sampler.Dispose();
            if ( Arg != null) Arg.Dispose();
            if ( texturedata != null) texturedata.Dispose();
            if ( texturedataveiw != null) texturedataveiw.Dispose();

            base.OnClosing(e);
        }



        #region onMouse
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (firstcatch)
            {

                old_X =  (int)newlook.X + e.X;
                old_y =  (int)newlook.Y + e.Y;
                firstcatch = false;

            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!firstcatch)
            {//this is the else for the onmousedown method
                lock (GameShell.locker)
                {
                    newlook = new SharpDX.Vector3(old_X - e.X , old_y - e.Y , ms_z);

                    change = true;
          
                }  
            }
            
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            firstcatch = true;

            old_X = (int)newlook.X;
            old_y = (int)newlook.Y;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {                       
            ms_z = e.Delta;
            base.OnMouseWheel(e);
        }
        #endregion

        private void GameShell_Load(object sender, EventArgs e)
        {

        }


        public void updatesurface(Surface[] surfaces)
        {
            
        }

    }
}

