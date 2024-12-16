using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class LinqExpressionCreator : Expression.Visitor<LinqExpression>, Statement.Visitor<LinqExpression>
    {
        private Dictionary<string, LinqExpressions.ParameterExpression> _params = new Dictionary<string, LinqExpressions.ParameterExpression>();

        public LinqExpression ConvertToLinqExpression(Expression expression)
        {
            return expression.Accept(this);
        }

        public LinqExpressions.BlockExpression Convert(List<Statement> statements)
        {
            var expressions = statements.Select(statement => statement.Accept(this));
            return LinqExpression.Block(expressions);
        }

        public LinqExpression VisitBinaryExpression(Expression.Binary expr)
        {            
            var left = ConvertToLinqExpression(expr.Left);
            var right = ConvertToLinqExpression(expr.Right);
            
            switch (expr.Operator.Type)
            {
                case TokenType.PLUS:
                    return LinqExpression.Add(left, right);
                }

            return null;
        }

        public LinqExpression VisitCallExpression(Expression.Call expr)
        {
            throw new NotImplementedException();
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

            return LinqExpression.Variable(typeof(long), expr.Name.Lexeme);
        }

        public LinqExpression VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            var expressions = new List<LinqExpression>();
            var expression = stmt.Expression.Accept(this);
            LinqExpressions.ParameterExpression param = LinqExpression.Parameter(expression.Type, stmt.Name.Lexeme);

            return LinqExpression.Block(new LinqExpressions.ParameterExpression[] { param },
                LinqExpression.Assign(param, expression));              
        }

        public LinqExpression VisitFunctionStatement(Statement.Function stmt)
        {
            var parameters = new List<LinqExpressions.ParameterExpression>() { };
            foreach (var a in stmt.Params)
            {
                var param = LinqExpression.Parameter(typeof(long), a.Lexeme);
                parameters.Add(param);
                _params[a.Lexeme] = param;
            }

            var parray = parameters.ToArray();
            var body = stmt.Body.Accept(this);

            foreach (var a in stmt.Params)
            {
                _params.Remove(a.Lexeme);
            }

            return LinqExpression.Lambda(body, parray);
        }

        public LinqExpression VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            return  stmt.Expression.Accept(this);
        }
    }
}
