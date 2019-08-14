using System;
using System.Collections.Generic;


namespace Compiler.Environment
{
    /// <summary>
    /// Runtime environment that execute all given instructions.
    /// </summary>
    class VirtualMachine
    {
        public static bool Run(List<Instruction> programCounter)
        {
            int i = 0;
            bool fin = false;
            Memory memory = new Memory();

            while (i < programCounter.Count)
            {
                Console.WriteLine("Executando linha " + programCounter[i].Id +
                    ". Comando: " + programCounter[i].Name + ". Valor SP: " +
                    memory.StackPointer);

                try
                {
                    switch (programCounter[i].Name)
                    {
                        case "start":
                            memory.Start();
                            break;

                        case "ldc":
                            memory.LoadConstant(programCounter[i].Parameters[0]);
                            break;

                        case "ldv":
                            memory.LoadValue(programCounter[i].Parameters[0]);
                            break;

                        case "str":
                            memory.Store(programCounter[i].Parameters[0]);
                            break;

                        case "add":
                            memory.Add();
                            break;

                        case "sub":
                            memory.Subtract();
                            break;

                        case "mult":
                            memory.Multiply();
                            break;

                        case "divi":
                            memory.DivideImediate();
                            break;

                        case "inv":
                            memory.Invert();
                            break;

                        case "and":
                            memory.And();
                            break;

                        case "or":
                            memory.Or();
                            break;

                        case "neg":
                            memory.Negate();
                            break;

                        case "cme":
                            memory.CompareLess();
                            break;

                        case "cma":
                            memory.CompareGreater();
                            break;

                        case "ceq":
                            memory.CompareEquals();
                            break;

                        case "cdif":
                            memory.CompareDifference();
                            break;

                        case "cmeq":
                            memory.CompareLessOrEqual();
                            break;

                        case "cmaq":
                            memory.CompareGreaterOrEqual();
                            break;

                        case "hlt":
                            fin = true;
                            break;

                        case "jmp":
                            i = memory.Jump(programCounter[i].Parameters[0]);
                            break;

                        case "jmpf":
                            i = memory.JumpIfFalse(programCounter[i].Parameters[0], i);
                            break;

                        case "null":
                            break;

                        case "rd":
                            memory.Read(int.Parse(Console.ReadLine()));
                            break;

                        case "prn":
                            memory.Print();
                            break;

                        case "alloc":
                            memory.Allocate(programCounter[i].Parameters[0],
                                programCounter[i].Parameters[1]);
                            break;

                        case "dalloc":
                            memory.Deallocate(programCounter[i].Parameters[0],
                                programCounter[i].Parameters[1]);
                            break;

                        case "call":
                            i = memory.Call(programCounter[i].Parameters[0], i);
                            break;

                        case "return":
                            i = memory.Return();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Line " + programCounter[i].Id + ": " + e);
                }

                i++;
            }
            if (fin) return true;
            return false;
        }
    }
}
