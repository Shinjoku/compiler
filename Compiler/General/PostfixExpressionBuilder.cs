using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compiler.Model;

namespace Compiler.General
{
    public class PostfixExpressionBuilder
    {
        private StringBuilder _postfixExpression;
        private Stack<Token> _operatorsStack;

        public PostfixExpressionBuilder()
        {
            _postfixExpression = new StringBuilder();
            _operatorsStack = new Stack<Token>();
        }

        public void Stack(Token t)
        {
            while (LPD.OperatorsPriority[_operatorsStack.First().Symbol] > LPD.OperatorsPriority[t.Symbol])
            {
                Unstack();
            }

            if (LPD.IdentifierTypes.Keys.Contains(t.Symbol))
                _postfixExpression.Append(t.Lexeme);
            else
                _operatorsStack.Push(t);
        }

        private void Unstack()
        {
            _postfixExpression.Append(_operatorsStack.Pop());
        }

        public override string ToString()
        {
            while (_operatorsStack.Count != 0)
                Unstack();

            return _postfixExpression.ToString();
        }
    }
}
