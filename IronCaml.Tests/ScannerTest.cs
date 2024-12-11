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

        [Fact]
        public void ScanExerciseOne()
        {
            var expected = new List<Token>
            {
                new Token(TokenType.LET, "let", null, 1),
                new Token(TokenType.IDENTIFIER, "hello", null, 1),
                new Token(TokenType.EQUAL, "=", null, 1),
                new Token(TokenType.STRING, "\"Hello, World!\"", "Hello, World!", 1),
                new Token(TokenType.EOF, "", null, 1),
            };

            var text = File.ReadAllText("ExercismExamples/hello.ml");
            var scanner = new Scanner(text);
            var tokens = scanner.ScanTokens();

            Assert.Equal(expected, tokens);
        }
    }
}
