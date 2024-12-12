using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronCaml.Statement;

namespace IronCaml
{
    public class Parser
    {
        private class ParseException : Exception
        {

        }

        private List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;  
        }
        public List<Statement> Parse()
        {
            List<Statement> statements = [];

            while (!IsAtEnd())
            {
                statements.Add(Decleration());
            }

            return statements;
        }

        private Statement Decleration()
        {
            if (Match(TokenType.LET)) return Let();
            return ExpressionStatement();
        }

        private Statement Let()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect identifier");

            Expression expression = null;

            var arguments = new List<Token>();

            while(Match(TokenType.IDENTIFIER))
            {
                var argument = Previous();
                arguments.Add(argument);
            }

            Consume(TokenType.EQUAL, "Expect equals after let");

            expression = Expression();

            if (arguments.Any())
            {
                return new Statement.Function(name, arguments, expression);
            }

            return new Statement.LetDecleration(name, expression);
        }


        private Statement ExpressionStatement()
        {
            var value = Expression();
            return new ExpressionStatement(value);
        }

        private Expression Expression()
        {
            var expression = Call();

            if (Match(TokenType.PLUS))
            {
                Token _operator = Previous();
                var right = Primary();
                expression = new Expression.Binary(expression, _operator, right);
            }

            return expression;
        }

        private Expression Call()
        {
            var expression = Primary();

            var didMatch = false;
            var arguments = new List<Expression>();

            while (Check(TokenType.IDENTIFIER) || Check(TokenType.INTEGER) || Check(TokenType.STRING))
            {
                didMatch = true;
                arguments.Add(Primary());
            }

            if (didMatch)
            {
                expression = new Expression.Call(expression, arguments);
            }

            return expression;
        }

        private Expression Primary()
        {
            if (Match(
                TokenType.STRING, 
                TokenType.INTEGER, 
                TokenType.FLOATINGPOINT,
                TokenType.CHAR))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new Expression.Variable(Previous());
            }

            throw Error(Peek(), "Expect expression.");
        }

        private bool Match(params TokenType[] tokenTypes)
        {
            foreach (TokenType tokenType in tokenTypes)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, String message)
        {
            Program.Error(token, message);
            return new ParseException();
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}
