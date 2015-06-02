using System;
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

        bool power;
        bool Power { get { lock (locker) { return power; } } set { lock (locker) { power = value; } } }

        bool paused = false;
        bool Paused { get { lock (locker) { return paused; } } set { lock (locker) { paused = value; } } }

        GameProject project;
        string currentmap;
        string CurrentMap { get { lock (locker) { return currentmap; } } set { lock (locker) { currentmap = value; } } } 
        string[] mapnames;

        Task RenderingTask;
        CancellationTokenSource RenderingCancel;

        int old_X;
        int old_y;

        int ms_z;

        SharpDX.Vector3 newlook;
        SharpDX.Vector3 NewLook { get { lock (locker) { return newlook; } } set { lock (locker) { newlook = value; } } }
        

        /// <summary>
        /// the cell coords of the last cell the mouse was over
        /// </summary>
        SharpDX.Vector2 lastcellover;

        bool firstcatch = true;

        bool surfacechange = false;
        bool SurfaceChange { get { lock (locker) { return surfacechange; } } set { lock (locker) { surfacechange = value; } } }

        bool editgamemode = false;
        bool EditGameMode { get { lock (locker) { return editgamemode; } } set { lock (locker) { editgamemode = value; } } }

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
                    if (EditGameMode)
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
                    if (EditGameMode)
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
            CurrentMap = mapnames[0];
            EditGameMode = editmode;

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

                
                while (Paused)
                {
                    Thread.Sleep(100);
                }
                




                lastcellover = new SharpDX.Vector2();
          
           
                if (SurfaceChange && currentmap != null)
                {
                    project[CurrentMap].UpdateSurfaces(d);
                    SurfaceChange = false;
                }
                

            
                //clear the screen
                d.ImmediateContext.ClearRenderTargetView(targetveiw, SharpDX.Color.CornflowerBlue);

                foreach (GameMap pmap in project)
                {                    
                    pmap.PrepareGraphics(d);

                    if (EditGameMode)
                    {

                        pmap.UpdateGraphics(d);

                    }


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
            if (EditGameMode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (firstcatch)
                    {

                        old_X = (int)NewLook.X + e.X;
                        old_y = (int)NewLook.Y + e.Y;
                        firstcatch = false;

                    }
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (EditGameMode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (!firstcatch)
                    {//this is the else for the onmousedown method
                        lock (GameShell.locker)
                        {
                            NewLook = new SharpDX.Vector3(old_X - e.X, old_y - e.Y, ms_z);

                            SurfaceChange = true;

                        }
                    }
                }
            }



            
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (EditGameMode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    firstcatch = true;

                    old_X = (int)NewLook.X;
                    old_y = (int)NewLook.Y;
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (EditGameMode)
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

            if (EditGameMode)
            {

                GameShellMouseEventArgs args = GenerateGameShellMouseEventArgs(e);

                if (args != null)
                {
                    OnGameClick(args);

                    if (args.IsSurfaceSet && currentmap != null)
                    {
                        project[CurrentMap].For((int)args.cellpos.X, (int)args.cellpos.Y, 1, 1, (ref Surface sur, int u, int v) => { sur = (Surface)(args.SurfaceForGame ?? sur); });
                        SurfaceChange = true;
                    }
                }
            }

        }

        private void GameShell_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditGameMode)
            {
                GameShellMouseEventArgs args = GenerateGameShellMouseEventArgs(e);

                if (args != null)
                {
                    if (lastcellover != args.cellpos)
                    {
                        OnGameMouseMove(args);
                        lastcellover = args.cellpos;

                        if (args.IsSurfaceSet && currentmap != null)
                        {
                            project[CurrentMap].For((int)args.cellpos.X, (int)args.cellpos.Y, 1, 1, (ref Surface sur, int u, int v) => { sur = (Surface)(args.SurfaceForGame ?? sur); });
                            SurfaceChange = true;
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
            if (EditGameMode && currentmap != null)
            {
                MemoryStream ms = new MemoryStream();

                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, project[CurrentMap]);

                ms.Close();
                
                return ms.ToArray();
            }
            else
            {
                throw new Exception("Cannot save. not in edit mode");
            }

        }

        public byte[] SaveProject()
        {
            MemoryStream ms = new MemoryStream();

            lock (locker)
            {
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(ms, project);
            }

            ms.Close();

            return ms.ToArray();
        }

        public void RemoveSelectedMap()
        {
            lock (locker)
            {
                project.RemoveMap(currentmap);
            }

            currentmap = "";
        }

        /// <summary>
        /// Uploads a map into the game in edit mode
        /// </summary>
        /// <param name="m">the map to load into the game</param>       
        public bool UploadMap(string name,GameMap m)
        {
            bool result = false;

            if(EditGameMode && Paused)
            {
                m.InitializeGraphics(d,ClientSize.Width,ClientSize.Height);

                project.AddMap(name, m);
                mapnames = project.GetKeys();               

                lastcellover = new SharpDX.Vector2();
                
                

                result = true;

                
            }



            return result;
        }

        public void ClearProject()
        {
            if (EditGameMode && Paused)
            {
                project.Clear();
                currentmap = null;
            }
        }

        public GameMap SelectMap(string name)
        {
            if (mapnames.Contains(name))
            {
                CurrentMap = name;
                lock (locker) { return project[name]; }
            }
            else {return null; }

        }
        public GameProject GetGameProject()
        {
            lock (locker) { return project; }
        }

        public void SetGameProject(GameProject p)
        { 
            lock(locker)
            {
                project.Dispose();

                project = p;
            }
        }

        public void PauseGameToggle()
        {
            Paused = !Paused;
        }
        public string[] GetMapNames()
        {
            return project.GetKeys();
        }

        GameShellMouseEventArgs GenerateGameShellMouseEventArgs(MouseEventArgs e)
        {
            //it is time to straighten out my grid
            Point p = e.Location;

            //find the cell at that position
            if (currentmap != null)
            {

                int x = (int)(p.X + NewLook.X) / project[CurrentMap].cellSize;
                int y = (int)(p.Y + NewLook.Y) / project[CurrentMap].cellSize;



                if (x > -1 && y > -1 && x < project[CurrentMap].gridWidth && y < project[CurrentMap].gridHeight)
                {
                    return new GameShellMouseEventArgs(project[CurrentMap][x, y], new SharpDX.Vector2(x, y), e);
                }
                else return null;
            }
            else return null;

        }

    }
}

