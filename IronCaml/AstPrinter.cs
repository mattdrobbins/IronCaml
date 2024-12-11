using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class AstPrinter : Expression.Visitor<string>
    {
        public string VisitBinaryExpression(Expression.Binary expr)
        {
            throw new NotImplementedException();
        }

        public string VisitLiteralExpr(Expression.Literal expr)
        {
            throw new NotImplementedException();
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
