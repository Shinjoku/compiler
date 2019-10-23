using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Model
{
    public class Symbol
    {
        public string Label { get; set; }
        public int Type { get; set; }
        public int Scope { get; set; }
        public bool IsVariable { get; set; }

        public Symbol(string label, int type, int scope, bool isVariable)
        {
            Label = label;
            Type = type;
            Scope = scope;
            IsVariable = isVariable;
        }
    }
}
