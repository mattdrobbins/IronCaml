using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class Interperater : Statement.Visitor<object>, Expression.Visitor<object>
    {
        private Dictionary<string, object> _locals = new();

        public Interperater(bool interactive = false)
        {
            Interactive = interactive;    
        }

        public bool Interactive { get; }

        public void Interperate(List<Statement> statements)
        {
            try
            {
                foreach (Statement statment in statements)
                {
                    Execute(statment);
                }
            }

            catch (RuntimeException e)
            {
                Program.RuntimeError(e);
            }
        }

        public object VisitBinaryExpression(Expression.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.PLUS:
                    if (left is long && right is long)
                    {
                        return (long)left + (long)right;
                    }
                    throw new RuntimeException(expr.Operator,
                        "Invalid addition");
                default:
                    return null;
            }
        }

        public object VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            var result = Evaluate(stmt.Expression);
            if (Interactive)
            {
                Console.WriteLine(result);
            }

            return null;
        }

        public object VisitFunctionStatement(Statement.Function stmt)
        {
            throw new NotImplementedException();
        }

        public object VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            object value = null;

            if (stmt.Expression != null)
            {
                value = Evaluate(stmt.Expression);
            }

            _locals[stmt.Name.Lexeme] = value;

            if (Interactive)
            {
                Console.WriteLine($"val {stmt.Name.Lexeme} : {value.GetType()} = {value}");
            }

            return value;
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return expr.Value;
        }

        public object VisitVariableExpr(Expression.Variable expr)
        {
            return _locals[expr.Name.Lexeme];
        }

        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private object Execute(Statement stmt)
        {
            return stmt.Accept(this);
        }
    }
}
