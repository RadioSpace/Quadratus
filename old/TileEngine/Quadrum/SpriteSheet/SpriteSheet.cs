using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using SharpDX;
using SharpDX.Direct3D11;

namespace Quadrum.SpriteSheet
{

    using Path = System.IO.Path;

    /// <summary>
    /// the object that manages SpriteSheet data   
    /// </summary>
    [Serializable]
    public class SpriteSheet
    {
        DataSpriteCollection sprites;
        public string Name { get { return sprites.TextureName; } }


        string texturepath;
        public string TexturePath { get { return texturepath; } }
        

        SpriteSheet(DataSpriteCollection collection, string tpath)
        {
            sprites = collection;
            texturepath = tpath;
                       
        }

        /// <summary>
        /// Loads a sprite sheet from a .cmp file. make sure the png and cmp are in the same directory and share the same name (except for the extension of course)
        /// </summary>        
        /// <param name="cmp_path">the path to the cmp file</param>
        /// <returns>a sprite sheet loadead from memory</returns>
        /// <remarks>if loading fails the name of the SpriteSheet will be "FAILED"</remarks>
        public static SpriteSheet Load(string cmp_path)
        {//path is assumed to be varified
            //the .png file and the .cmp fille are assumed to be in the same directory and this was varfied

            DataSpriteCollection collection = null;
            

            try
            {
                collection = DataSpriteCollection.ReadCollection(cmp_path);
            }
            catch (Exception EX)
            {
                return GetFailedSpriteSheet();
            }



            return new SpriteSheet(collection, Path.GetFileNameWithoutExtension(cmp_path) + ".png");
        
        }

        private static SpriteSheet GetFailedSpriteSheet()
        {
            //this will return a standard fail sprite sheet
            return null;
        }







    }
}
