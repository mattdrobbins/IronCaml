using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IronCaml
{
    public abstract record Statement
    {
        public interface Visitor<R>
        {
            R VisitLetDeclerationStatment(LetDecleration stmt);

            R VisitFunctionStatement(Function stmt);

            R VisitExpressionStatement(ExpressionStatement stmt);
        }

        public record Function : Statement
        {
            private readonly Expression.Function _function;

            public Expression.Function Func => _function;

            public Function(Expression.Function function)
            {
                _function = function;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitFunctionStatement(this);
            }

            public virtual bool Equals(Function? obj)
            {
                return (Func == obj.Func);
            }

            public override int GetHashCode() => HashCode.Combine(Func);
        }

        public record ExpressionStatement : Statement
        {
            private readonly Expression _expression;

            public Expression Expression => _expression;

            public ExpressionStatement(Expression expression)
            {
                _expression = expression;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitExpressionStatement(this);
            }
        }

        public record LetDecleration : Statement
        {
            private readonly Token _name;
            private readonly Expression _expression;

            public LetDecleration(Token name, Expression expression)
            {
                _name = name;
                _expression = expression;
            }

            public Token Name => _name;
            public Expression Expression => _expression;

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitLetDeclerationStatment(this);
            }
        }

        public abstract T Accept<T> (Visitor<T> visitor);
    }
}
