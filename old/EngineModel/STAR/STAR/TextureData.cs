using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.IO;
using SharpDX;

namespace STAR
{
    /// <summary>
    /// a collection of textures to composite
    /// </summary>
    [Serializable]
    public class TextureDataCollection : IList<TextureData>
    {
        List<TextureData> textures;

        TCoord cellunit;
        public TCoord CellUnit { get { return cellunit; } }


        string texturename;
        public string TextureName { get { return texturename; } }


        public TextureDataCollection()
        {
            textures = new List<TextureData>();
            cellunit = new TCoord() { u = 0, v = 0 };
            texturename = "";
        }
        public TextureDataCollection(string texname, float _u,float _v,params TextureData[] data)
        {
            textures = new List<TextureData>();

            textures.AddRange(data);

            texturename = texname;
            cellunit = new TCoord() { u = _u, v = _v };
        }


        public Vector2[] GetTexCoords()
        {
            return this.Select(a => (Vector2)a.Texcoord).ToArray();
        }

        public static TextureDataCollection ReadCollection(string path)
        {
            TextureDataCollection tdc = new TextureDataCollection();

            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fs = File.OpenRead(path))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        tdc = (TextureDataCollection)bf.Deserialize(fs);
                    }
                }
                catch (Exception EX) { throw new Exception("Could not deserilize " + path + " Because " + EX.Message, EX); }
            }
            else throw new  FileNotFoundException(path + " does not exsist");

            return tdc;
        }

        public static void WriteCollection(string path,TextureDataCollection tdc)
        {
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, tdc);
                }
            }
            catch(Exception EX)
            {
                throw new Exception("writing the file failed because " + EX.Message,EX); 
            }
        }

        #region Ilist

        public int IndexOf(TextureData item)
        {
            return textures.IndexOf(item);
        }

        public void Insert(int index, TextureData item)
        {
            textures.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            textures.RemoveAt(index);
        }

        public TextureData this[int index]
        {
            get
            {
                return textures[index];
            }
            set
            {//this is the whole reason we implement a custom collection!  ( I guess later if we need to customize this functionality )
                throw new NotImplementedException("cannot change data!");
            }
        }

        public void Add(TextureData item)
        {
            textures.Add(item);
        }

        public void Clear()
        {
            textures.Clear();
        }

        public bool Contains(TextureData item)
        {
            return textures.Contains(item);
        }

        public void CopyTo(TextureData[] array, int arrayIndex)
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

        public bool Remove(TextureData item)
        {
            return textures.Remove(item);
        }

        public IEnumerator<TextureData> GetEnumerator()
        {
            return textures.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return textures.GetEnumerator();
        }
        #endregion
    }

    [Serializable]
    public struct TextureData
    {
        string path;
        public string Path
        {
            get { return path; }
        }
        
        TCoord texcoord;
        public TCoord Texcoord 
        {
            get { return texcoord; }
        }
                

        int index;
        public int Index { get { return index; } }
        

        public TextureData(string p,TCoord texC,int i)
        {
            path = p;
            texcoord = texC;
            index = i;
        }


    }

    [Serializable]
    public struct TCoord
    {
        public float u;
        public float v;

        public static implicit operator SharpDX.Vector2(TCoord tc)
        {
            return new Vector2(tc.u,tc.v);
        }

        public static explicit operator TCoord(SharpDX.Vector2 vec)
        {
            return new TCoord() { u = vec.X, v = vec.Y };
        }


        public override string ToString()
        {
            return "u: " + u + "|v: " + v;
        }
    }
}
