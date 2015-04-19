using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quadrum
{
    static class Renderer
    {
        static List<IRendered> RenderedItems;

        static Renderer()
        {
            RenderedItems = new List<IRendered>();
        }



    }

    public interface IRendered
    {
        
    }

     
}
