using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpDX;
using SharpDX.Direct3D11;

namespace Quadrum.Map
{
    using Buffer = SharpDX.Direct3D11.Buffer;

    public class CellManager
    {




        CellCollection cells;

        //just a concept
        List<CellMemberEventArgs> members;
  



        public void AddMember(ICellMember member )
        {

            
            members.Add(new CellMemberEventArgs(member));

                
        }




        public void RemoveMember(ICellMember member)
        {
           
        }


        private void LeaveCell(object sender, CellMemberEventArgs e)
        {

        }

        private void EnterCell(object sender, CellMemberEventArgs e)
        {

        }

        internal void Initialize()
        {
        
        }

        internal void Update(long miliseconds)
        {
            for (int x = 0; x < cells.Count; x++)
            {
                OnUpdated(new CellEventArgs(cells[x], miliseconds));
            }
        }

        internal void Draw() { }
        
        public Buffer GetDirect3D11StructuredBuffer(Device d)
        {
            return cells.ToBuffer(d);
        }        

        //we will make more of these
        event EventHandler<CellEventArgs> Updated;
        protected virtual void OnUpdated(CellEventArgs e) { if (Updated != null)Updated(this, e); }


        bool memberexists(ICellMember member)
        {
            return members.Select(a => a.member).Contains(member);
        }
               
    }





    public class CellEventArgs : EventArgs
    {
        public readonly Cell target;
        public readonly long miliseconds;

        public CellEventArgs(Cell tar,long ml)
        {
            target = tar;
            miliseconds = ml;
        }
    }



    
}
