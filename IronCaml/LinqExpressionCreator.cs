using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class LinqExpressionCreator : Expression.Visitor<LinqExpression>, Statement.Visitor<LinqExpression>
    {
        private Dictionary<string, LinqExpressions.ParameterExpression> _params = new Dictionary<string, LinqExpressions.ParameterExpression>();
        private Dictionary<string, LambdaExpression> _functions = new Dictionary<string, LambdaExpression>();
        private Dictionary<string, ParameterExpression> _globals = new Dictionary<string, ParameterExpression>();
        private Environment _environment = new Environment();

        public LinqExpression ConvertToLinqExpression(Expression expression)
        {
            return expression.Accept(this);
        }

        public void SetGlobals(string name, LinqExpressions.ParameterExpression value)
        {
            _globals[name] = value;
        }

        public LinqExpressions.BlockExpression Convert(List<Statement> statements)
        {
            var expressions = statements.Select(statement => statement.Accept(this));
            if (!expressions.Any()) 
            {
                expressions = [LinqExpression.Empty()];
            }
            return LinqExpression.Block(_globals.Values, expressions);
        }

        public LinqExpression VisitBinaryExpression(Expression.Binary expr)
        {
            var left = ConvertToLinqExpression(expr.Left);
            var right = ConvertToLinqExpression(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.PLUS:
                    return LinqExpression.Add(left, right);
                case TokenType.MULTIPLY:
                    return LinqExpression.Multiply(left, right);
                case TokenType.DIVIDE:
                    return LinqExpression.Divide(left, right);
                case TokenType.SUBTRACT:
                    return LinqExpression.Subtract(left, right);
                case TokenType.MODINT:
                    return LinqExpression.Modulo(left, right);
                case TokenType.EQUAL:
                    return LinqExpression.Equal(left, right);
                case TokenType.BOOL_AND:
                    return LinqExpression.And(left, right);
                case TokenType.BOOL_OR:
                    return LinqExpression.Or(left, right);
            }

            return null;
        }


        public LinqExpression VisitUnaryExpression(Expression.Unary expr)
        {
            var right = ConvertToLinqExpression(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BOOL_NOT:
                    return LinqExpression.Not(right);
            }

            return null;
        }

        public LinqExpression VisitCallExpression(Expression.Call expr)
        {
            return LinqExpression.Invoke(_functions[(expr.Callee as Expression.Variable).Name.Lexeme],
                expr.Arguments.Select(s => s.Accept(this)));
        }

        public LinqExpression VisitLiteralExpr(Expression.Literal expr)
        {
            return LinqExpression.Constant(expr.Value);
        }

        public LinqExpression VisitVariableExpr(Expression.Variable expr)
        {
            if (_params.ContainsKey(expr.Name.Lexeme))
            {
                return _params[expr.Name.Lexeme];
            }
            if (_environment.Get(expr.Name) != null)
            {
                return _environment.Get(expr.Name);
            }

            return _globals[expr.Name.Lexeme];
        }

        public LinqExpression VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            var expressions = new List<LinqExpression>();
            var expression = stmt.Expression.Accept(this);
            var param = _globals[stmt.Name.Lexeme];

            return LinqExpression.Assign(param, expression);
        }

        public LinqExpression VisitLetExpression(Expression.LetExpression expr)
        {
            var previous = _environment;
            _environment = new Environment(previous);
            var parameter = LinqExpression.Variable(expr.Initialiser.ResultType(), expr.Name.Lexeme);
            _environment.Define(expr.Name.Lexeme, parameter);
            var body =  LinqExpression.Block([parameter], [LinqExpression.Assign(parameter, expr.Initialiser.Accept(this)),
                expr.Body.Accept(this)]);
            _environment = previous;
            return body;
        }

        public LinqExpression VisitFunctionStatement(Statement.Function stmt)
        {
            return stmt.Func.Accept(this);
        }

        public LinqExpression VisitGroupingExpression(Expression.Grouping expr)
        {
            return expr.Expression.Accept(this);
        }

        public LinqExpression VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            return stmt.Expression.Accept(this);
        }

        public LinqExpression VisitAssignmentExpression(Expression.Assignment expr)
        {
            throw new NotImplementedException();
        }

        public LinqExpression VisitFunctionExpression(Expression.Function expr)
        {
            var parameters = new List<LinqExpressions.ParameterExpression>() { };
            foreach (var a in expr.Params)
            {
                var param = LinqExpression.Parameter(typeof(long), a.Lexeme);
                parameters.Add(param);
                _params[a.Lexeme] = param;
            }

            var parray = parameters.ToArray();
            var body = expr.Body.Accept(this);

            foreach (var a in expr.Params)
            {
                _params.Remove(a.Lexeme);
            }

            var function = LinqExpression.Lambda(body, parray);
            _functions[expr.Name.Lexeme] = function;
            return function;
        }
    }
}
