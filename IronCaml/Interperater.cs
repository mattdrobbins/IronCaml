using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class Interperater : Statement.Visitor<object>, Expression.Visitor<object>
    {
        private Dictionary<string, object> env = new Dictionary<string, object>();

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
                IronCaml.RuntimeError(e);
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
            var function = new OCamlFunction(stmt, env);
            env[stmt.Name.Lexeme] = function;
            return null;
        }

        public object VisitCallExpression(Expression.Call expr)
        {
            var callee = Evaluate(expr.Callee);
            List<object> args = new List<object>();

            foreach (var arg in expr.Arguments)
            {
                args.Add(Evaluate(arg));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeException(null, "Call only call functions and classes");
            }

            ICallable function = (ICallable)callee;

            if (args.Count != function.Arity())
            {
                throw new RuntimeException(null, $"Expected {function.Arity()} arguments but got " +
                    $"{args.Count}");
            }

            return function.Call(this, args);
        }

        public object CallFunction(Expression exp, Dictionary<string, object> arguments)
        {
            var previous = new Dictionary<string, object>(this.env);

            this.env = arguments;
            var result = Evaluate(exp);
            this.env = previous;

            return result;
        }

        public object VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            object value = null;

            if (stmt.Expression != null)
            {
                value = Evaluate(stmt.Expression);
            }

            env[stmt.Name.Lexeme] = value;

            if (Interactive)
            {
                Console.WriteLine($"val {stmt.Name.Lexeme} : {value.GetType()} = {value}");
            }

            return value;
        }

        public object VisitGroupingExpression(Expression.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return expr.Value;
        }

        public object VisitVariableExpr(Expression.Variable expr)
        {
            return env[expr.Name.Lexeme];
        }

        private object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private object Execute(Statement stmt)
        {
            return stmt.Accept(this);
        }

        public object VisitUnaryExpression(Expression.Unary expr)
        {
            throw new NotImplementedException();
        }
    }
}
