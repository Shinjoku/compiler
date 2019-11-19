using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Model;

namespace Compiler.General
{
    public class PostfixExpressionBuilder
    {
        public List<Token> Expression { get; }
        private Stack<Token> _operatorsStack;

        public PostfixExpressionBuilder()
        {
            Expression = new List<Token>();
            _operatorsStack = new Stack<Token>();
        }

        public void Stack(Token t)
        {

            if (LPD.IdentifierTypes.ContainsKey(t.Symbol))
            {
                Expression.Add(t);
            }
            else
            {
                if (_operatorsStack.Count > 0 && t.Symbol != LPD.Symbol.OPEN_PARENTHESIS)
                { 
                    while (LPD.OperatorsPriority[_operatorsStack.First().Symbol] > LPD.OperatorsPriority[t.Symbol])
                    {
                        Unstack();
                    }
                }
                _operatorsStack.Push(t);
            }
        }

        private void Unstack()
        {
            Expression.Add(_operatorsStack.Pop());
        }

        public void UnstackUntilParenthesis()
        {
            while(_operatorsStack.FirstOrDefault()?.Symbol != LPD.Symbol.OPEN_PARENTHESIS)
            {
                Unstack();
            }

            // Removes the open parenthesis
            _operatorsStack.Pop();
        }

        public List<Token> ToList()
        {
            while (_operatorsStack.Count != 0)
                Unstack();

            return Expression;
        }
    }
}
