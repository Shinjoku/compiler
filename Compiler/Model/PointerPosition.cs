using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Model
{
    class PointerPosition
    {
        public int Column { get; set; }
        public int Line { get; set; }

        public PointerPosition()
        {
            Column = 0;
            Line = 0;
        }
    }
}
