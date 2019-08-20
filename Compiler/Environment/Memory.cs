using System.Collections.Generic;


namespace Compiler.Environment
{
    /// <summary>
    /// Contains instructions and methods to manipulate the Memory Stack
    /// </summary>
    public class Memory
    {
        public List<int> DataStack { get; set; }
        public int StackPointer { get; set; }

        public Memory()
        {
            DataStack = new List<int>();
        }

        #region Instructions

        // S:= s - 1
        public void Start() => StackPointer = -1;

        // S = s + 1, M[s] = k
        public void LoadConstant(int kI)
        {
            int k = kI;
            DataStack.Add(0);
            StackPointer++;
            DataStack[StackPointer] = k;
        }

        // S = s + 1, M[s] = M[n]
        public void LoadValue(int nI)
        {
            DataStack.Add(0);
            StackPointer++;
            DataStack[StackPointer] = DataStack[nI];
        }

        // M[s - 1] = M[s - 1] + M[s], S = s - 1
        public void Add()
        {
            DataStack[StackPointer - 1] = 
                DataStack[StackPointer - 1] + DataStack[StackPointer];

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[s - 1] = M[s - 1] - M[s], S = s - 1
        public void Subtract()
        {
            DataStack[StackPointer - 1] = 
                DataStack[StackPointer - 1] - DataStack[StackPointer];

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[s - 1] = M[s - 1] * M[s], S = s - 1
        public void Multiply()
        {
            DataStack[StackPointer - 1] = 
                DataStack[StackPointer - 1] * DataStack[StackPointer];

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[s - 1] = M[s - 1] / M[s], S = s - 1
        public void DivideImediate()
        {
            DataStack[StackPointer - 1] =
                DataStack[StackPointer - 1] / DataStack[StackPointer];

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[s] = -M[s]
        public void Invert()
        {
            DataStack[StackPointer] = DataStack[StackPointer] * -1;
        }

        // If M [s-1] = 1 and M[s] = 1, then M[s-1]:=`, else M[s-1]:=0;  s:=s - 1 
        public void And()
        {
            if (DataStack[StackPointer - 1] == 1 && DataStack[StackPointer] == 1)
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] = 1 or M[s] = 1, then M[s-1] =1, else M[s-1] =0; s =s - 1 
        public void Or()
        {
            if (DataStack[StackPointer - 1] == 1 || DataStack[StackPointer] == 1)
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[s]:=1 - M[s] 
        public void Negate()
        {
            DataStack[StackPointer] = 1 - DataStack[StackPointer];
        }

        // If M[s-1] < M[s], then M[s-1]:=1 else M[s-1]:=0; s:=s - 1 
        public void CompareLess()
        {
            if (DataStack[StackPointer - 1] < DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] > M[s], then M[s-1]:=1 else M[s-1]:=0; s:=s - 1 
        public void CompareGreater()
        {
            if (DataStack[StackPointer - 1] > DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] = M[s], then M[s-1]:=1 else M[s-1]:=0; s:=s - 1 
        public void CompareEquals()
        {
            if (DataStack[StackPointer - 1] == DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] != M[s], then M[s-1]:=1, else M[s-1]:=0; s:=s - 1 
        public void CompareDifference()
        {
            if (DataStack[StackPointer - 1] != DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] <= M[s], then M[s-1]:=1, else M[s-1]:=0; s:=s - 1 
        public void CompareLessOrEqual()
        {
            if (DataStack[StackPointer - 1] <= DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // If M[s-1] >= M[s], then M[s-1]:=1 else M[s-1]:=0; s:=s - 1 
        public void CompareGreaterOrEqual()
        {
            if (DataStack[StackPointer - 1] >= DataStack[StackPointer])
                DataStack[StackPointer - 1] = 1;
            else
                DataStack[StackPointer - 1] = 0;

            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // M[n]:= M[s]; s:= s-1 
        public void Store(int n)
        {
            DataStack[n] = DataStack[StackPointer];
            DataStack.RemoveAt(StackPointer);
            StackPointer--;
        }

        // i = t
        public int Jump(int n)
        {
            return n;
        }

        // If M[s] = 0 then i:=t, else i:=i + 1; s:=s-1 
        public int JumpIfFalse(int n, int i)
        {
            var currentValue = DataStack[StackPointer];
            StackPointer--;
            if (DataStack[StackPointer] == 0)
            {
                return n;
            }
            else
            {
                return i + 1;
            }
        }

        // S:=s + 1; M[s]:= “next input value”. 	
        public void Read(int value)
        {
            DataStack.Add(value);
            StackPointer++;
        }

        //  “Print M[s]”; s:=s-1
        public int Print()
        {
            int currentValue = DataStack[StackPointer];
            DataStack.RemoveAt(StackPointer);
            StackPointer--;
            return currentValue;
        }

        // For k:=0 till n-1, do { s:=s + 1; M[s]:=M[m+k] } 
        public void Allocate(int mI, int nI)
        {
            int k;
            for (k = 0; k <= (nI - 1); k++)
            {
                StackPointer++;
                DataStack.Add(mI + k);
            }
        }

        // Para k:=n-1 até 0 faça {M[m+k]:=M[s]; s:=s - 1} 
        public void Deallocate(int mI, int nI)
        {
            int k;
            for (k = (nI - 1); k >= 0; k--)
            {
                DataStack[mI + k] = DataStack[StackPointer];
                DataStack.RemoveAt(StackPointer);
                StackPointer--;
            }
        }

        // S:=s + 1; M[s]:= i+ 1; i:=t 
        public int Call(int n, int i)
        {
            StackPointer++;
            DataStack[StackPointer] = i + 1;
            return n;
        }

        // i:=M[s]; s:=s - 1 
        public int Return()
        {
            int currentValue = DataStack[StackPointer];
            DataStack.RemoveAt(StackPointer);
            StackPointer--;
            return currentValue;
        }

        #endregion
    }

}
