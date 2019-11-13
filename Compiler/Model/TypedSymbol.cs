using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Model
{
    public class TypedSymbol : Symbol
    {
        public LPD.ValueType ReturnType { get; set; }

        public TypedSymbol(string label, uint scope, LPD.IdentifierType type)
            : base(label, scope, type){}

        public TypedSymbol(string label, uint scope, LPD.IdentifierType type, LPD.ValueType returnType)
            : base(label, scope, type)
        {
            ReturnType = returnType;
        }
    }
}
