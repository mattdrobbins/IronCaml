using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public record Token(TokenType Type, string? Lexeme, object? Literal, int Line)
    {
        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
