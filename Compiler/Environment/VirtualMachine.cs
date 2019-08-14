using System;
using System.Collections.Generic;


namespace Compiler.Environment
{
    /// <summary>
    /// Runtime environment that execute all given instructions.
    /// </summary>
    class VirtualMachine
    {
        public static bool Run(List<Instruction> instructions)
        {
            int PC = 0;
            bool fin = false;
            Memory memory = new Memory();

            while (PC < instructions.Count)
            {
                Console.WriteLine("Executando linha " + instructions[PC].Id +
                    ". Comando: " + instructions[PC].Name + ". Valor SP: " +
                    memory.StackPointer);

                try
                {
                    switch (instructions[PC].Name)
                    {
                        case "start":
                            memory.Start();
                            break;

                        case "ldc":
                            memory.LoadConstant(instructions[PC].Parameters[0]);
                            break;

                        case "ldv":
                            memory.LoadValue(instructions[PC].Parameters[0]);
                            break;

                        case "str":
                            memory.Store(instructions[PC].Parameters[0]);
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
                            PC = memory.Jump(instructions[PC].Parameters[0]);
                            break;

                        case "jmpf":
                            PC = memory.JumpIfFalse(instructions[PC].Parameters[0], PC);
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
                            memory.Allocate(instructions[PC].Parameters[0],
                                instructions[PC].Parameters[1]);
                            break;

                        case "dalloc":
                            memory.Deallocate(instructions[PC].Parameters[0],
                                instructions[PC].Parameters[1]);
                            break;

                        case "call":
                            PC = memory.Call(instructions[PC].Parameters[0], PC);
                            break;

                        case "return":
                            PC = memory.Return();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Line " + instructions[PC].Id + ": " + e);
                }
                if (fin) break;

                PC++;
            }
            if (fin) return true;
            return false;
        }
    }
}
