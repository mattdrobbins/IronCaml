using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IronCaml
{
    public abstract record Expression
    {
        public abstract Type ResultType();

        public interface Visitor<R>
        {
            R VisitLiteralExpr(Literal expr);

            R VisitBinaryExpression(Binary expr);

            R VisitUnaryExpression(Unary expr);

            R VisitVariableExpr(Variable expr);

            R VisitCallExpression(Call expr);

            R VisitGroupingExpression(Grouping expr);
        }

        public record Call : Expression
        {
            private readonly Expression _callee;
            private readonly List<Expression> _arguments;

            public Expression Callee => _callee;
            public List<Expression> Arguments => _arguments;

            public Call(Expression callee, List<Expression> arguments)
            {
                _callee = callee;
                _arguments = arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpression(this);
            }

            public virtual bool Equals(Call? obj)
            {
                return _callee == obj.Callee && Arguments.SequenceEqual(obj.Arguments);
            }

            public override int GetHashCode() => HashCode.Combine(Callee, Arguments);

            public override Type ResultType()
            {
                throw new NotImplementedException();
            }
        }

        public record Unary : Expression
        {
            private readonly Token _operator;
            private readonly Expression _right;

            public Expression Right => _right;
            public Token Operator => _operator;

            public Unary(Token _operator, Expression right)
            {
                this._operator = _operator;
                _right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpression(this);
            }

            public override Type ResultType()
            {
                throw new NotImplementedException();
            }
        }

        public record Grouping : Expression
        {
            private readonly Expression _expression;

            public Expression Expression => _expression;
   
            public Grouping(Expression expression)
            {
                _expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpression(this);
            }

            public override Type ResultType()
            {
                throw new NotImplementedException();
            }
        }

        public record Variable : Expression
        {
            private readonly Token _name;

            public Token Name => _name;

            public Variable(Token name)
            {
                _name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }

            public override Type ResultType()
            {
                throw new NotImplementedException();
            }
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

            public override Type ResultType() => _value.GetType();
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

            public override Type ResultType()
            {
                switch (_operator.Type)
                {
                    case TokenType.PLUS:
                    case TokenType.MULTIPLY:
                    case TokenType.SUBTRACT:
                        return typeof(long);
                    case TokenType.BOOL_NOT: 
                    case TokenType.BOOL_OR:
                        return typeof(bool);
                    default:
                        return typeof(long);
                }
            }
        }

        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
