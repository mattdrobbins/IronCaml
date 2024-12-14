using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqExpressions = System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace IronCaml
{
    public class LinqExpressionCreator : Expression.Visitor<LinqExpression>, Statement.Visitor<LinqExpression>
    {
        public System.Linq.Expressions.Expression ConvertToLinqExpression(Expression expression)
        {
            return expression.Accept(this);
        }

        public System.Linq.Expressions.BlockExpression Convert(List<Statement> statements)
        {
            var expressions = statements.Select(statement => statement.Accept(this));
            return System.Linq.Expressions.Expression.Block(expressions);
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
            return LinqExpression.Variable(typeof(long), expr.Name.Lexeme);
        }

        public LinqExpression VisitLetDeclerationStatment(Statement.LetDecleration stmt)
        {
            var expressions = new List<LinqExpression>();
            var expression = stmt.Expression.Accept(this);
            LinqExpressions.ParameterExpression param = LinqExpression.Variable(expression.Type, stmt.Name.Lexeme);

            return LinqExpression.Block(new LinqExpressions.ParameterExpression[] { param },
                LinqExpression.Assign(param, expression));              
        }

        public LinqExpression VisitFunctionStatement(Statement.Function stmt)
        {
            var parameters = new List<LinqExpressions.ParameterExpression>() { };
            foreach (var a in stmt.Params)
            {
                parameters.Add(LinqExpression.Parameter(typeof(long), a.Lexeme));
            }

            var parray = parameters.ToArray();
            var body = stmt.Body.Accept(this);
            return LinqExpression.Lambda(body, parray);
        }

        public LinqExpression VisitExpressionStatement(Statement.ExpressionStatement stmt)
        {
            return  stmt.Expression.Accept(this);
        }
    }
}
