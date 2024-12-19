using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class VariableResolver : Statement.Visitor<object>
    {
        private readonly LinqExpressionCreator _creator;

        public VariableResolver(LinqExpressionCreator creator)
        {
            _creator = creator;
        }

        public void Resolve(List<Statement> statements)
        {
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
        }

        public object VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            return null;
        }

        public object VisitFunctionStatement(Statement.Function stmt)
        {
            return null;
        }

        public object VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            _creator.SetGlobals(stmt.Name.Lexeme, LinqExpression.Variable(stmt.Expression.ResultType(), stmt.Name.Lexeme));
            return null;
        }
    }
}
