using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronCaml
{
    public class RuntimeException : Exception
    {
        public RuntimeException(Token token, string message) : base(message)
        {
            Token = token;
        }

        public Token Token { get; private set; }
    }
}
