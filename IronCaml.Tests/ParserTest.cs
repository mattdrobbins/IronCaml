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
