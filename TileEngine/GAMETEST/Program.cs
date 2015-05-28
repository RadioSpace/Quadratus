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
            STAR.GameMap firstMap = new STAR.GameMap("dev1.cmp", 10,2, 24);

            firstMap.For(0, 0, (ref STAR.Surface s) => { s.texindex = 1; });

            firstMap.For(19, 0, (ref STAR.Surface s) => { s.texindex = 2; });


            STAR.GameMap secondMap = new STAR.GameMap("dev1.cmp", 2, 10, 24);
            secondMap.For(0, 0, (ref STAR.Surface s) => { s.texindex = 1; });

            secondMap.For(19, 0, (ref STAR.Surface s) => { s.texindex = 2; });

            STAR.GameProject project = new STAR.GameProject();
            project.AddMap("first", firstMap);
            project.AddMap("second", secondMap);



          
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new STAR.GameShell(project));

        }
    }
}
