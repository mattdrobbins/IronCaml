using static System.Net.Mime.MediaTypeNames;

namespace IronCaml
{
    public class Program
    {
        static bool _hadError = false;
        static bool _hadRuntimeError = false;

        static void Main(string[] args)
        {
            RunPrompt();
        }

        private static void RunPrompt()
        {
            var interperater = new Interperater(true);

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (line == "\u0004")
                {
                    break;
                }

                Run(line, interperater);
            }
        }

        private static void Run(string line, Interperater interperater)
        {
            var scanner = new Scanner(line);
            var tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            List<Statement> stmts = parser.Parse();

            interperater.Interperate(stmts);
        }

        public static void RuntimeError(RuntimeException runtimeException)
        {
            Console.WriteLine($"{runtimeException.Message} \n " +
                $"[{runtimeException.Token.Line} \" ]");
            _hadRuntimeError = true;
        }

        public static void Error(int line, string message)
        {
            Report(line, " ", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where} : {message}");
            _hadError = true;
        }


        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }
    }
}