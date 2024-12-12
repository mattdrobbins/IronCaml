using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml.Tests
{
    public class ParserTest
    {
        [Fact]
        public void Call()
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
                new Token(TokenType.IDENTIFIER, "add_two_numbers", null, 2),
                new Token(TokenType.INTEGER, "5", 5L, 2),
                new Token(TokenType.INTEGER, "9", 9L, 2),
                new Token(TokenType.EOF, "", null, 2),
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
                    )),

                new Statement.ExpressionStatement(new Expression.Call(
                    new Expression.Variable(new Token(TokenType.IDENTIFIER, "add_two_numbers", null, 2)),
                    new List<Expression>
                    {
                        new Expression.Literal(5L),
                        new Expression.Literal(9L)
                    }))

            };

            var parser = new Parser(tokens);
            var result = parser.Parse();

            Assert.Equal(expected, result);
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
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SingleExpression()
        {
            var tokens = new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "foo", null, 1),
                new Token(TokenType.EOF, "", null, 1)
            };

            var expected = new List<Statement>
            {
                new Statement.ExpressionStatement(
                    new Expression.Variable(
                        new Token(TokenType.IDENTIFIER, "foo", null, 1)))
            };

            var parser = new Parser(tokens);
            var result = parser.Parse();

            Assert.Equal(expected, result);
        }

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

            var expected = new List<Statement>
            {
                new Statement.LetDecleration(new Token(TokenType.IDENTIFIER, "a", null, 1)
                ,new Expression.Binary(new Expression.Literal(2L), new Token(TokenType.PLUS, "+", null, 1),
                new Expression.Literal(2L)))               
            };

            var parser = new Parser(tokens);
            var result = parser.Parse();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseExerciseOne()
        {
            var tokens = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "hello", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.STRING, "\"Hello, World!\"", "Hello, World!", 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var expected = new List<Statement>
            {
                new Statement.LetDecleration(new Token(TokenType.IDENTIFIER, "hello", null, 1)
                , new Expression.Literal("Hello, World!"))
            };

            var parser = new Parser(tokens);
            var result = parser.Parse();

            Assert.Equal(expected, result);
        }
    }
}
