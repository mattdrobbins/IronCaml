using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public enum TokenType
    {     
        // SINGLE CHAR TOKENS
        PLUS,
        MULTIPLY,
        SUBTRACT,

        // LITERALS
        IDENTIFIER,
        INTEGER,
        FLOATINGPOINT,
        STRING,
        CHAR,

        // KEYWORDS
        LET,
        IN,
        EQUAL,
        EOF,
        FUNCTION,
        FUN,
        TYPE,
    }
}
