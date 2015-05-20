using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace TileMapMaker
{

    

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string TimeStamp { get { return DateTime.Now.ToLongDateString() + "\t" + DateTime.Now.ToLongTimeString(); } }

        public static ProjectState ProjectState =TileMapMaker.ProjectState.Unsaved;

        static App()
        {

            System.IO.Stream fs = System.IO.File.Open("TraceLog.txt", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);

            TextWriterTraceListener listener = new TextWriterTraceListener(fs);
            Trace.Listeners.Add(listener);

            Trace.WriteLine(TimeStamp + "\t" + " Session begin" + "\t" + Process.GetCurrentProcess().Id);
        }

        
    }


    
    public enum ProjectState {Unsaved,Saved,ReadOnly};

}
