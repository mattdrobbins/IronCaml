using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    internal class ParameterResolver : Expression.Visitor<object>, Statement.Visitor<object>
    {
        private Dictionary<Token, Type> _paramTypes;
        private Type? _enclosingExpected = null;

        public void ResolveParams (List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                statement.Accept(this);
            }
        }

        public object VisitAssignmentExpression(Expression.Assignment expr)
        {
            return null;
        }

        public object VisitBinaryExpression(Expression.Binary expr)
        {
            switch (expr.Operator.Type)
            {
                case TokenType.PLUS:
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                case TokenType.SUBTRACT:
                    _enclosingExpected = typeof(long);
                    break;
                case TokenType.BOOL_AND:
                case TokenType.BOOL_OR:
                    _enclosingExpected = typeof(bool);
                break;
            }

            expr.Left.Accept(this);
            expr.Right.Accept(this);

            return null;
        }

        public object VisitCallExpression(Expression.Call expr)
        {
            return null;
        }

        public object VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            return null;
        }

        public object VisitFunctionExpression(Expression.Function expr)
        {
            _paramTypes = expr.ParamTypes;
            expr.Body.Accept(this);
            _paramTypes = null;
            return null;
        }

        public object VisitFunctionStatement(Statement.Function stmt)
        {
            stmt.Func.Accept(this);
            return null;
        }

        public object VisitGroupingExpression(Expression.Grouping expr)
        {
            return null;
        }

        public object VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            return null;
        }

        public object VisitLetExpression(Expression.LetExpression expr)
        {
            return null;
        }

        public object VisitLiteralExpr(Expression.Literal expr)
        {
            return null;
        }

        public object VisitUnaryExpression(Expression.Unary expr)
        {
            switch (expr.Operator.Type)
            {
                case TokenType.BOOL_NOT:
                    _enclosingExpected = typeof(bool);
                    break;
            }

            expr.Right.Accept(this);

            return null;
        }

        public object VisitVariableExpr(Expression.Variable expr)
        {
            if (_paramTypes != null)
            {
                if (_paramTypes.ContainsKey(expr.Name) && _paramTypes[expr.Name] != _enclosingExpected)
                {
                    throw new RuntimeException(expr.Name, "Type Mismatch");
                }

                _paramTypes[expr.Name] = _enclosingExpected;
            }

            return null;
        }
    }
}
