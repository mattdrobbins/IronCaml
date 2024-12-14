using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqExpressions = System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;
using System.Linq.Expressions;

namespace IronCaml.Tests
{
    public class LinqExpressionCreatorTests
    {
        [Fact]
        public void BinaryExpression()
        {
            var tokens = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "a", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.INTEGER, "2", 2L, 1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Token(TokenType.INTEGER, "2", 2L, 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var parser = new Parser(tokens);
            var result = parser.Parse();

            var creator = new LinqExpressionCreator();
            var dlr = creator.Convert(result);           
            var x = LinqExpression.Lambda<Func<long>>(dlr).Compile();
            var r = x();
            Assert.Equal(4L, r);
        }

        [Fact]
        public void Function()
        {
            var tokens = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "add_two_numbers", null, 1),
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.IDENTIFIER, "y", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Token(TokenType.IDENTIFIER, "y", null, 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var expectedParams = new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.IDENTIFIER, "y", null, 1),
            };

            var expected = new List<Statement>
            {
                new Statement.Function(
                    new Token(TokenType.IDENTIFIER, "add_two_numbers", null, 1),
                    expectedParams, new Expression.Binary(
                        new Expression.Variable(new Token(TokenType.IDENTIFIER, "x", null, 1)),
                        new Token(TokenType.PLUS, "+", null, 1),
                        new Expression.Variable(new Token(TokenType.IDENTIFIER, "y", null, 1))
                    ))
            };

            var parser = new Parser(tokens);
            var result = parser.Parse();
            var creator = new LinqExpressionCreator();
            var dlr = creator.Convert(result);
            var r = dlr.Result;

            LinqExpressions.ParameterExpression x2 = LinqExpression.Parameter(typeof(long), "x");
            LinqExpressions.ParameterExpression y2 = LinqExpression.Parameter(typeof(long), "y");
            var addExpression = LinqExpression.Add(x2, y2);
            var lambdaExpression = LinqExpression.Lambda<Func<long, long, long>>(addExpression, x2, y2);
            var resultx = lambdaExpression.Compile()(3, 5);

            if (r is Expression<Func<long,long,long>> s)
            {
                var x = s.Compile()(3,5);
            }

            Assert.Equal(expected, result);
        }

        [Fact]
        public void DLRTest3()
        {
            LinqExpressions.ParameterExpression x = LinqExpression.Parameter(typeof(long), "x");
            LinqExpressions.ParameterExpression y = LinqExpression.Parameter(typeof(long), "y");
            var addExpression = LinqExpression.Add(x, y);
            var lambdaExpression = LinqExpression.Lambda<Func<long, long, long>>(addExpression, x, y);
            var result = lambdaExpression.Compile()(3, 5);
        }

        [Fact]
        public void AddTwoParams()
        {
            LinqExpressions.ParameterExpression x = LinqExpression.Parameter(typeof(int), "x");
            LinqExpressions.ParameterExpression y = LinqExpression.Parameter(typeof(int), "y");
            var result = LinqExpression.Add(x, y);
        }

        [Fact]
        public void DLRTest2()
        {
            LinqExpressions.ParameterExpression x = LinqExpression.Variable(typeof(String), "x");
            var block = LinqExpression.Block([x], LinqExpression.Assign(x, LinqExpression.Constant("Hello")));

            Func<string> blockDelegate = LinqExpression.Lambda<Func<string>>(block).Compile();
            var result = blockDelegate();
        }

        [Fact]
        public void DLRTest()
        {
            LinqExpressions.ParameterExpression param = LinqExpression.Variable(typeof(long), "a");
            var block = LinqExpression.Block(new LinqExpressions.ParameterExpression[] { param },
                LinqExpression.Assign(param, LinqExpression.Constant(2L)));

        }
    }
}
