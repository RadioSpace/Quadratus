using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Quadrum
{
    //a base class for all game objects
    public class Gob : ICellMember
    {
        Dictionary<CellMemberEvents, EventHandler<CellMemberEventArgs>> events;

        public EventHandler<CellMemberEventArgs> this[CellMemberEvents ce]
        {
            get
            {
                return events[ce];
            }
            set
            {
                events[ce] = value;
            }
        }

        public Gob()
        {
            
        }


    }

    public interface ICellMember
    {
        EventHandler<CellMemberEventArgs> this[CellMemberEvents ce] { get; set; }
    }

    public class CellMemberEventArgs : EventArgs
    {
        public readonly ICellMember member;

        public CellMemberEventArgs(ICellMember mem)
        {
            member = mem;
        }


    }
}
