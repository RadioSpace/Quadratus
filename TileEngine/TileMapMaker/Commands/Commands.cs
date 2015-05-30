using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TileMapMaker.Commands
{
    public enum MapCommandMode {None, ChangeTexture, ChangeColor, ChangePosition }

    class MapEditCommands
    {
        /// <summary>
        /// sets the map editor to ChangeTextureMode
        /// </summary>
        public static RoutedCommand ChangeTexture = new RoutedCommand();

        /// <summary>
        /// sets the map editor to change position
        /// </summary>
        public static RoutedCommand ChangePosition = new RoutedCommand();

        /// <summary>
        /// sets the map editor to ChangeSizeMode
        /// </summary>
        public static RoutedCommand ChangeColor = new RoutedCommand();

        /// <summary>
        /// Add a new map to the Project
        /// </summary>
        public static RoutedCommand Createmap = new RoutedCommand();

        /// <summary>
        /// load a map from a file
        /// </summary>
        public static RoutedCommand ImportMap = new RoutedCommand();

        /// <summary>
        /// remove a map from the project
        /// </summary>
        public static RoutedCommand RemoveMap = new RoutedCommand();
        
        /// <summary>
        /// exports a map to a file
        /// </summary>
        public static RoutedCommand ExportMap = new RoutedCommand();

        /// <summary>
        /// clear the project of all maps
        /// </summary>
        public static RoutedCommand ClearProject = new RoutedCommand();

        /// <summary>
        /// Change the selected Map 
        /// </summary>
        public static RoutedCommand SelectMap = new RoutedCommand();
 




        static MapEditCommands()
        { }
    }
}
