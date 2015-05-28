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

        Dictionary<string, GameMap> maps;       


        public GameProject()
        {
            maps = new Dictionary<string, GameMap>();
        }

        public void AddMap(string name, GameMap map)
        {
            if (!maps.ContainsKey(name))
            {
                

                maps.Add(name, map);
            }
        }

        public void RemoveMap(string name)
        {
            if (maps.ContainsKey(name))
            {
                maps.Remove(name);
            }
        }

        public GameMap this[string name]
        {
            get
            {
                if (maps.ContainsKey(name ?? ""))
                {
                    return maps[name];
                }
                else { throw new IndexOutOfRangeException(name + " does not exist"); }
            }
        }

        public bool Contains(string name)
        {
            return maps.ContainsKey(name);
        }

        public string[] GetKeys()
        {
            return maps.Keys.ToArray();
        }
        
        public void UpdateGlobal(Device d,Vector3 gpos)
        {
            foreach(string name in maps.Keys)
            {

                maps[name].UpdateGraphics(d, gpos);

            }
        }

        public IEnumerator<GameMap> GetEnumerator()
        {
            return maps.Values.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return maps.Values.GetEnumerator();
        }

        public void Dispose()
        {
            foreach(GameMap gm in maps.Values)
            {
                gm.Dispose();
            }

            maps.Clear();
        }
    }
}
