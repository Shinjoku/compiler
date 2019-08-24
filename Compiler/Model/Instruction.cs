using System;
using System.Collections.Generic;

namespace Compiler.Environment
{
    /// <summary>
    /// Instructions base class.
    /// </summary>
    public class Instruction
    {
        public static List<string> LabelInstructions = new List<string>
        {
            "jmp",
            "jmpf",
            "call"
        };

        public int Id;
        public string Name;
        public int[] Parameters;
    
        public Instruction(int id, string name, int[] parameters)
        {
            Id = id;
            Name = name;
            Parameters = parameters;
        }

        public static string[] GetInfos(string line)
        {
            var currentLine = line.Replace("\r", "");
            return currentLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static List<Instruction> ExtractInstructions(string txt)
        {
            int i = 0;
            int lineCounter = 0;
            string[] strParameters;
            var lines = txt.ToLower().Split('\n');
            var result = new List<Instruction>();
            var labels = new Dictionary<string, string>();

            foreach(var readLine in lines)
            {
                if (readLine == string.Empty) break;

                var infos = GetInfos(readLine);
                if (infos[0] == "null")
                    labels.Add(infos[1], lineCounter.ToString());
                lineCounter++;
            }

            foreach (var line in lines)
            {
                if (line == string.Empty) break;

                var infos = GetInfos(line);
                var parameters = new int[] { };

                // If it's a Label Instruction, changes the label for it's line value
                if (LabelInstructions.Contains(infos[0]))
                {
                    infos[1] = labels[infos[1]];
                }

                if (infos[0] != "null")
                {
                    if (infos.Length > 1)
                    {
                        strParameters = infos[1].Replace(" ", "").Split(',');

                        if (strParameters.Length > 1)
                            parameters = new int[] { int.Parse(strParameters[0]), int.Parse(strParameters[1]) };
                        else
                            parameters = new int[] { int.Parse(strParameters[0]) };
                    }
                }

                result.Add(new Instruction(i, infos[0], parameters));
                i++;
            }

            return result;
        }

    }
}
