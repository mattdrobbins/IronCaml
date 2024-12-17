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
            return Or();
        }

        private Expression Or()
        {
            Expression expr = And();

            while (Match(TokenType.BOOL_OR))
            {
                var _operator = Previous();
                var right = Equality();
                expr = new Expression.Binary(expr, _operator, right);
            }

            return expr;
        }

        private Expression And()
        {
            var expr = Equality();

            while (Match(TokenType.BOOL_AND))
            {
                var _operator = Previous();
                var right = Equality();
                expr = new Expression.Binary(expr, _operator, right);
            }

            return expr;
        }

        private Expression Equality()
        {
            Expression expr = Term();

            while (Match(TokenType.EQUAL))
            {
                Token _operator = Previous();
                Expression right = Term();
                expr = new Expression.Binary(expr, _operator, right);
            }

            return expr;
        }

        private Expression Term()
        {
            var expr = Mod();

            while (Match(TokenType.MULTIPLY, TokenType.SUBTRACT, TokenType.PLUS))
            {
                Token _operator = Previous();
                Expression right = Mod();
                expr = new Expression.Binary(expr, _operator, right);
            }

            return expr;
        }

        private Expression Mod()
        {
            var expression = Call();

            if (Match(TokenType.MODINT))
            {
                Token _operator = Previous();
                var right = Call();
                expression = new Expression.Binary(expression, _operator, right);
            }

            return expression;
        }

        private Expression Call()
        {
            var expression = Unary();

            var didMatch = false;
            var arguments = new List<Expression>();

            while (Check(TokenType.IDENTIFIER) || Check(TokenType.INTEGER) || Check(TokenType.STRING))
            {
                didMatch = true;
                arguments.Add(Unary());
            }

            if (didMatch)
            {
                expression = new Expression.Call(expression, arguments);
            }

            return expression;
        }

        private Expression Unary()
        {
            if (Match(TokenType.BOOL_NOT))
            {
                var _operator = Previous();
                var right = Primary();
                return new Expression.Unary(_operator, right);
            }

            return Primary();
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

            if (Match(TokenType.LEFT_PAREN))
            {
                var exp = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expression.Grouping(exp);
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
            IronCaml.Error(token, message);
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
