﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quadrum.Map
{
    //not sure how this is going to work yet
    //use this to help build dynamic expressions
    public abstract class CellProperty<T>
    {
        public abstract  int ID { get; }

        public abstract T Value { get; set; }       
    }


}
