using System;

namespace Compiler.Environment
{
    /// <summary>
    /// Instructions base class.
    /// </summary>
    class Instruction
    {
        public int Id;
        public string Name;
        public int[] Parameters;
    
        public Instruction(int id, string name, int[] parameters)
        {
            Id = id;
            Name = name;
            Parameters = parameters;
        }
    }
}
