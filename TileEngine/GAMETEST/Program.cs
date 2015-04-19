using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GAMETEST
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            MapCreate.MapProject project = new MapCreate.MapProject("C:\\Users\\daniel\\Desktop\\Eat the fail.cmp", 2, 2, 24);



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new STAR.GameShell(project.TextureDataPath,project.GetSurfaces(),project.gridWidth,project.gridHeight,project.cellSize,project.getPNGPath()));
        }
    }
}
