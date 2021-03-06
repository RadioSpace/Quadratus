﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Threading;
using System.Threading.Tasks;

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



        SharpDX.Direct3D11.Texture2D target;
        SharpDX.Direct3D11.RenderTargetView targetveiw;       


        SharpDX.Direct3D11.SamplerState sampler;

        #endregion

        #region util

        bool Power = true;

        
        float cellsize = 24;


        GameProject project;
        string currentmap;
        string[] mapnames;

        Task RenderingTask;
        CancellationTokenSource RenderingCancel;

        int old_X;
        int old_y;

        int ms_z;

        SharpDX.Vector3 newlook;
        

        /// <summary>
        /// the cell coords of the last cell the mouse was over
        /// </summary>
        SharpDX.Vector2 lastcellover;

        bool firstcatch = true;
        bool surfaceChange = false;

        bool editgamemode = false;

        #endregion


        #region events

        EventHandler<GameShellMouseEventArgs> gameclick;
        /// <summary>
        /// used when the gameshell is in editmode
        /// </summary>
        public event EventHandler<GameShellMouseEventArgs> GameClick
        {
            add 
            {
                lock (locker)
                {
                    if (editgamemode)
                    {
                        gameclick += value;
                    }
                }
            }
            remove
            {
                lock (locker)
                {
                    if (gameclick != null)
                    {
                        if (gameclick.GetInvocationList().Contains(value))
                        {
                            gameclick -= value;
                        }
                    }
                }
            }
        }

        protected virtual void OnGameClick(GameShellMouseEventArgs e) { if (gameclick != null)gameclick(this, e); }


        EventHandler<GameShellMouseEventArgs> gamemousemove;
        /// <summary>
        /// used when the game shell is in edit mode
        /// </summary>
        public event EventHandler<GameShellMouseEventArgs> GameMouseMove 
        {
            add
            {
                lock (locker)
                {
                    if (editgamemode)
                    {
                        gamemousemove += value;
                    }
                }
            }
            remove
            {
                lock (locker)
                {
                    if (gamemousemove != null)
                    {
                        if (gamemousemove.GetInvocationList().Contains(value))
                        {
                            gamemousemove -= value;
                        }
                    }
                }
            }
        }
        protected virtual void OnGameMouseMove(GameShellMouseEventArgs e) { if (gamemousemove != null)gamemousemove(this, e); }

        #endregion


        public GameShell(GameProject p,bool editmode = false)
        {
            InitializeComponent();

            InitializeGraphics();

            project = p;

         
            mapnames = project.GetKeys();
            currentmap = mapnames[0];
            editgamemode = editmode;

            lastcellover = new SharpDX.Vector2();
            
            foreach (string name in mapnames)
            {
                project[name].InitializeGraphics(d, ClientSize.Width, ClientSize.Height);
                
            }

            RenderingCancel = new CancellationTokenSource();
            RenderingTask = Task.Factory.StartNew(render, RenderingCancel.Token);            

        }




        void InitializeGraphics()
        {

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


                 
            d.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            d.ImmediateContext.VertexShader.Set(new SharpDX.Direct3D11.VertexShader(d, File.ReadAllBytes("VertexShader.cso")));
            d.ImmediateContext.PixelShader.Set(new SharpDX.Direct3D11.PixelShader(d, File.ReadAllBytes("PixelShader.cso")));
                        
            d.ImmediateContext.Rasterizer.SetViewport(0, 0,ClientSize.Width,ClientSize.Height);
            d.ImmediateContext.Rasterizer.State = new SharpDX.Direct3D11.RasterizerState(d, new SharpDX.Direct3D11.RasterizerStateDescription()
            {
                CullMode = SharpDX.Direct3D11.CullMode.None,
                FillMode = SharpDX.Direct3D11.FillMode.Solid               
            });           
            
            d.ImmediateContext.OutputMerger.SetRenderTargets(targetveiw);


            
        }

        void render()
        {
            
            //todo: implement the cancellation token instead of the Power variable

            Power = true;

            while (Power)
            {
                //now we have the Gamemap object and it is ready to dynamically edit the surface data

                #region junk
           
                if (surfaceChange)
                {
                    project[currentmap].UpdateSurfaces(d);
                    surfaceChange = false;
                }
                


                
                
                #endregion

                //clear the screen
                d.ImmediateContext.ClearRenderTargetView(targetveiw, SharpDX.Color.CornflowerBlue);

                foreach (GameMap pmap in project)
                {
                    if (editgamemode)
                    {

                        pmap.UpdateGraphics(d, newlook);

                    }
                    
                    pmap.PrepareGraphics(d);

                    d.ImmediateContext.DrawIndexed(Math.Abs(pmap.gridArea * 6), 0, 0);
                }

                //draw all tiles
               

                //render to the screen
                sc.Present(0, SharpDX.DXGI.PresentFlags.None);                
            }
        }

        void dualincrease(ref uint x, uint a, ref uint y, uint b)
        {
            x += a;
            y += b;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Power = false;//power is used by another thread

            RenderingTask.Wait();

            //shut'er down
            if ( d != null) d.Dispose();
            if ( sc != null) sc.Dispose();
         
            if ( target != null) target.Dispose();
            if ( targetveiw != null) targetveiw.Dispose();
  
            if ( sampler != null) sampler.Dispose();

            if (project != null) project.Dispose();

            base.OnClosing(e);
        }

        #region onMouse

        //these are for scrolling and were initially for testing 

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (editgamemode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (firstcatch)
                    {

                        old_X = (int)newlook.X + e.X;
                        old_y = (int)newlook.Y + e.Y;
                        firstcatch = false;

                    }
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (editgamemode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (!firstcatch)
                    {//this is the else for the onmousedown method
                        lock (GameShell.locker)
                        {
                            newlook = new SharpDX.Vector3(old_X - e.X, old_y - e.Y, ms_z);

                            surfaceChange = true;

                        }
                    }
                }
            }



            
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (editgamemode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    firstcatch = true;

                    old_X = (int)newlook.X;
                    old_y = (int)newlook.Y;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (editgamemode)
            {
                ms_z = e.Delta;
            }
            base.OnMouseWheel(e);
        }
        #endregion

        private void GameShell_Load(object sender, EventArgs e)
        {

        }

        private void GameShell_MouseDown(object sender, MouseEventArgs e)
        {

            if (editgamemode)
            {

                GameShellMouseEventArgs args = GenerateGameShellMouseEventArgs(e);

                if (args != null)
                {
                    OnGameClick(args);

                    if (args.IsSurfaceSet)
                    {
                        project[currentmap].For((int)args.cellpos.X, (int)args.cellpos.Y, 1, 1, (ref Surface sur, int u, int v) => { sur = (Surface)(args.SurfaceForgame ?? sur); });
                        surfaceChange = true;
                    }
                }
            }

        }

        private void GameShell_MouseMove(object sender, MouseEventArgs e)
        {
            if (editgamemode)
            {
                GameShellMouseEventArgs args = GenerateGameShellMouseEventArgs(e);

                if (args != null)
                {
                    if (lastcellover != args.cellpos)
                    {
                        OnGameMouseMove(args);
                        lastcellover = args.cellpos;

                        if (args.IsSurfaceSet)
                        {
                            project[currentmap].For((int)args.cellpos.X, (int)args.cellpos.Y, 1, 1, (ref Surface sur, int u, int v) => { sur = (Surface)(args.SurfaceForgame ?? sur); });
                            surfaceChange = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// saves the game map to a stream via binary serilization
        /// </summary>
        /// <returns>a stream containg the game map</returns>
        public byte[] SaveMap()
        {
            if (editgamemode)
            {
                MemoryStream ms = new MemoryStream();

                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, project[currentmap]);

                ms.Close();
                
                return ms.ToArray();
            }
            else
            {
                throw new Exception("Cannot save. not in edit mode");
            }

        }

        /// <summary>
        /// laods a map into the game
        /// </summary>
        /// <param name="m">the map to load into the game</param>
        public void LoadMap(GameMap m)
        {
            if (editgamemode)
            {
                Power = false;

                RenderingTask.Wait();

                d.ImmediateContext.ClearState();

                InitializeGraphics();

                RenderingTask = new Task(render, RenderingCancel.Token);
                RenderingTask.Start();
            }
        }

        public void SelectMap(string name)
        {
            if (mapnames.Contains(name))
            {
                currentmap = name;
            }
        }

        GameShellMouseEventArgs GenerateGameShellMouseEventArgs(MouseEventArgs e)
        {
            //it is time to straighten out my grid
            Point p = e.Location;

            //find the cell at that position


            int x = (int)(p.X + newlook.X) / project[currentmap].cellSize;
            int y = (int)(p.Y + newlook.Y) / project[currentmap].cellSize;



            if (x > -1 && y > -1 && x < project[currentmap].gridWidth && y < project[currentmap].gridHeight)
            {
                return new GameShellMouseEventArgs(project[currentmap][x, y], new SharpDX.Vector2(x, y), e);
            }
            else return null;           

        }

    }
}

