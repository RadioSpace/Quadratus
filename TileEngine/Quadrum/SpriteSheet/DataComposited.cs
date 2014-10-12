using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using SharpDX;


namespace Quadrum.SpriteSheet
{

    [Serializable]
    public class DataSpriteCollection : IList<DataSprite>
    {
        List<DataSprite> textures;

        SVector2 cellunit;
        public SVector2 CellUnit { get { return cellunit; } }

        string texturename;
        public string TextureName { get { return texturename; } }

        public DataSpriteCollection()
        {
            textures = new List<DataSprite>();
            cellunit = new SVector2(0, 0);
            texturename = "";
        }

        public DataSpriteCollection(string tname, float u, float v, params DataSprite[] data)
        {
            texturename = tname;
            cellunit = new SVector2(u, v);
            textures = new List<DataSprite>(data);
        }

        public SVector2[] GetTexCoords()
        {
            return this.Select(a => a.TexCoord).ToArray();
        }


        public static DataSpriteCollection ReadCollection(string path)
        {
            DataSpriteCollection tdc = new DataSpriteCollection();

            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        tdc = (DataSpriteCollection)bf.Deserialize(fs);
                    }
                }
                catch (Exception EX) { throw new Exception("Could not deserilize " + path + " Because " + EX.Message, EX); }
            }
            else throw new FileNotFoundException(path + " does not exsist");

            return tdc;
        }

        public static void WriteCollection(string path, DataSpriteCollection dsc)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, dsc);
                }
            }
            catch (Exception EX)
            {
                throw new Exception("writing the file failed because " + EX.Message, EX);
            }
        }



        public int IndexOf(DataSprite item)
        {
            return textures.IndexOf(item);
        }

        public void Insert(int index, DataSprite item)
        {
            textures.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            textures.RemoveAt(index);
        }

        public DataSprite this[int index]
        {
            get
            {
                return textures[index];
            }
            set
            {
                textures[index] = value;
            }
        }

        public void Add(DataSprite item)
        {
            textures.Add(item);
        }

        public void Clear()
        {
            textures.Clear();
        }

        public bool Contains(DataSprite item)
        {
            return textures.Contains(item);
        }

        public void CopyTo(DataSprite[] array, int arrayIndex)
        {
            textures.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return textures.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(DataSprite item)
        {
            return textures.Remove(item);
        }

        public IEnumerator<DataSprite> GetEnumerator()
        {
            return textures.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return textures.GetEnumerator();
        }
    }


    /// <summary>
    /// data used by the game graphics to pickout the intended texture from the sprite sheet or better known as the UV coordinates
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct DataSprite : IEquatable<DataSprite>
    {
        string path;
        public string Path
        {
            get { return path; }
        }

        SVector2 texcoord;
        public SVector2 TexCoord { get { return texcoord; } }

        int index;
        public int Index { get { return index; } }

        public DataSprite(string p, SVector2 texC,int i)
        {
            path = p;
            texcoord = texC;
            index = i;
        }

        public bool Equals(DataSprite other)
        {
            return this.path == other.Path && this.texcoord == other.TexCoord && this.index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (obj is DataSprite) return this.Equals((DataSprite)obj);
            else return false;            
        }

        public override int GetHashCode()
        {
            int hash = 11;
            hash = hash * 89 + path.GetHashCode();
            hash = hash * 89 + texcoord.GetHashCode();
            return hash * 89 + index.GetHashCode();
        }

        public override string ToString()
        {
            return "path: " + path + "|texcoord: " + texcoord.ToString() + "|index: " + index;
        }    
    
        public static bool operator ==(DataSprite dsA,DataSprite dsB)
        {
            return dsA.Equals(dsB);        
        }

        public static bool operator !=(DataSprite dsA, DataSprite dsB)
        {
            return !dsA.Equals(dsB);
        }
    }
}
