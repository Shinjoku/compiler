using System;
using System.Collections.Generic;

namespace Compiler.Environment
{
    /// <summary>
    /// Instructions base class.
    /// </summary>
    public class Instruction
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

        public static List<Instruction> ExtractInstructions(string txt)
        {
            int i = 0;
            string[] strParameters;
            var lines = txt.Split('\n');
            var result = new List<Instruction>();

            foreach(var line in lines)
            {
                var currentLine = line.Replace("\r", "");
                var infos = currentLine.Split(' ');
                var parameters = new int[] { };

                if (infos.Length > 1)
                {
                    strParameters = infos[1].Replace(" ", "").Split(',');

                    if (strParameters.Length > 1)
                        parameters = new int[] { int.Parse(strParameters[0]), int.Parse(strParameters[1]) };
                    else
                        parameters = new int[] { int.Parse(strParameters[0]) };
                }

                result.Add(new Instruction(i, infos[0], parameters));
                i++;
            }

            return result;
        }
    }
}
