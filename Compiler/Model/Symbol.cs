using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compiler.Model;

namespace Compiler.Model
{
    public class Symbol
    {
        public string Label { get; set; }
        public uint Scope { get; set; }
        public LPD.IdentifierType IdentifierType { get; set; }

        public Symbol(string label, uint scope, LPD.IdentifierType type)
        {
            Label = label;
            IdentifierType = type;
            Scope = scope;
        }
    }
}
