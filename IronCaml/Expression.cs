using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public abstract record Expression
    {
        public interface Visitor<R>
        {
            R VisitLiteralExpr(Literal expr);

            R VisitBinaryExpression(Binary expr);

        }

        public record Literal : Expression
        {
            private readonly object _value;

            public Literal(object value)
            {
                _value = value;
            }

            public object Value => _value;

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public record Binary : Expression
        {
            private readonly Expression _left;
            private readonly Token _operator;
            private readonly Expression _right;

            public Expression Left => _left;
            public Expression Right => _right;
            public Token Operator => _operator;

            public Binary(Expression left, Token _operator, Expression right)
            {
                _left = left;
                this._operator = _operator;
                _right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpression(this);
            }
        }

        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
