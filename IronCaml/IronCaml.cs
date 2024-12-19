using System.Data.SqlTypes;
using System.Linq.Expressions;
using static IronCaml.Statement;
using static System.Net.Mime.MediaTypeNames;

namespace IronCaml
{
    public class IronCaml
    {
        static bool _hadError = false;
        static bool _hadRuntimeError = false;

        public static T Execute<T>(string ocamlCode, bool printAst = false) where T : System.Delegate
        {
            var scanner = new Scanner(ocamlCode);
            var tokens = scanner.ScanTokens();

            Parser parser = new Parser(tokens);
            List<Statement> stmts = parser.Parse();

            if (printAst)
            {
                var astPrinter = new AstPrinter();
                foreach (var s in stmts)
                {
                    if (s is Function f)
                    {
                        Console.WriteLine(astPrinter.Print(f.Func.Body));
                    }
                }
            }

            var creator = new LinqExpressionCreator();
            var resolver = new VariableResolver(creator);
            resolver.Resolve(stmts);

            var linqExpression = creator.Convert(stmts);

            if (linqExpression.Result is LambdaExpression le)
            {
                return (T)(le.Compile());
            }

            return (T)(LinqExpression.Lambda(linqExpression).Compile());
        }

        static void Main(string[] args)
        {            
            RunPrompt();
        }

        private static void RunPrompt()
        {
            Console.WriteLine("OCaml running on .NET");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line == "\u0004")
                {
                    break;
                }

                try
                {
                    var result = Execute<Delegate>(line, true);
                    try
                    {
                        Console.WriteLine(result.DynamicInvoke());
                    }
                    catch (Exception)
                    {

                        Console.WriteLine(result);
                    }

                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
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