using Compiler.Model.Interfaces;
using Compiler.Model;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Compiler.General
{
    public class Semantic : IAnalyzable
    {
        private PostfixExpressionBuilder _expressionBuilder;
        public List<Token> PostfixExpression { get => _expressionBuilder.ToList(); }

        public Semantic()
        {
            _expressionBuilder = new PostfixExpressionBuilder();
        }

        public bool Run(string filepath)
        {
            return false;
        }

        public void AddToExpression(Token t)
        {
            _expressionBuilder.Stack(t);
        }

        public void UnstackUntilOpenParenthesis()
        {
            _expressionBuilder.UnstackUntilParenthesis();
        }
    }
}
