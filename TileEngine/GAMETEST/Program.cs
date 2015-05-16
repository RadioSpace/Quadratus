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
            STAR.GameMap project = new STAR.GameMap("C:\\Users\\daniel\\Desktop\\Eat the fail.cmp", 4, 6, 24);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new STAR.GameShell(project));

        }
    }
}
