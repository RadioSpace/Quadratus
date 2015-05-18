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
    public enum MapCommandMode {None, ChangeTexture }

    class MapEditCommands
    {
        public static RoutedCommand ChangeTexture = new RoutedCommand();

        static MapEditCommands()
        { }
    }
}
