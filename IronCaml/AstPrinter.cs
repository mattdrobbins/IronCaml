using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class AstPrinter : Expression.Visitor<string>
    {
        public string Print(Expression expression)
        {
            return expression.Accept(this);
        }

        public string VisitGroupingExpression(Expression.Grouping expr)
        {
            return Parenthesise("group", expr.Expression);
        }

        public string VisitBinaryExpression(Expression.Binary expr)
        {
            return Parenthesise(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitCallExpression(Expression.Call expr)
        {
            return Parenthesise($"call {(expr.Callee as Expression.Variable).Name.Lexeme} ", expr.Arguments.ToArray());
        }

        public string VisitLiteralExpr(Expression.Literal expr)
        {
            return " " + expr.Value.ToString();
        }

        public string VisitVariableExpr(Expression.Variable expr)
        {
            return " " + expr.Name.Lexeme;
        }

        public string VisitUnaryExpression(Expression.Unary expr)
        {
            return Parenthesise(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesise(string name, params Expression[] exprs)
        {
            var builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(expr.Accept(this));
            }

            builder.Append(")");

            return builder.ToString();
        }
    }
}
