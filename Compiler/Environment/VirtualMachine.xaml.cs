using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compiler.Environment
{
    /// <summary>
    /// Runtime environment that execute all given instructions.
    /// </summary>
    public partial class VirtualMachine : UserControl
    {
        public Memory Memory { get; set; }

        public VirtualMachine()
        {
            InitializeComponent();
            Memory = new Memory();
        }

        public async Task<bool> Run(List<Instruction> instructions)
        {
            int PC = 0;
            bool fin = false;

            while (PC < instructions.Count)
            {
                Console.WriteLine("Executando linha " + instructions[PC].Id +
                    ". Comando: " + instructions[PC].Name + ". Valor SP: " +
                    Memory.StackPointer);

                try
                {
                    switch (instructions[PC].Name)
                    {
                        case "start":
                            Memory.Start();
                            PC++;
                            break;

                        case "ldc":
                            Memory.LoadConstant(instructions[PC].Parameters[0]);
                            PC++;
                            break;

                        case "ldv":
                            Memory.LoadValue(instructions[PC].Parameters[0]);
                            PC++;
                            break;

                        case "str":
                            Memory.Store(instructions[PC].Parameters[0]);
                            PC++;
                            break;

                        case "add":
                            Memory.Add();
                            PC++;
                            break;

                        case "sub":
                            Memory.Subtract();
                            PC++;
                            break;

                        case "mult":
                            Memory.Multiply();
                            PC++;
                            break;

                        case "divi":
                            Memory.DivideImediate();
                            PC++;
                            break;

                        case "inv":
                            Memory.Invert();
                            PC++;
                            break;

                        case "and":
                            Memory.And();
                            PC++;
                            break;

                        case "or":
                            Memory.Or();
                            PC++;
                            break;

                        case "neg":
                            Memory.Negate();
                            PC++;
                            break;

                        case "cme":
                            Memory.CompareLess();
                            PC++;
                            break;

                        case "cma":
                            Memory.CompareGreater();
                            PC++;
                            break;

                        case "ceq":
                            Memory.CompareEquals();
                            PC++;
                            break;

                        case "cdif":
                            Memory.CompareDifference();
                            PC++;
                            break;

                        case "cmeq":
                            Memory.CompareLessOrEqual();
                            PC++;
                            break;

                        case "cmaq":
                            Memory.CompareGreaterOrEqual();
                            PC++;
                            break;

                        case "hlt":
                            fin = true;
                            break;

                        case "jmp":
                            PC = Memory.Jump(instructions[PC].Parameters[0]);
                            break;

                        case "jmpf":
                            PC = Memory.JumpIfFalse(instructions[PC].Parameters[0], PC);
                            break;

                        case "null":
                            PC++;
                            break;

                        case "rd":
                            Memory.Read(int.Parse(Console.ReadLine()));
                            PC++;
                            break;

                        case "prn":
                            await Task.Run(() => In.Text += Memory.Print() + '\n');
                            PC++;
                            break;

                        case "alloc":
                            Memory.Allocate(instructions[PC].Parameters[0],
                                instructions[PC].Parameters[1]);
                            PC++;
                            break;

                        case "dalloc":
                            Memory.Deallocate(instructions[PC].Parameters[0],
                                instructions[PC].Parameters[1]);
                            PC++;
                            break;

                        case "call":
                            PC = Memory.Call(instructions[PC].Parameters[0], PC);
                            break;

                        case "return":
                            PC = Memory.Return();
                            break;

                        default: throw new WrongCommandException();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Line " + instructions[PC].Id + ": " + e);
                    break;
                }
                if (fin) break;
            }
            if (fin) return true;
            return false;
        }

    }
    class WrongCommandException : Exception { };
}
