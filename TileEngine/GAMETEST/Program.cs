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
            STAR.GameMap project = new STAR.GameMap("dev1.cmp", 10,2, 24);

            project.For(0, 0, (ref STAR.Surface s) => { s.texindex = 1; });

            project.For(19, 0, (ref STAR.Surface s) => { s.texindex = 2; });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new STAR.GameShell(project));

        }
    }
}
