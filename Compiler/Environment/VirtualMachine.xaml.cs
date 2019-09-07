using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Compiler.Environment
{
    /// <summary>
    /// Runtime environment that execute all given instructions.
    /// </summary>
    public partial class VirtualMachine : UserControl, INotifyPropertyChanged
    {
        private object _inTextLock = new object();
        private object _outTextLock = new object();

        #region Properties

        public Memory Memory { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set
            {
                if (value != _inputText)
                {
                    lock (_inTextLock)
                    {
                        _inputText = value;
                        OnPropertyChanged("InputText");
                    }

                }
            }
        }

        private string _outputText;
        public string OutputText
        {
            get { return _outputText; }
            set
            {
                if (value != _outputText)
                {
                    lock (_outTextLock)
                    {
                        _outputText = value;
                        OnPropertyChanged("OutputText");
                    }

                }
            }
        }

        #endregion

        public VirtualMachine()
        {
            InitializeComponent();
            DataContext = this;
            Memory = new Memory();
        }

        private void Clear()
        {
            InputText = OutputText = string.Empty;
            Memory.ClearMemory();
        }

        public async Task<bool> Run(List<AssemblyInstruction> instructions)
        {
            int PC = 0;
            bool fin = false;
            Clear();

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
                            await App.Current.Dispatcher.BeginInvoke((Action) (() =>
                            {
                                UserInput userInput = new UserInput();
                                userInput.SendInput += value =>
                                {
                                    InputText += value + "\n";
                                    Memory.Read(int.Parse(value));
                                };
                                userInput.Owner = Application.Current.MainWindow;
                                userInput.ShowDialog();

                                PC++;
                            }));
                            break;

                        case "prn":
                            var printOk = await Task.Run(() =>
                            {
                                try
                                {
                                    OutputText += Memory.Print() + "\n";
                                    return true;
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    return false;
                                }
                            });
                            if (!printOk) throw new ArgumentOutOfRangeException();
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

                        default: throw new WrongCommandException("Unknown command has been found. Check your code.");
                    }
                }
                catch(ArgumentOutOfRangeException)
                {
                    throw new Exception("You should take a look at your indexes!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Line " + instructions[PC].Id + ": " + e);
                    throw new Exception("Unknown error has occured.");
                }
                if (fin) break;
            }
            if (fin) return true;
            throw new SystemException("The machine wasn't closed successfully. Have you forgot the 'hlt' command?");
        }

    }
    public class WrongCommandException : Exception
    {
        public WrongCommandException(string msg) : base(msg) {}
    };
}
