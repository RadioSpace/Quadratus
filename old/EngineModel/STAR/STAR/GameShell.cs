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
        
        public static Random rand;
        public static object locker;

        bool Power = true;

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


        uint indexcount;
        float cellsize = 24;

        int old_X;
        int old_y;

        int ms_z;

        SharpDX.Vector3 newlook;

        bool firstcatch = true;
        bool change = false;

        TextureDataCollection tdc;

        static GameShell()
        {
            rand = new Random();
            locker = new object();
        }

        public GameShell()
        {
            InitializeComponent();
            tdc = TextureDataCollection.ReadCollection("TEST3.cmp");//test code!!! later this path will be passed in

            InitializeGraphics();

            //start drawing
            System.Threading.Tasks.Task.Factory.StartNew(render);
                        
        }


        void InitializeGraphics()
        {
            #region generate data

            Perlin r = new Perlin() { Frequency = 2, Lacunarity = 1.2, NoiseQuality = NoiseQuality.High, OctaveCount = 3, Persistence = 1, Seed = rand.Next() };
            Perlin g = new Perlin() { Frequency = .92, Lacunarity = .9, NoiseQuality = NoiseQuality.High, OctaveCount = 1, Persistence = .75, Seed = rand.Next() };
            Perlin b = new Perlin() { Frequency = 1.42, Lacunarity = 2.3, NoiseQuality = NoiseQuality.High, OctaveCount = 4, Persistence = .85, Seed = rand.Next() };

            Size grid = new System.Drawing.Size(1024, 1024 );
            uint gridvolume = (uint)(grid.Width * grid.Height);
            

            //generate the indexes to reference
            //uint vertcount = (uint)(gridvolume + grid.Width + grid.Height + 1);//vert count is always > 0 | cool but useless

            indexcount = gridvolume * 6;
            uint[] indices = new uint[indexcount];

           

            #region old code
            /*
            this was an attempt at vertex sharing 
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x <= grid.Width; x++)
                {
                    //detect front triangle   

                    if (x < grid.Width)
                    {// create front triangle
                        uint vert0 = (uint)(x + (y * (grid.Width + 1)));

                        indices.Add(vert0);//base vert
                        indices.Add(vert0 + 1);//next vert
                        indices.Add((uint)(vert0 + (grid.Width + 1)));//vert below base
                    }

                    if (x > 0)
                    {//create back triangle 
                        uint vert0 = (uint)(x + (y * (grid.Width + 1)));

                        indices.Add(vert0);//base vert
                        indices.Add((uint)(vert0 + (grid.Width + 1)));
                        indices.Add((uint)(vert0 + grid.Width));
                    }

                }
            }
     
            */
            #endregion

            for (uint x = 0, y = 0; x < indexcount; dualincrease(ref x, 6, ref y, 4))
            {
                indices[x + 0] = 0 + y;
                indices[x + 1] = 1 + y;
                indices[x + 2] = 2 + y;
                indices[x + 3] = 2 + y;
                indices[x + 4] = 1 + y;
                indices[x + 5] = 3 + y;
            }





            //generate surfacae data 
            Surface[] surfacepositions = new Surface[grid.Width * grid.Height];
                

            for (uint _u = 0; _u < grid.Width; _u++)
            {
                for (uint _v = 0; _v < grid.Height; _v++)
                {
                    uint index = (uint)(_u + (_v * grid.Width));
                    //each loop is a quad   

                    //set quad data
                    surfacepositions[index].trans = new SharpDX.Vector3(_u * cellsize, _v * cellsize, 0);
                    //surfacepositions[index].color = new SharpDX.Vector3((float)r.GetValue(_u, _v, 0), (float)g.GetValue(_u, _v, 0), (float)b.GetValue(_u, _v, 0));
                    surfacepositions[index].color = SharpDX.Vector3.One;

                    surfacepositions[index].texindex = (uint)rand.Next(0,8);
                   
                }
               
            }

            //load texturedata
            
            


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

            target = SharpDX.Direct3D11.Texture2D.FromSwapChain<SharpDX.Direct3D11.Texture2D>(sc, 0);
            targetveiw = new SharpDX.Direct3D11.RenderTargetView(d, target);



            //index buffer
            ib = SharpDX.Direct3D11.Buffer.Create(d, SharpDX.Direct3D11.BindFlags.IndexBuffer,indices);

            surfacedata = SharpDX.Direct3D11.Buffer.Create(d,surfacepositions ,new SharpDX.Direct3D11.BufferDescription()
            {
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                SizeInBytes = SharpDX.Utilities.SizeOf<Surface>(surfacepositions),
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

            surface = SharpDX.Direct3D11.Texture2D.FromFile<SharpDX.Direct3D11.Texture2D>(d, "TEST3.png");//the other side of TEST1.cmp
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
                lock (locker)
                {
                    if (change)
                    {

                        SharpDX.Matrix w = SharpDX.Matrix.Identity;
                        w.Transpose();

                        VArgs vargs = new VArgs() {world = w,cs = cellsize / 2f,glbTrans = newlook,texcoordbase = tdc.CellUnit};

                        d.ImmediateContext.UpdateSubresource( ref vargs, Arg);                        

                        change = false;
                    }
                }
                

                d.ImmediateContext.ClearRenderTargetView(targetveiw, SharpDX.Color.CornflowerBlue);

                d.ImmediateContext.DrawIndexed(System.Math.Abs((int)indexcount), 0, 0);

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
            Power = false;

            System.Threading.Thread.Sleep(500);

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

    }
}

