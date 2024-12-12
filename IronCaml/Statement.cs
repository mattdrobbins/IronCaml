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
            private readonly Token _name;
            private readonly List<Token> _params;
            private readonly Expression _body;

            public Token Name => _name;
            public Expression Body => _body;
            public HashSet<Token> Params => new HashSet<Token>(_params);

            public Function(Token name, List<Token> _params, Expression body)
            {
                _name = name;
                this._params = _params;
                _body = body;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.VisitFunctionStatement(this);
            }

            public virtual bool Equals(Function? obj)
            {
                return Name == obj.Name && Body == obj.Body && Params.SetEquals(obj.Params);
            }

            public override int GetHashCode() => HashCode.Combine(Body, Name, Params);
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
