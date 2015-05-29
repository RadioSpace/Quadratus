using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Direct3D11;

namespace STAR
{
    [Serializable]
    public class GameProject : IEnumerable<GameMap>,IDisposable
    {
        static object locker = new object();

        Dictionary<string, GameMap> maps;       
        

        public GameProject()
        {
            maps = new Dictionary<string, GameMap>();
        }

        public bool AddMap(string name, GameMap map)
        {
            lock (locker)
            {
                if (!maps.ContainsKey(name))
                {
                    maps.Add(name, map);
                    return true;
                }
                else return false;
            }
        }

        public void RemoveMap(string name)
        {
            lock (locker)
            {
                if (maps.ContainsKey(name))
                {
                    maps.Remove(name);
                }
            }
        }

        public GameMap this[string name]
        {
            get
            {
                lock (locker)
                {
                    if (maps.ContainsKey(name ?? ""))
                    {
                        return maps[name];
                    }
                    else { throw new IndexOutOfRangeException(name + " does not exist"); }
                }
            }
        }

        public bool Contains(string name)
        {
            lock (locker){ return maps.ContainsKey(name);}
        }

        public string[] GetKeys()
        {
            lock (locker) { return maps.Keys.ToArray(); }
        }
        
        public void UpdateGlobal(Device d,Vector3 gpos)
        {
            lock (locker)
            {
                foreach (string name in maps.Keys)
                {
                    maps[name].UpdateGraphics(d, gpos);
                }
            }
        }

        public void Clear()
        {
            foreach(GameMap gm in maps.Values)
            {
                gm.Dispose();
            }
            maps.Clear(); 

        }

        public IEnumerator<GameMap> GetEnumerator()
        {
            lock (locker)
            {
                return maps.Values.AsEnumerable().GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (locker)
            {
                return maps.Values.GetEnumerator();
            }
        }

        public void Dispose()
        {
            lock (locker)
            {
                foreach (GameMap gm in maps.Values)
                {
                    gm.Dispose();
                }

                maps.Clear();
            }
        }
    }
}
