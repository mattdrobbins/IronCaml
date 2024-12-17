using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml.Tests
{
    public class ScannerTest
    {
        [Fact]
        public void Function()
        {
            var expected = new List<Token>
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

            var text = File.ReadAllText("ExpressionExamples/function.ml");
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void FunctionSingleArgument()
        {
            var expected = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "double", null, 1),
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Token(TokenType.IDENTIFIER, "x", null, 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var text = File.ReadAllText("ExpressionExamples/functionSingleArgument.ml");
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void SingleExpression()
        {
            var expected = new List<Token>
            {
                new Token(TokenType.IDENTIFIER, "foo", null, 1),
                new Token(TokenType.EOF, "", null, 1)
            };

            var text = File.ReadAllText("ExpressionExamples/singleexpression.ml");
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void BinaryExpression()
        {
            var expected = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "a", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.INTEGER, "2", 2L, 1),
                new Token(TokenType.PLUS, "+", null, 1),
                new Token(TokenType.INTEGER, "2", 2L, 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var text = File.ReadAllText("ExpressionExamples/binary.ml");
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();

            Assert.Equal(expected, tokens);
        }
    }
}
