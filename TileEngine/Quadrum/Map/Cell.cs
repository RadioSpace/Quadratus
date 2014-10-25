using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;



namespace Quadrum.Map
{

    using Debug = System.Diagnostics.Debug;
    using Buffer = SharpDX.Direct3D11.Buffer;
    using Device = SharpDX.Direct3D11.Device;

    [Serializable]
    public class CellCollection : IEnumerable<Cell>
    {
        Cell[] cells;

        int count;
        public int Count { get { return count; } }

        public CellCollection(Grid grid)
        {
            count = grid.Volume;
            cells = new Cell[count];

            //populate positions
            grid.ForAllCells((i,x,y)=>{
                cells[i] = new Cell(0,new SVector3(x * grid.CellSize.Width,y * grid.CellSize.Height,0));
            });


        }

        public Cell this[int index]
        {
            get 
            {
                if(checkbounds(index))
                {
                    return cells[index];
                }
                else throw new IndexOutOfRangeException("index is out side the bounds of the array: index is " + index + " : size is " + count);                
            }
        }


        public IEnumerator<Cell> GetEnumerator()
        {
            return cells.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return cells.GetEnumerator();
        }

        bool checkbounds(int index)
        {
            return index > -1 || index < count;
        }

        /// <summary>
        /// returns a structured buffer
        /// </summary>
        /// <param name="d">the device used for rendering</param>
        /// <returns>a buffer that can be used to pass a structured buffer to a shader in the shader pipeline</returns>
        public Buffer ToBuffer(Device d)
        {
            Buffer cellbuffer;

            try
            {
                 cellbuffer = Buffer.Create(d, cells, new SharpDX.Direct3D11.BufferDescription()
                {
                    BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.BufferStructured,
                    SizeInBytes = SharpDX.Utilities.SizeOf(cells),
                    StructureByteStride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Cell)),
                    Usage = SharpDX.Direct3D11.ResourceUsage.Default
                });
            }
            catch (Exception EX)
            {
#if DEBUG
                Debug.WriteLine("\r\n\r\n.......ERROR\r\n\r\nfailed to createBuffer in Cell.ToBuffer\r\n" + EX.Message + "\r\n\r\n" + EX.StackTrace + "\r\n\r\nEND...\r\n\r\n");
#endif
                cellbuffer = null;

            }

            return cellbuffer;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size=12)]
    public struct Cell : IEquatable<Cell>
    {
        int spriteindex;//4
        public int SpriteIndex { get { return spriteindex; } }
        
        SVector3 position;//8
        public SVector3 Position { get { return position; } }
        
        public Cell(int sindex , SVector3 pos)
        {
            spriteindex = sindex;
            position = pos;
            
        }

        public override bool Equals(object obj)
        {
            if (obj is Cell) { return Equals((Cell)obj); }
            else return false;            
        }

        public override int GetHashCode()
        {
            int h = 73327;//are larger primes better ???
            h = h * 64927 + spriteindex.GetHashCode();
            h = h * 64927 + position.GetHashCode();

            return h;
        }

        public override string ToString()
        {
            return "s:" + spriteindex + "|p:{" + position.ToString()+"}";
        }


        //I guess I really don't need the equals stuff. but it doesn't hurt.
        public static bool operator ==(Cell a, Cell b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Cell a, Cell b)
        { 
            return !a.Equals(b);
        }

        public bool Equals(Cell other)
        {
            return other.position == position && other.spriteindex == spriteindex;
        }
    }
}
